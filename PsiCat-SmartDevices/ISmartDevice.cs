namespace PsiCat.SmartDevices
{
    public interface ISmartDevice
    {
        string Id { get; }
        string IP { get; }
        bool IsAvailable { get; }
    }
}