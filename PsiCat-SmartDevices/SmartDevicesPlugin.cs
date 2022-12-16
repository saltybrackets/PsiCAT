namespace PsiCat.SmartDevices
{
    using System.Collections.Generic;
    using System.IO;
    using PsiCat.Plugins;


    public class SmartDevicesPlugin : PsiCatPlugin
    {
        private SmartDevicesConfig config;

        public override string Author
        {
            get
            {
                return "Wren White (Primitive Concept)";
            }
        }

        public override SmartDevicesConfig Config
        {
            get
            {
                return this.config;
            }
        }

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

        public SmartLights SmartLights { get; private set; }

        public override string Version
        {
            get { return "1.0.0"; }
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
                this.config = PsiCat.Config.LoadFromJson<SmartDevicesConfig>(path);
            }
            else
            {
                this.Logger.LogWarning($"Creating new config at: {path}");
                this.config = new SmartDevicesConfig();
                this.Config.Save(path);
            }
        }


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

            if (this.SmartLights.Lights.Count > 0)
                await this.SmartLights.ConnectToAll();
        }


        public override void OnUpdate()
        {
            // TODO
        }
    }
}