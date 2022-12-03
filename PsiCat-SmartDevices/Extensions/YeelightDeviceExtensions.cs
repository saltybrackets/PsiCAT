namespace PsiCat.SmartDevices
{
    using System.Collections.Generic;
    using System.Drawing;
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


        public static async Task<int> GetBrightness(this YeelightDevice yeelightDevice)
        {
            if (!yeelightDevice.IsConnected)
                return 0;
            if (await yeelightDevice.IsOn() == false)
                return 0;
            
            object brightnessProperty = await yeelightDevice.GetProp(PROPERTIES.bright);
            return brightnessProperty != null 
                       ? int.Parse(brightnessProperty.ToString() ?? "0") 
                       : 0;
        }


        public static async Task<Color> GetColor(this YeelightDevice yeelightDevice)
        {
            if (!yeelightDevice.IsConnected)
                return Color.Black;
            if (await yeelightDevice.IsOn() == false)
                return Color.Black;
            
            object colorProperty = await yeelightDevice.GetProp(PROPERTIES.rgb);
            if (colorProperty == null)
                return Color.Black;
            
            int colorValue = int.Parse(colorProperty.ToString() ?? "0");

            return Color.FromArgb(colorValue);
        }
    }
}