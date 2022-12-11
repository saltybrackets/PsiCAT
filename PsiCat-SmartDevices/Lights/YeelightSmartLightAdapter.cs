namespace PsiCat.SmartDevices
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using YeelightAPI;
    using YeelightAPI.Models;


    public class YeelightSmartLightAdapter : ISmartLight
    {
        private readonly YeelightDevice yeelightDevice;
        private Dictionary<PROPERTIES, object> yeelightProps;


        public YeelightSmartLightAdapter(YeelightDevice yeelightDevice)
        {
            this.yeelightDevice = yeelightDevice;
        }


        public string IP
        {
            get { return this.yeelightDevice.Hostname; }
        }


        public void ApplyToConfig(SmartDevicesConfig config)
        {
            if (config == null)
                return;
            
            if (config.Devices == null)
                config.Devices = new List<SmartDevice>();
            
            SmartDevice configEntry = config.Devices.FirstOrDefault(
                entry => entry.IP == this.yeelightDevice.Hostname);
            if (configEntry == null)
            {
                config.Devices.Add(ToSmartDevice());
            }
        }


        public async Task<bool> Connect()
        {
            return await this.yeelightDevice.Connect();
        }


        public Task<bool> Disconnect()
        {
            if (!this.yeelightDevice.IsConnected)
                return Task.FromResult(false);
            
            this.yeelightDevice.Disconnect();
            return Task.FromResult(true);
        }


        public async Task<int> GetBrightness()
        {
            if (this.yeelightProps != null)
                return int.Parse(this.yeelightProps[PROPERTIES.bright].ToString() ?? "0");
            
            if (!this.yeelightDevice.IsConnected)
                return 0;
            if (await IsOn() == false)
                return 0;
            
            object brightnessProperty = await this.yeelightDevice.GetProp(PROPERTIES.bright);
            return brightnessProperty != null 
                       ? int.Parse(brightnessProperty.ToString() ?? "0") 
                       : 0;
        }


        public async Task<Color> GetColor()
        {
            int colorValue;
            
            if (this.yeelightProps != null)
            {
                colorValue = int.Parse(this.yeelightProps[PROPERTIES.rgb].ToString() ?? "0");
                return Color.FromArgb(colorValue);
            }
            
            if (!this.yeelightDevice.IsConnected)
                return Color.Black;
            if (await IsOn() == false)
                return Color.Black;
            
            object colorProperty = await this.yeelightDevice.GetProp(PROPERTIES.rgb);
            if (colorProperty == null)
                return Color.Black;
            
            colorValue = int.Parse(colorProperty.ToString() ?? "0");

            return Color.FromArgb(colorValue);
        }


        public Task<SmartLightDetails> GetDetails()
        {
            return Task.FromResult(
                new SmartLightDetails
                    {
                        IP = this.IP,
                        Model = this.yeelightDevice.Model.ToString(),
                        Name = this.yeelightDevice.Name,
                        Port = this.yeelightDevice.Port,
                        Other = JsonConvert.SerializeObject(this.yeelightProps, Formatting.Indented) 
                    });
        }


        public Task<bool> IsConnected()
        {
            return Task.FromResult(this.yeelightDevice.IsConnected);
        }


        public async Task<bool> IsOn()
        {
            if (this.yeelightProps != null)
                return this.yeelightProps[PROPERTIES.power].ToString() == "on";
            
            if (!this.yeelightDevice.IsConnected)
                return false;
            object powerProperty = await this.yeelightDevice.GetProp(PROPERTIES.power);
            
            return powerProperty.ToString() == "on";
        }


        public async Task<bool> SetBrightness(int brightness)
        {
            return await this.yeelightDevice.SetBrightness(brightness);
        }


        public async Task<bool> SetColor(int hue, int saturation)
        {
            return await this.yeelightDevice.SetHSVColor(hue, saturation);
        }


        public async Task<bool> Sync()
        {
            this.yeelightProps = await this.yeelightDevice.GetAllProps();
            return this.yeelightProps != null
                   && this.yeelightProps.Count > 0;
        }


        public async Task<bool> Toggle()
        {
            return await this.yeelightDevice.Toggle();
        }


        public SmartDevice ToSmartDevice()
        {
            SmartDevice smartDevice = new SmartDevice();
            smartDevice.Name = $"{this.yeelightDevice.Model} Yeelight";
            smartDevice.IP = this.yeelightDevice.Hostname;
            smartDevice.Type = SmartDeviceType.Light;

            return smartDevice;
        }


        public async Task<bool> TurnOff()
        {
            return await this.yeelightDevice.TurnOff();
        }


        public async Task<bool> TurnOn()
        {
            return await this.yeelightDevice.TurnOn();
        }
    }
}