namespace PsiCat.SmartDevices
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using PsiCat.Plugins;


    public class SmartDevicesPlugin : PsiCatPlugin
    {
        
        public override string Author
        {
            get
            {
                return "Wren White (Primitive Concept)";
            }
        }


        public override Config Config { get; set; } // TODO


        public override string Description
        {
            get
            {
                return "Manages access to various smart devices.";
            }
        }


        public override string Name
        {
            get { return "PsiCAT Smart Devices"; }
        }


        public override PluginHost PluginHost { get; set; }


        public override string Version
        {
            get { return "1.0.0"; }
        }
        
        public SmartLights SmartLights { get; private set; }

        public override async void OnStart()
        {
            LoadConfig();

            SmartDevicesConfig config = (SmartDevicesConfig)this.Config;
            
            this.SmartLights = 
                new SmartLights((SmartDevicesConfig)this.Config)
                    {
                        Logger = this.Logger
                    };

            IEnumerable<ISmartLight> smartLights = await this.SmartLights.LocateAll();
            
            foreach (ISmartLight smartLight in smartLights)
            {
                smartLight.ApplyToConfig(config);
            }
            config.Save(SmartDevicesConfig.DefaultFilePath);

            await this.SmartLights.ConnectToAll();
        }


        public override void OnUpdate()
        {
            // TODO
        }
        
        
        /// <summary>
        /// Load in main config file.
        /// If no config file found, a new one will be created.
        /// </summary>
        public void LoadConfig(string path = null)
        {
            if (string.IsNullOrEmpty(path))
                path = SmartDevicesConfig.DefaultFilePath;
            
            if (File.Exists(path))
            {
                this.Config = PsiCat.Config.LoadFromJson<SmartDevicesConfig>(path);
            }
            else
            {
                this.Logger.LogWarning($"Creating new config at: {path}");
                this.Config = new SmartDevicesConfig();
                this.Config.Save(path);
            }
        }
    }
}