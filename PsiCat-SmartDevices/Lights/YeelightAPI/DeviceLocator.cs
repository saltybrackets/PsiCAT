﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YeelightAPI.Core;
using YeelightAPI.Events;
using YeelightAPI.Models;

namespace YeelightAPI
{
    /// <summary>
    ///   Finds devices through LAN
    /// </summary>
    public static class DeviceLocator
    {
        #region Constructors

        static DeviceLocator()
        {
            DeviceLocator.MaxRetryCount = 1;
        }

        #endregion

        #region Private Fields

        private const string _defaultSsdpMessage =
          "M-SEARCH * HTTP/1.1\r\nHOST: 239.255.255.250:1982\r\nMAN: \"ssdp:discover\"\r\nST: wifi_bulb";
        private const string _ssdpMessage =
            "M-SEARCH * HTTP/1.1\r\nHOST: {0}:1982\r\nMAN: \"ssdp:discover\"\r\nST: wifi_bulb";
        private static readonly List<object> _allPropertyRealNames = PROPERTIES.ALL.GetRealNames();
        private static readonly char[] _colon = { ':' };
        private static readonly string _defaultMulticastIPAddress = "239.255.255.250";
        private static readonly IPEndPoint _multicastEndPoint = new IPEndPoint(IPAddress.Parse(DeviceLocator._defaultMulticastIPAddress), 1982);
        private static readonly byte[] _ssdpDiagram = Encoding.ASCII.GetBytes(DeviceLocator._defaultSsdpMessage);
        private static readonly string _yeelightlocationMatch = "Location: yeelight://";

        #endregion Private Fields

        /// <summary>
        /// Retry count for network sockets lookup to find devices.
        /// </summary> 
        /// <value>The number of lookup retries. Default is 3.</value>
        /// <remarks>A single iteration will take a maximum of 1 second. Each iteration will poll in intervals of 10 ms to listen to an IP for devices. This means the value of <see cref="MaxRetryCount"/> is equivalent to execution time in seconds.</remarks>
        public static int MaxRetryCount { get; set; }

        /// <summary>
        /// Use all available multicast addresses for the discovery instead of the default multicast address only
        /// </summary>
        /// <remarks>Setting this parameter to true may result in a longer discovery time</remarks>
        
        /// <summary>
        /// Default multicast IP address used for the discovery. Can be changed to allow discovery on specific network configurations
        /// </summary>
        public static string DefaultMulticastIPAddress { get; set; } = DeviceLocator._defaultMulticastIPAddress;

        #region Depricated API. TODO: Remove

        /// <summary>
        ///   Notification Received event
        /// </summary>
        [Obsolete("Deprecated. Event will be removed in next release.")]
        public static event DeviceFoundEventHandler OnDeviceFound;

        /// <summary>
        ///   Notification Received event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [Obsolete("Deprecated. Event will be removed in next release.")]
        public delegate void DeviceFoundEventHandler(object sender, DeviceFoundEventArgs e);

        #region Public Methods

        /// <summary>
        ///   Discover devices in a specific Network Interface
        /// </summary>
        /// <param name="preferredInterface"></param>
        /// <returns></returns>
        [Obsolete("Deprecated. Use DiscoverAsync and overloads instead.")]
        public static async Task<List<YeelightDevice>> Discover(NetworkInterface preferredInterface)
        {
            List<Task<List<YeelightDevice>>> tasks = DeviceLocator.CreateDiscoverTasks(preferredInterface, DeviceLocator.MaxRetryCount);

            List<YeelightDevice>[] result = await Task.WhenAll(tasks);
            return result
              .SelectMany(devices => devices)
              .GroupBy(d => d.Hostname)
              .Select(g => g.First())
              .ToList();
        }

        /// <summary>
        ///   Discover devices in LAN
        /// </summary>
        /// <returns></returns>
        [Obsolete("Deprecated. Use DiscoverAsync and overloads instead.")]
        public static async Task<List<YeelightDevice>> Discover()
        {
            var tasks = new List<Task<List<YeelightDevice>>>();
            int retryCount = DeviceLocator.MaxRetryCount;
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces()
              .Where(n => n.OperationalStatus == OperationalStatus.Up))
            {
                tasks.AddRange(DeviceLocator.CreateDiscoverTasks(ni, retryCount));
            }


            List<YeelightDevice>[] result = await Task.WhenAll(tasks);
            return result
              .SelectMany(devices => devices)
              .GroupBy(d => d.Hostname)
              .Select(g => g.First())
              .ToList();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        ///   Create Discovery tasks for a specific Network Interface
        /// </summary>
        /// <param name="netInterface"></param>
        /// <param name="retryCount">Number of retries when lookup fails.</param>
        /// <returns></returns>
        [Obsolete("Deprecated. Use SearchNetworkForDevicesAsync instead")]
        private static List<Task<List<YeelightDevice>>> CreateDiscoverTasks(NetworkInterface netInterface, int retryCount)
        {
            var devices = new ConcurrentDictionary<string, YeelightDevice>();
            var tasks = new List<Task<List<YeelightDevice>>>();

            if (netInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 &&
                netInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
            {
                return tasks;
            }

            foreach (UnicastIPAddressInformation ip in netInterface.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily != AddressFamily.InterNetwork)
                {
                    continue;
                }

                for (var count = 0; count < retryCount; count++)
                {
                    Task<List<YeelightDevice>> t = Task.Run(
                      async () =>
                      {
                          var stopWatch = new Stopwatch();
                          try
                          {
                              using (var ssdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
                              {
                                  Blocking = false,
                                  Ttl = 1,
                                  UseOnlyOverlappedIO = true,
                                  MulticastLoopback = false
                              })
                              {
                                  ssdpSocket.Bind(new IPEndPoint(ip.Address, 0));
                                  ssdpSocket.SetSocketOption(
                          SocketOptionLevel.IP,
                          SocketOptionName.AddMembership,
                          new MulticastOption(DeviceLocator._multicastEndPoint.Address));

                                  ssdpSocket.SendTo(
                          DeviceLocator._ssdpDiagram,
                          SocketFlags.None,
                          DeviceLocator._multicastEndPoint);

                                  stopWatch.Start();
                                  while (stopWatch.Elapsed < TimeSpan.FromSeconds(1))
                                  {
                                      try
                                      {
                                          int available = ssdpSocket.Available;

                                          if (available > 0)
                                          {
                                              var buffer = new byte[available];
                                              int i = ssdpSocket.Receive(buffer, SocketFlags.None);

                                              if (i > 0)
                                              {
                                                  string response = Encoding.UTF8.GetString(buffer.Take(i).ToArray());
                                                  YeelightDevice yeelightDevice = DeviceLocator.GetDeviceInformationFromSsdpMessage(response);

                                                  //add only if no device already matching
                                                  if (devices.TryAdd(yeelightDevice.Hostname, yeelightDevice))
                                                  {
                                                      DeviceLocator.OnDeviceFound?.Invoke(null, new DeviceFoundEventArgs(yeelightDevice));
                                                  }
                                              }
                                          }
                                      }
                                      catch (SocketException)
                                      {
                                          // Continue polling
                                      }

                                      await Task.Delay(TimeSpan.FromMilliseconds(10));
                                  }

                                  stopWatch.Stop();
                              }
                          }
                          catch (SocketException)
                          {
                              return devices.Values.ToList();
                          }
                          finally
                          {
                              stopWatch.Stop();
                          }

                          return devices.Values.ToList();
                      });

                    tasks.Add(t);
                }
            }

            return tasks;
        }

        #endregion Depricated API. TODO: Remove

        #endregion Depricated API. TODO: Remove

        #region Async API Methods

        /// <summary>
        /// Discover devices in LAN
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<YeelightDevice>> DiscoverAsync() =>
          await DeviceLocator.DiscoverAsync(deviceFoundReporter: null);

        /// <summary>
        /// Discover devices in LAN
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<YeelightDevice>> DiscoverAsync(IProgress<YeelightDevice> deviceFoundReporter)
        {
            IEnumerable<Task<IEnumerable<YeelightDevice>>> tasks = NetworkInterface.GetAllNetworkInterfaces()
              .Where(networkInterface => networkInterface.OperationalStatus == OperationalStatus.Up)
              .Select(networkInterface => DeviceLocator.DiscoverAsync(networkInterface, deviceFoundReporter));

            IEnumerable<YeelightDevice>[] result = await Task.WhenAll(tasks);
            return result
              .SelectMany(devices => devices)
              .GroupBy(d => d.Hostname)
              .Select(g => g.First())
              .ToList();
        }

        /// <summary>
        /// Discover devices in a specific Network Interface
        /// </summary>
        /// <param name="networkInterface"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<YeelightDevice>> DiscoverAsync(NetworkInterface networkInterface) =>
          await DeviceLocator.DiscoverAsync(networkInterface, null);

        /// <summary>
        /// Discover devices in a specific Network Interface
        /// </summary>
        /// <param name="networkInterface"></param>
        /// <param name="deviceFoundReporter"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<YeelightDevice>> DiscoverAsync(
          NetworkInterface networkInterface,
          IProgress<YeelightDevice> deviceFoundReporter) =>
          await DeviceLocator.SearchNetworkForDevicesAsync(networkInterface, deviceFoundReporter);

        #endregion Async API Methods

        /// <summary>
        /// Create Discovery tasks for a specific Network Interface
        /// </summary>
        /// <param name="netInterface"></param>
        /// <param name="deviceFoundCallback"></param>
        /// <returns></returns>
        private static async Task<IEnumerable<YeelightDevice>> SearchNetworkForDevicesAsync(NetworkInterface netInterface, IProgress<YeelightDevice> deviceFoundCallback)
        {
            if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            {
                int retryCount = DeviceLocator.MaxRetryCount;
                
                return await Task.Run(
                  () => netInterface.GetIPProperties().UnicastAddresses
                    .Where(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    .AsParallel()
                    .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                    .SelectMany(ip => DeviceLocator.CheckSocketForDevices(ip, deviceFoundCallback, retryCount))
                    .ToList());
            }

            return new List<YeelightDevice>();
        }

        private static IEnumerable<YeelightDevice> CheckSocketForDevices(UnicastIPAddressInformation ip, IProgress<YeelightDevice> deviceFoundCallback, int retryCount)
        {
            // Use hash table for faster lookup, than List.Contains
            var devices = new Dictionary<string, YeelightDevice>();

            for (int count = 0; count < retryCount; count++)
            {
                try
                {
                    using (var ssdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                    {
                        var multicastIPEndpoint = _multicastEndPoint;
                        DeviceLocator.InitializeSocket(multicastIPEndpoint, ip, ssdpSocket);
                        DeviceLocator.GetDevicesFromSocket(multicastIPEndpoint, deviceFoundCallback, ssdpSocket, devices);
                    }
                }
                catch (SocketException)
                {
                    //return devices.Values.ToList();
                }
            }

            return devices.Values.ToList();
        }

        private static void InitializeSocket(IPEndPoint multicastIPEndpoint, UnicastIPAddressInformation ip, Socket ssdpSocket)
        {
            ssdpSocket.Blocking = false;
            ssdpSocket.Ttl = 2;
            ssdpSocket.UseOnlyOverlappedIO = true;
            ssdpSocket.MulticastLoopback = false;
            ssdpSocket.Bind(new IPEndPoint(ip.Address, 0));
            ssdpSocket.SetSocketOption(
              SocketOptionLevel.IP,
              SocketOptionName.AddMembership,
              new MulticastOption(multicastIPEndpoint.Address));
        }

        private static void GetDevicesFromSocket(IPEndPoint multicastIPEndpoint, IProgress<YeelightDevice> deviceFoundCallback, Socket ssdpSocket, Dictionary<string, YeelightDevice> devices)
        {
            ssdpSocket.SendTo(
                Encoding.ASCII.GetBytes(string.Format(DeviceLocator._ssdpMessage, multicastIPEndpoint.Address)),
                SocketFlags.None,
                multicastIPEndpoint
            );

            var stopWatch = Stopwatch.StartNew();
            try
            {
                while (stopWatch.Elapsed < TimeSpan.FromSeconds(1))
                {
                    try // Catch socket read exception
                    {
                        int available = ssdpSocket.Available;

                        if (available > 0)
                        {
                            var buffer = new byte[available];
                            int numberOfBytesRead = ssdpSocket.Receive(buffer, SocketFlags.None);

                            if (numberOfBytesRead > 0)
                            {
                                string response = Encoding.UTF8.GetString(buffer.Take(numberOfBytesRead).ToArray());
                                YeelightDevice yeelightDevice = DeviceLocator.GetDeviceInformationFromSsdpMessage(response);

                                if (!devices.ContainsKey(yeelightDevice.Hostname))
                                {
                                    devices.Add(yeelightDevice.Hostname, yeelightDevice);
                                    deviceFoundCallback?.Report(yeelightDevice);
                                }
                            }
                        }
                    }
                    catch (SocketException)
                    {
                        // Ignore SocketException and continue polling
                    }

                    Thread.Sleep(TimeSpan.FromMilliseconds(10));
                }
            }
            finally
            {
                stopWatch.Stop();
            }
        }

        /// <summary>
        ///   Gets the informations from a raw SSDP message (host, port)
        /// </summary>
        /// <param name="ssdpMessage"></param>
        /// <returns></returns>
        private static YeelightDevice GetDeviceInformationFromSsdpMessage(string ssdpMessage)
        {
            if (ssdpMessage != null)
            {
                string[] split = ssdpMessage.Split(new[] { Constants.LineSeparator }, StringSplitOptions.RemoveEmptyEntries);
                string host = null;
                int port = Constants.DefaultPort;
                var properties = new Dictionary<string, object>();
                var supportedMethods = new List<METHODS>();
                string id = null, firmwareVersion = null;
                MODEL model = default;

                foreach (string part in split)
                {
                    if (part.StartsWith(DeviceLocator._yeelightlocationMatch))
                    {
                        string url = part.Substring(DeviceLocator._yeelightlocationMatch.Length);
                        string[] hostnameParts = url.Split(DeviceLocator._colon, StringSplitOptions.RemoveEmptyEntries);
                        if (hostnameParts.Length >= 1)
                        {
                            host = hostnameParts[0];
                        }

                        if (hostnameParts.Length == 2)
                        {
                            int.TryParse(hostnameParts[1], out port);
                        }
                    }
                    else
                    {
                        string[] property = part.Split(DeviceLocator._colon);
                        if (property.Length == 2)
                        {
                            string propertyName = property[0].Trim();
                            string propertyValue = property[1].Trim();

                            if (DeviceLocator._allPropertyRealNames.Contains(propertyName))
                            {
                                properties.Add(propertyName, propertyValue);
                            }
                            else if (propertyName == "id")
                            {
                                id = propertyValue;
                            }
                            else if (propertyName == "model")
                            {
                                if (!RealNameAttributeExtension.TryParseByRealName(propertyValue, out model))
                                {
                                    model = default;
                                }
                            }
                            else if (propertyName == "support")
                            {
                                string[] supportedOperations = propertyValue.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                                foreach (string operation in supportedOperations)
                                {
                                    if (RealNameAttributeExtension.TryParseByRealName(operation, out METHODS method))
                                    {
                                        supportedMethods.Add(method);
                                    }
                                }
                            }
                            else if (propertyName == "fw_ver")
                            {
                                firmwareVersion = propertyValue;
                            }
                        }
                    }
                }

                return new YeelightDevice(host, port, id, model, firmwareVersion, properties, supportedMethods);
            }

            return null;
        }
    }
}