namespace PsiCat.SmartDevices
{
    using System;
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


        public Config Config { get; private set; } // TODO


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


        public override void OnStart()
        {
            this.Config = null;

            Devices devices = new Devices();
            devices.Logger = this.Logger;
            
            Task locateTask = devices.LocateAll();
        }


        public override void OnUpdate()
        {
            // TODO
        }
    }
}