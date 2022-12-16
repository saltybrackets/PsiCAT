namespace PsiCat.Home;

using Microsoft.AspNetCore.Components;
using PsiCat.SmartDevices;


public partial class LightGroup : ComponentBase 
{
    [Parameter]
    public SmartDeviceGroup LightGroupConfig { get; set; }

    public List<Light> Lights { get; set; } = new List<Light>();

    [Parameter]
    public bool Locked { get; set; } = false;

    private Light GroupLightElement { get; set; }

    private Light LightElement
    {
        set { this.Lights.Add(value); }
    }

    private SmartLightGroup SmartLightGroup { get; set; } = new SmartLightGroup();

    private SmartLights SmartLights
    {
        get { return this.SmartDevices.SmartLights; }
    }


    public ISmartLight GetLight(string deviceIP)
    {
        if (string.IsNullOrEmpty(deviceIP))
            return null;
        
        return (!this.SmartLights.Lights.ContainsKey(deviceIP) 
                    ? null 
                    : this.SmartLights.Lights[deviceIP]);
    }


    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            foreach (Light light in this.Lights)
            {
                this.SmartLightGroup.Add(light.SmartLight);
            }
            UpdateState();
        }
        
        base.OnAfterRender(firstRender);
    }


    private async void UpdateState()
    {
        await this.GroupLightElement.UpdateState();
        foreach (Light light in this.Lights)
        {
            if (light != null)
                await light.UpdateState();
        }
        await InvokeAsync(StateHasChanged);
    }
}