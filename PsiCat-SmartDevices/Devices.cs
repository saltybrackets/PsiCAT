namespace PsiCat.SmartDevices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using YeelightAPI;


    public class Devices
    {
        public ILogger Logger { get; set; }
        
        public IEnumerable<Device> Lights { get; private set; }

        public async Task LocateAll()
        {
            //NetworkDiscovery.PingAll();
            DeviceLocator.UseAllAvailableMulticastAddresses = true;
            this.Lights = await DeviceLocator.DiscoverAsync();

            IEnumerable<Device> enumerable = this.Lights as Device[] ?? this.Lights.ToArray();
            if (!enumerable.Any())
            {
                this.Logger.LogInfo("No lights found.");
                return;
            }
			
            DeviceGroup deviceGroup = new DeviceGroup();
            foreach (Device light in enumerable)
            {
                this.Logger.LogInfo($"Found Light: {light.Model} on {light.Hostname}");
                deviceGroup.Add(light);
            }
			
            this.Logger.LogInfo($"Found {deviceGroup.Count} lights.");
        }
    }
}