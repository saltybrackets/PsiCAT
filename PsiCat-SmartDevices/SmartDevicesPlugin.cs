namespace PsiCat.SmartDevices
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using PsiCat.Plugins;


    public class SmartDevicesPlugin : PsiCatPlugin
    {
        #region Properties
        public override string Author
        {
            get
            {
                return "Zachary White (Primitive Concept)";
            }
        }


        public SmartDevicesConfig Config { get; private set; } // TODO


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
        #endregion


        public override async void OnStart()
        {
            LoadConfig();

            SmartLights smartLights = new SmartLights();
            smartLights.Logger = this.Logger;
            
            await smartLights.LocateAll(this.Config);
            
            this.Config.Save(SmartDevicesConfig.DefaultFilePath);
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