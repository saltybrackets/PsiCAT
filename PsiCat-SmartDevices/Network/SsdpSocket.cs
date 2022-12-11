namespace PsiCat.SmartDevices
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;


    public class SsdpSocket
    {
        private readonly string ipv4;
        private readonly int port;
        
        public SsdpSocket(string multicastIPV4, int port)
        {
            this.ipv4 = multicastIPV4;
            this.port = port;
        }
        

        public string SendMulticastData(
            string data,
            int ttl = 2)
        {
            Socket ssdpSocket = new Socket(
                AddressFamily.InterNetwork, 
                SocketType.Dgram, 
                ProtocolType.Udp);
            
            IPAddress multicastIpv4Address = IPAddress.Parse(this.ipv4);
            
            // Set time-to-live: Note, decrements by 1 for each router passthrough.
            ssdpSocket.SetSocketOption(
                optionLevel: SocketOptionLevel.IP,
                optionName: SocketOptionName.MulticastTimeToLive, 
                optionValue: ttl);
            
            // Bind to a port for listening.
            IPEndPoint bindEndpoint = new IPEndPoint(IPAddress.Any, this.port);
            ssdpSocket.Bind(bindEndpoint);
            
            // Join multicast group
            ssdpSocket.SetSocketOption(
                optionLevel: SocketOptionLevel.IP,
                optionName: SocketOptionName.AddMembership,
                optionValue: new MulticastOption(multicastIpv4Address));
            
            //ssdpSocket.Connect(ipEndpoint);
            
            // Send data
            IPEndPoint multicastEndpoint = new IPEndPoint(multicastIpv4Address, this.port);
            byte[] dataBytes = Encoding.ASCII.GetBytes(data);
            ssdpSocket.SendTo(
                dataBytes, dataBytes.Length, 
                SocketFlags.None, 
                multicastEndpoint);
            
            // Receive data
            StringBuilder aggregateData = new StringBuilder();
            Stopwatch stopWatch = Stopwatch.StartNew();
            while (stopWatch.Elapsed < TimeSpan.FromSeconds(1))
            {
                int availableBytes = ssdpSocket.Available;
                if (availableBytes > 0)
                {
                    byte[] receivedBytes = new byte[availableBytes];
                    int bytesReceived = ssdpSocket.Receive(receivedBytes, SocketFlags.None);
                    if (bytesReceived > 0)
                    {
                        aggregateData.Append(Encoding.UTF8.GetString(receivedBytes, 0, receivedBytes.Length));    
                    }    
                }
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
            }
            stopWatch.Stop();
            ssdpSocket.Close();

            return aggregateData.ToString();
        }


        public string ReceiveMulticastData()
        {
            Socket ssdpSocket = new Socket(
                AddressFamily.InterNetwork, 
                SocketType.Dgram, 
                ProtocolType.Udp);
            ssdpSocket.Blocking = false;
            ssdpSocket.Ttl = 1;
            ssdpSocket.UseOnlyOverlappedIO = true;
            ssdpSocket.MulticastLoopback = false;
            
            // Join multicast group
            IPAddress multicastIPAddress = IPAddress.Parse(this.ipv4);
            ssdpSocket.SetSocketOption(
                optionLevel: SocketOptionLevel.IP,
                optionName: SocketOptionName.AddMembership,
                optionValue: new MulticastOption(
                    @group: multicastIPAddress, 
                    mcint: IPAddress.Any));
            
            // Receive data
            StringBuilder aggregateData = new StringBuilder();
            Stopwatch stopWatch = Stopwatch.StartNew();
            while (stopWatch.Elapsed < TimeSpan.FromSeconds(1))
            {
                int availableBytes = ssdpSocket.Available;
                if (availableBytes > 0)
                {
                    byte[] dataBytes = new byte[availableBytes];
                    int bytesReceived = ssdpSocket.Receive(dataBytes, SocketFlags.None);
                    if (bytesReceived > 0)
                    {
                        aggregateData.Append(Encoding.UTF8.GetString(dataBytes, 0, dataBytes.Length));    
                    }    
                }
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
            }
            stopWatch.Stop();
            ssdpSocket.Close();

            return aggregateData.ToString();
        }
    }
}