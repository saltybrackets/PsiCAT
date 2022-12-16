namespace PsiCat.Home;

using Microsoft.AspNetCore.Components;
using PsiCat.SmartDevices;


public partial class Lights : ComponentBase
{
    private bool RefreshDisabled { get; set; } = false;
    
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
                foreach (Light light in lightGroup.Lights)
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
            foreach (Light light in lightGroup.Lights)
            {
                light.IsDisabled = true;
            }
        }
        
        await InvokeAsync(StateHasChanged);
        
        this.SmartDevices.SmartLights.DisconnectAll();
        await this.SmartDevices.SmartLights.LocateAll();

        if (this.SmartDevices.SmartLights.Lights.Count > 0)
        {
            await this.SmartDevices.SmartLights.ConnectToAll();
        
            foreach (LightGroup lightGroup in this.LightGroups)
            {
                foreach (Light light in lightGroup.Lights)
                {
                    await light.UpdateState();
                }
            }    
        }

        this.RefreshDisabled = false;
        await InvokeAsync(StateHasChanged);
        
        this.SmartDevices.Logger.Log("Finished syncing with smart lights.");
    }


    private SmartDeviceGroup GetAllLights()
    {
        SmartDeviceGroup allLightsGroup = new SmartDeviceGroup();
        allLightsGroup.Name = "All";
        allLightsGroup.GroupType = SmartDeviceType.Light;
        foreach (SmartDevice light in this.SmartDevices.Config.Devices
                     .Where(device => device.Type == SmartDeviceType.Light))
        {
            allLightsGroup.Add(light.IP);
        }

        return allLightsGroup;
    }
}