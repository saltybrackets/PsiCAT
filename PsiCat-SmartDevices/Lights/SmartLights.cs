namespace PsiCat.SmartDevices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using YeelightAPI;


    public class SmartLights
    {
        public SmartLights(SmartDevicesConfig config)
        {
            this.Config = config;
        }


        public SmartDevicesConfig Config { get; }

        public bool IsLocating { get; set; }

        // IP : YeelightDevice
        public Dictionary<string, ISmartLight> Lights { get; } = 
            new Dictionary<string, ISmartLight>();

        public ILogger Logger { get; set; }

        private Dictionary<string, DeviceGroup> LightGroups { get; set; } = 
            new Dictionary<string, DeviceGroup>();


        public async Task ConnectToAll()
        {
            if (this.Logger != null)
                this.Logger.Log($"Attempting connection to {this.Lights.Count} smart lights...");
            
            int successes = 0;
            foreach (ISmartLight smartLight in this.Lights.Values)
            {
                if (await smartLight.IsConnected())
                    continue;
                
                if (smartLight.Connect().Wait(this.Config.DeviceConnectionTimeout))
                {
                    successes++;
                    if (this.Logger != null)
                        this.Logger.Log($"Connected to smart light: {smartLight.IP}");
                }
                else if (this.Logger != null)
                {
                    this.Logger.Log($"Failed to connect to smart light: {smartLight.IP}");
                }
            }
            
            if (this.Logger != null)
                this.Logger.Log($"Connected to {successes} total smart lights.");
        }


        public async void DisconnectAll()
        {
            foreach (ISmartLight smartLight in this.Lights.Values)
            {
                await smartLight.Disconnect();
            }
            
            if (this.Logger != null)
                this.Logger.Log($"Disconnected from all smart lights.");
        }


        public async Task<IEnumerable<ISmartLight>> LocateAll()
        {
            this.IsLocating = true;
            this.Lights.Clear();

            List<ISmartLight> smartLights = new List<ISmartLight>();

            IEnumerable<ISmartLight> yeelights = await LocateYeelights(); 
            
            // Aggregate all discovered lights
            smartLights.AddRange(yeelights);

            foreach (ISmartLight smartLight in smartLights)
            {
                this.Lights.Add(smartLight.IP, smartLight);    
            }
            
            if (this.Logger != null)
                this.Logger.Log($"Loaded {smartLights.Count} smart lights in total.");
            
            this.IsLocating = false;
            return smartLights;
        }


        private async Task<IEnumerable<ISmartLight>> LocateYeelights()
        {
            List<ISmartLight> smartLights = new List<ISmartLight>();

            // Try to discover lights on network first.
            List<YeelightDevice> yeelights = (await DeviceLocator.DiscoverAsync()).ToList();
            foreach (YeelightDevice yeelight in yeelights)
            {
                this.Logger.Log($"Discovered Yeelight: {yeelight.Model} on {yeelight.Hostname}");
            }

            // Add undiscovered lights remaining in config.
            IEnumerable<SmartDevice> configuredLights = this.Config.Devices.Where(
                device =>
                    device.Type == SmartDeviceType.Light
                    && device.Manufacturer == SmartDeviceManufacturer.Yeelight);
            foreach (SmartDevice configuredLight in configuredLights)
            {
                bool alreadyAdded = yeelights
                    .Any(device => device.Hostname == configuredLight.IP);
                
                if (!alreadyAdded)
                {
                    YeelightDevice yeelight = new YeelightDevice(configuredLight.IP);
                    yeelights.Add(yeelight);
                    if (this.Logger != null)
                        this.Logger.Log($"Loaded undiscovered Yeelight from config: {yeelight.Hostname}");
                }
            }

            // Adapt loaded Yeelights to ISmartLights
            foreach (YeelightDevice yeelight in yeelights)
            {
                YeelightSmartLightAdapter adaptedYeelight = new YeelightSmartLightAdapter(yeelight);
                smartLights.Add(adaptedYeelight);
            }
            
            return smartLights;
        }
    }
}