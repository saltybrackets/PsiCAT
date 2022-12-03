namespace PsiCat.SmartDevices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using YeelightAPI;


    public class SmartLights
    {
        public ILogger Logger { get; set; }

        // IP : YeelightDevice
        public Dictionary<string, YeelightDevice> Lights { get; private set; }

        private Dictionary<string, DeviceGroup> LightGroups { get; set; }
        
        public async Task LocateAll(SmartDevicesConfig config = null)
        {
            DeviceLocator.UseAllAvailableMulticastAddresses = true;
            IEnumerable<YeelightDevice> foundLights = await DeviceLocator.DiscoverAsync();

            YeelightDevice[] lights = foundLights as YeelightDevice[] ?? foundLights.ToArray();
            if (lights.Length == 0)
            {
                this.Logger.Log("No lights found.");
                return;
            }

            this.Lights = new Dictionary<string, YeelightDevice>();
            foreach (YeelightDevice light in lights)
            {
                this.Logger.Log($"Found Light: {light.Model} on {light.Hostname}");
                this.Lights.Add(light.Hostname, light);
                await light.Connect();
                if (config != null)
                    light.ApplyToConfig(config);
            }
			
            this.Logger.Log($"Found {this.Lights.Count} lights.");
        }
    }
}