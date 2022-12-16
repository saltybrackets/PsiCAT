namespace PsiCat.Home;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using PsiCat.SmartDevices;


public partial class SmartDeviceSelector : ComponentBase
{
    private List<SmartDevice> smartDevices = new List<SmartDevice>();

    [Parameter, EditorRequired]
    public SmartDeviceType DeviceType { get; set; }

    [Parameter]
    public string SelectedDeviceIP { get; set; }

    private SmartDevicesConfig Config
    {
        get
        {
            return (SmartDevicesConfig)this.SmartDevices.Config;
        }
    }


    protected override void OnInitialized()
    {
        // Populate device list.
        this.smartDevices.Clear();
        foreach (SmartDevice device in this.Config.Devices
                     .Where(device => device.Type == this.DeviceType))
        {
            this.smartDevices.Add(device);
        }

        if (this.smartDevices.Count > 0)
            this.SelectedDeviceIP = this.smartDevices[0].IP;
    }
}