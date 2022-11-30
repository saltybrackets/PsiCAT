namespace PsiCat.SmartDevices
{
    using System;
    using Newtonsoft.Json;


    [Serializable]
    public class SmartDevice
    {
        public virtual string Name { get; set; }
        public virtual string IP { get; set; }
    }
}