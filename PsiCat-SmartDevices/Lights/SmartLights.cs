namespace PsiCat.SmartDevices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using YeelightAPI;


    public class SmartLights
    {
        public ILogger Logger { get; set; }

        // IP : YeelightDevice
        public Dictionary<string, YeelightDevice> Lights { get; } = 
            new Dictionary<string, YeelightDevice>();

        private Dictionary<string, DeviceGroup> LightGroups { get; set; } = 
            new Dictionary<string, DeviceGroup>();
        
        public bool IsLocating { get; set; }
        
        public async Task LocateAll(SmartDevicesConfig config = null)
        {
            this.IsLocating = true;
            this.Lights.Clear();
            DeviceLocator.UseAllAvailableMulticastAddresses = true;
            
            IEnumerable<YeelightDevice> foundLights = await DeviceLocator.DiscoverAsync();

            YeelightDevice[] lights = foundLights as YeelightDevice[] ?? foundLights.ToArray();
            if (lights.Length == 0)
            {
                this.Logger.Log("No lights found.");
                return;
            }

            foreach (YeelightDevice light in lights)
            {
                this.Logger.Log($"Found Light: {light.Model} on {light.Hostname}");
                this.Lights.Add(light.Hostname, light);
                await light.Connect();
                if (config != null)
                    light.ApplyToConfig(config);
            }
			
            this.Logger.Log($"Found {this.Lights.Count} lights.");
            this.IsLocating = false;
        }
    }
}