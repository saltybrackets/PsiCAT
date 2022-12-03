namespace PsiCat.SmartDevices
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using YeelightAPI;
    using YeelightAPI.Models;


    public static class YeelightDeviceExtensions
    {
        public static SmartDevice ToSmartDevice(this YeelightDevice yeelightDevice)
        {
            SmartDevice smartDevice = new SmartDevice();
            smartDevice.Name = $"{yeelightDevice.Model} Yeelight";
            smartDevice.IP = yeelightDevice.Hostname;
            smartDevice.Type = SmartDeviceType.Light;

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


        public static async Task<bool> IsOn(this YeelightDevice yeelightDevice)
        {
            if (!yeelightDevice.IsConnected)
                return false;
            object powerProperty = await yeelightDevice.GetProp(PROPERTIES.power);
            
            return powerProperty.ToString() == "on";
        }
    }
}