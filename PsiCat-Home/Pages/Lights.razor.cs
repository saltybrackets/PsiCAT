namespace PsiCat.Home;

using Microsoft.AspNetCore.Components;
using PsiCat.SmartDevices;


public partial class Lights : ComponentBase
{
    private SmartDevicesConfig Config
    {
        get { return (SmartDevicesConfig)this.SmartDevices.Config; }
    }

    private bool RefreshDisabled { get; set; } = true;

    private Yeelight yeelightElement
    {
        set { this.Yeelights.Add(value); }
    }

    private List<Yeelight> Yeelights { get; set; } = new List<Yeelight>();


    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            UpdateState();
        }
        
        base.OnAfterRender(firstRender);
    }


    private async void Refresh()
    {
        foreach (Yeelight light in this.Yeelights)
        {
            if (light != null)
                light.IsDisabled = true;
        }
        this.RefreshDisabled = true;
        await InvokeAsync(StateHasChanged);
        
        await this.SmartDevices.SmartLights.LocateAll();
        UpdateState();
    }


    private async void UpdateState()
    {
        foreach (Yeelight light in this.Yeelights)
        {
            if (light != null)
                await light.UpdateState();
        }
        this.RefreshDisabled = false;
        await InvokeAsync(StateHasChanged);
    }
}