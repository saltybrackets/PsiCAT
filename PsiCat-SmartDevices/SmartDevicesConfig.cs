namespace PsiCat.SmartDevices
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;


    [Serializable]
    public class SmartDevicesConfig : Config
    {
        public static readonly string DefaultFilePath = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}"
                                                        + "/smart-devices-config.json";

        public bool UseAllAvailableMulticastAddresses = false;
        
        [DefaultValue(500)]
        public int DeviceConnectionTimeout = 500;
        
        public List<SmartDevice> Devices = new List<SmartDevice>();

        public Dictionary<string, List<SmartDevice>> SmartLightGroups = 
            new Dictionary<string, List<SmartDevice>>();

        public override void Save(string filePath = null)
        {
            if (filePath == null)
                filePath = DefaultFilePath;
            base.Save(filePath);
        }
    }
}