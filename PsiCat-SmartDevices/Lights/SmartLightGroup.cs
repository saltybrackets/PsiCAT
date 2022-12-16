namespace PsiCat.SmartDevices
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading.Tasks;


    /// <summary>
    /// Collection of smart lights that may be operated as a single unit.
    /// </summary>
    public class SmartLightGroup : List<ISmartLight>,
                                   ISmartLight

    {
        public string IP
        {
            get { return string.Empty; }
        }

        public string Name { get; set; }

        public SmartDevice SmartDevice
        {
            get
            {
                return new SmartDevice
                           {
                               Name = this.Name,
                               Type = SmartDeviceType.Light,
                               IP = string.Empty
                           };
            }
        }


        public void ApplyToConfig(SmartDevicesConfig config)
        {
            // TODO: Add to config's dictionary
        }


        public async Task<bool> Connect()
        {
            return await ExecuteInParallel(light => light.Connect(), false);
        }


        public async Task<bool> Disconnect()
        {
            return await ExecuteInParallel(light => light.Disconnect());
        }


        public async Task<int> GetBrightness()
        {
            List<Task<int>> results = await GetInParallel(light => light.GetBrightness());
            int total = results.Sum(result => result.Result);
            return total / results.Count;
        }


        public async Task<Color> GetColor()
        {
            List<Task<Color>> results = await GetInParallel(light => light.GetColor());
            Color color = results[0].Result;
            for (int i = 1; i < results.Count; i++)
            {
                if (results[i].Result != color)
                    return Color.Transparent;
            }

            return color;
        }


        public Task<SmartLightDetails> GetDetails()
        {
            return Task.FromResult(
                new SmartLightDetails
                    {
                        IP = this.IP,
                        Model = "Light Group",
                        Name = "TODO: Light Group Name",
                        Port = 0,
                        Other = string.Empty
                    });
        }


        public async Task<bool> IsConnected()
        {
            List<Task<bool>> results = await GetInParallel(light => light.IsConnected());
            foreach (Task<bool> result in results)
            {
                if (result.Result == true)
                    return true;
            }

            return false;
        }


        public async Task<bool> IsOn()
        {
            List<Task<bool>> results = await GetInParallel(light => light.IsOn());
            foreach (Task<bool> result in results)
            {
                if (result.Result == true)
                    return true;
            }

            return false;
        }


        public async Task<bool> SetBrightness(int brightness)
        {
            return await ExecuteInParallel(light => light.SetBrightness(brightness));
        }


        public async Task<bool> SetColor(int hue, int saturation)
        {
            return await ExecuteInParallel(light => light.SetColor(hue, saturation));
        }


        public Task<bool> Sync()
        {
            return Task.FromResult(true);
        }


        public async Task<bool> Toggle()
        {
            if (await IsOn())
                return await TurnOff();
            else
                return await TurnOn();
        }


        public SmartDevice ToSmartDevice()
        {
            SmartDevice smartDevice = new SmartDevice();
            smartDevice.Name = $"TODO: Light Group Name";
            smartDevice.IP = "TODO: Host IP";
            smartDevice.Type = SmartDeviceType.Light;

            return smartDevice;
        }


        public async Task<bool> TurnOff()
        {
            return await ExecuteInParallel(light => light.TurnOff());
        }


        public async Task<bool> TurnOn()
        {
            return await ExecuteInParallel(light => light.TurnOn());
        }


        private async Task<bool> ExecuteInParallel(
            Func<ISmartLight, Task<bool>> function, bool mustBeConnected = true)
        {
            bool result = true;
            List<Task<bool>> tasks = new List<Task<bool>>();
            
            foreach (ISmartLight light in this)
            {
                if (light == null)
                    continue;
                
                if (mustBeConnected)
                {
                    if (!await light.IsConnected())
                        continue;
                }
                tasks.Add(function(light));
            }
            
            await Task.WhenAll(tasks);
            
            foreach (Task<bool> task in tasks)
            {
                result &= task.Result;
            }

            return result;
        }


        private async Task<List<Task<T>>> GetInParallel<T>(Func<ISmartLight, Task<T>> getter)
        {
            List<Task<T>> tasks = new List<Task<T>>();
            foreach (ISmartLight light in this)
            {
                if (light == null
                    || !await light.IsConnected())
                {
                    continue;
                }
                tasks.Add(getter(light));
            }

            await Task.WhenAll(tasks);

            return tasks;
        }
    }
}