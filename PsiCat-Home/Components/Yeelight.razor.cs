namespace PsiCat.Home;

using System.Drawing;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using PsiCat.SmartDevices;
using YeelightAPI;
using YeelightAPI.Models;


public partial class Yeelight : ComponentBase
{
    private SmartDevice? smartDevice;

    public int Brightness { get; private set; } = 100;
    public string Color { get; private set; } = "#FFF";

    public bool IsDisabled { get; set; } = true;
    public bool IsOn { get; private set; }

    [Parameter]
    public SmartDevice? SmartDevice
    {
        get { return this.smartDevice; }
        set
        {
            this.smartDevice = value;
        }
    }

    private YeelightDevice? light
    {
        get { return GetLight(); }
    }

    private SmartLights SmartLights
    {
        get { return this.SmartDevices.SmartLights; }
    }


    public async Task Toggle()
    {
        YeelightDevice? yeelightDevice = this.light;
        if (yeelightDevice == null)
            return;
        await yeelightDevice.Toggle();
    }


    public async Task UpdateBrightness(ChangeEventArgs args)
    {
        if (this.light == null
            || args.Value == null)
        {
            return;
        }

        this.Brightness = int.Parse(args.Value!.ToString()!);
        await this.light.SetBrightness(this.Brightness);
    }


    public async Task UpdateColor(ChangeEventArgs args)
    {
        if (this.light == null
            || args.Value == null)
        {
            return;
        }
            
        Color color = ColorTranslator.FromHtml(args.Value.ToString() ?? "#000");
        
        int hue = Math.Clamp((int)(color.GetHue()), 1, 359);
        int saturation = Math.Clamp((int)(color.GetRealSaturation() * 100f), 0, 100);
        int brightness = Math.Clamp((int)(color.GetValue() * 100f), 0, 100);
        
        if (brightness > 0)
        {
            await this.light.TurnOn();
            await this.light.SetHSVColor(hue, saturation);
        }
        else
        {
            await this.light.TurnOff();
        }
    }


    public async Task UpdateState()
    {
        if (this.light == null)
        {
            this.IsDisabled = true;
            return;
        }
            
        this.IsDisabled = !this.light.IsConnected;
        if (this.IsDisabled)
            return;

        var lightProps = await this.light.GetAllProps();
        this.IsOn = lightProps.IsOn();
        this.Color = ColorTranslator.ToHtml(lightProps.GetColor());
        this.Brightness = lightProps.GetBrightness();
    }


    private YeelightDevice? GetLight()
    {
        if (this.SmartDevice == null)
            return null;
        
        return !this.SmartLights.Lights.ContainsKey(this.SmartDevice.IP) 
                   ? null 
                   : this.SmartLights.Lights[this.SmartDevice.IP];
    }
    
    private async void GetDetails()
    {
        
    }
}