namespace PsiCat.Home;

using Microsoft.AspNetCore.Components;
using PsiCat.SmartDevices;


public partial class LightGroup : ComponentBase 
{
    [Parameter]
    public KeyValuePair<string, List<SmartDevice>> LightGroupConfig { get; set; }

    [Parameter]
    public bool Locked { get; set; } = false;

    public List<Yeelight> Yeelights { get; set; } = new List<Yeelight>();

    private Yeelight YeelightElement
    {
        set { this.Yeelights.Add(value); }
    }


    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            UpdateState();
        }
        
        base.OnAfterRender(firstRender);
    }


    private async void UpdateState()
    {
        foreach (Yeelight light in this.Yeelights)
        {
            if (light != null)
                await light.UpdateState();
        }
        await InvokeAsync(StateHasChanged);
    }
}