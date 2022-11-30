namespace PsiCat.SmartDevices
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;


    [Serializable]
    public class SmartDevice
    {
        public string Name { get; set; }
        public string IP { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public SmartDeviceType Type { get; set; }
    }
}