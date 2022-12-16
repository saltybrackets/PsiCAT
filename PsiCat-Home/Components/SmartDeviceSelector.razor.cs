namespace PsiCat.Home;

using Microsoft.AspNetCore.Components;
using PsiCat.SmartDevices;


public partial class SmartDeviceSelector : ComponentBase
{
    private List<SmartDevice> smartDevices = new List<SmartDevice>();

    [Parameter, EditorRequired]
    public SmartDeviceType DeviceType { get; set; }

    [Parameter]
    public string SelectedDeviceIP { get; set; }

    protected override void OnInitialized()
    {
        // Populate device list.
        this.smartDevices.Clear();
        foreach (SmartDevice device in this.SmartDevices.Config.Devices
                     .Where(device => device.Type == this.DeviceType))
        {
            this.smartDevices.Add(device);
        }

        if (this.smartDevices.Count > 0)
            this.SelectedDeviceIP = this.smartDevices[0].IP;
    }
}