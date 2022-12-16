namespace PsiCat.SmartDevices
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;


    [Serializable]
    public class SmartDevicesConfig : Config
    {
        public static readonly string DefaultFilePath = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}"
                                                        + "/smart-devices-config.json";

        [DefaultValue(1000)]
        public int DeviceConnectionTimeout = 1000;

        public List<SmartDevice> Devices = new List<SmartDevice>();

        public Dictionary<string, SmartDeviceGroup> DeviceGroups = new Dictionary<string, SmartDeviceGroup>();


        public IEnumerable<SmartDeviceGroup> GetDeviceGroupsOfType(SmartDeviceType type)
        {
            return this.DeviceGroups
                .Values
                .Where(group => group.GroupType == type);
        }


        public override void Save(string filePath = null)
        {
            if (filePath == null)
                filePath = DefaultFilePath;
            base.Save(filePath);
        }


        internal void SetDefaultValuesAfterDeserialization(StreamingContext context)
        {
            if (this.DeviceGroups == null
                || !this.DeviceGroups.Any())
            {
                this.DeviceGroups = new Dictionary<string, SmartDeviceGroup>();
            }
        }
    }
}