namespace PsiCat.SmartDevices
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;


    public class SmartDevicesConfig : Config
    {
        public static readonly string DefaultFilePath = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}"
                                                        + "/smart-devices-config.json";

        public List<SmartDevice> Devices = new List<SmartDevice>();
    }
}