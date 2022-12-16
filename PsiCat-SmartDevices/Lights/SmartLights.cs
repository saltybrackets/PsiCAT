namespace PsiCat.SmartDevices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using YeelightAPI;


    public class SmartLights
    {
        public SmartLights(SmartDevicesConfig config)
        {
            this.Config = config;
        }


        /// <summary>
        /// Whether lights are currently being located.
        /// </summary>
        public bool IsLocating { get; set; }

        /// <summary>
        /// All loaded smart lights, keyed on device IP.
        /// </summary>
        public Dictionary<string, ISmartLight> Lights { get; } = 
            new Dictionary<string, ISmartLight>();

        /// <summary>
        /// Logging interface to use for logging.
        /// </summary>
        public ILogger Logger { get; set; }

        private SmartDevicesConfig Config { get; }


        /// <summary>
        /// Connect to all loaded smart lights.
        /// </summary>
        public async Task ConnectToAll()
        {
            if (this.Logger != null)
                this.Logger.Log($"Attempting connection to {this.Lights.Count} smart lights...");
            
            int successes = 0;
            int failures = 0;
            foreach (ISmartLight smartLight in this.Lights.Values)
            {
                if (await smartLight.IsConnected())
                    continue;
                
                if (smartLight.Connect().Wait(this.Config.DeviceConnectionTimeout))
                {
                    successes++;
                }
                else
                {
                    failures++;
                }
            }

            if (this.Logger != null)
            {
                if (failures > 0)
                    this.Logger.Log($"Failed to connect to {failures} smart lights.");    
                if (successes > 0)
                    this.Logger.Log($"Successfully connected to {successes} smart lights.");
            }
                
        }


        /// <summary>
        /// Disconnected from all loaded smart lights.
        /// </summary>
        public async void DisconnectAll()
        {
            foreach (ISmartLight smartLight in this.Lights.Values)
            {
                await smartLight.Disconnect();
            }
            
            if (this.Logger != null)
                this.Logger.Log($"Disconnected from all smart lights.");
        }


        /// <summary>
        /// Load all available smart lights from network and configuration.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ISmartLight>> LocateAll()
        {
            this.IsLocating = true;
            this.Lights.Clear();

            List<ISmartLight> smartLights = new List<ISmartLight>();

            IEnumerable<ISmartLight> yeelights = await FindAllYeelights(); 
            
            // Aggregate all discovered lights
            smartLights.AddRange(yeelights);
            foreach (ISmartLight smartLight in smartLights)
            {
                this.Lights.Add(smartLight.IP, smartLight);    
            }
            
            if (this.Logger != null)
                this.Logger.Log($"Loaded {smartLights.Count} total smart lights.");
            
            this.IsLocating = false;
            return smartLights;
        }


        private void AddUndiscoveredYeelights(List<YeelightDevice> yeelights)
        {
            // Add undiscovered lights remaining in config.
            int undiscoveredLights = 0;
            
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
                    undiscoveredLights++;
                }
            }

            if (undiscoveredLights > 0
                && this.Logger != null)
            {
                this.Logger.Log($"Loaded {undiscoveredLights} undiscovered Yeelights from config.");
            }
        }


        private async Task<List<YeelightDevice>> DiscoverNetworkYeelights()
        {
            List<YeelightDevice> yeelights = (await DeviceLocator.DiscoverAsync()).ToList();
            foreach (YeelightDevice yeelight in yeelights)
            {
                this.Logger.Log($"Discovered Yeelight: {yeelight.Model} on {yeelight.Hostname}");
            }

            return yeelights;
        }


        private async Task<IEnumerable<ISmartLight>> FindAllYeelights()
        {
            List<ISmartLight> smartLights = new List<ISmartLight>();

            List<YeelightDevice> yeelights = await DiscoverNetworkYeelights();
            AddUndiscoveredYeelights(yeelights);

            // Adapt Yeelights to ISmartLights
            foreach (YeelightDevice yeelight in yeelights)
            {
                SmartDevice smartDevice = this.Config.Devices
                    .FirstOrDefault(device => device.IP == yeelight.Hostname);
                if (smartDevice == null)
                {
                    smartDevice = new SmartDevice
                                      {
                                          IP = yeelight.Hostname,
                                          Manufacturer = SmartDeviceManufacturer.Yeelight,
                                          Name = yeelight.Name,
                                          Type = SmartDeviceType.Light
                                      };
                }
                    
                YeelightSmartLightAdapter adaptedYeelight = 
                    new YeelightSmartLightAdapter(yeelight, smartDevice);
                smartLights.Add(adaptedYeelight);
            }
            
            return smartLights;
        }
    }
}