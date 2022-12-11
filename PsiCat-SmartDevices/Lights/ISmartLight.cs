namespace PsiCat.SmartDevices
{
    using System.Drawing;
    using System.Threading.Tasks;
 
    public interface ISmartLight
    {
        string IP { get; }

        void ApplyToConfig(SmartDevicesConfig config);

        Task<bool> Connect();
        Task<bool> Disconnect();
        Task<int> GetBrightness();
        Task<Color> GetColor();
        Task<SmartLightDetails> GetDetails();
        Task<bool> IsConnected();
        Task<bool> IsOn();
        Task<bool> SetBrightness(int brightness);
        Task<bool> SetColor(int hue, int saturation);
        Task<bool> Sync();
        Task<bool> Toggle();
        SmartDevice ToSmartDevice();
        Task<bool> TurnOff();
        Task<bool> TurnOn();
    }
}