namespace PsiCat.Home;

using Microsoft.AspNetCore.Components;
using PsiCat.SmartDevices;


public partial class Lights : ComponentBase
{
    private bool RefreshDisabled { get; set; } = false;
    
    private SmartDevicesConfig Config
    {
        get
        {
            return (SmartDevicesConfig)this.SmartDevices.Config;
        }
    }
    
    private LightGroup LightGroupElement
    {
        set { this.LightGroups.Add(value); }
    }

    private List<LightGroup> LightGroups { get; set; } = new List<LightGroup>();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            foreach (LightGroup lightGroup in this.LightGroups)
            {
                foreach (Yeelight light in lightGroup.Yeelights)
                {
                    await light.UpdateState();
                }
            }
            this.RefreshDisabled = false;
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }


    private async void Refresh()
    {
        this.SmartDevices.Logger.Log("Syncing with smart lights...");
        
        this.RefreshDisabled = true;

        foreach (LightGroup lightGroup in this.LightGroups)
        {
            foreach (Yeelight light in lightGroup.Yeelights)
            {
                light.IsDisabled = true;
            }
        }
        
        await InvokeAsync(StateHasChanged);
        
        await this.SmartDevices.SmartLights.LocateAll();

        if (this.SmartDevices.SmartLights.Lights.Count > 0)
        {
            await this.SmartDevices.SmartLights.ConnectToAll();
        
            foreach (LightGroup lightGroup in this.LightGroups)
            {
                foreach (Yeelight light in lightGroup.Yeelights)
                {
                    await light.UpdateState();
                }
            }    
        }

        this.RefreshDisabled = false;
        await InvokeAsync(StateHasChanged);
        
        this.SmartDevices.Logger.Log("Finished syncing with smart lights.");
    }


    private KeyValuePair<string, List<SmartDevice>> GetAllLights()
    {
        List<SmartDevice> lights = new List<SmartDevice>(); 
        foreach (SmartDevice light in this.Config.Devices
                     .Where(device => device.Type == SmartDeviceType.Light))
        {
            lights.Add(light);
        }

        return new KeyValuePair<string, List<SmartDevice>>("All", lights);
    }
}