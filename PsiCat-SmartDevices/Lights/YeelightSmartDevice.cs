namespace YeelightAPI
{
    using PsiCat.SmartDevices;


    partial class Device:
        ISmartDevice
    {
        public string IP { get; set; }

        public bool IsAvailable
        {
            get { return this.IsConnected; }
        }
    }
}