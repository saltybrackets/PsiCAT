namespace PsiCat.SmartDevices
{
    using System;
    using System.Collections.Generic;


    [Serializable]
    public class SmartDeviceGroup : List<string>
    {
        public string Name = "New Device Group";
        public SmartDeviceType GroupType;
    }
}