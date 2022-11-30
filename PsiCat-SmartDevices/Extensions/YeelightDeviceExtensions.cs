namespace PsiCat.SmartDevices
{
    using System.Collections.Generic;
    using System.Linq;
    using YeelightAPI;


    public static class YeelightDeviceExtensions
    {
        public static SmartDevice ToSmartDevice(this YeelightDevice yeelightDevice)
        {
            SmartDevice smartDevice = new SmartDevice();
            smartDevice.Name = $"Unknown {yeelightDevice.Model} Light";
            smartDevice.IP = yeelightDevice.Hostname;

            return smartDevice;
        }


        public static void ApplyToConfig(
            this YeelightDevice yeelightDevice, 
            SmartDevicesConfig config)
        {
            if (config == null)
                return;
            
            if (config.Devices == null)
                config.Devices = new List<SmartDevice>();
            
            SmartDevice configEntry = config.Devices.FirstOrDefault(
                entry => entry.IP == yeelightDevice.Hostname);
            if (configEntry == null)
            {
                config.Devices.Add(yeelightDevice.ToSmartDevice());
            }
        }
    }
}