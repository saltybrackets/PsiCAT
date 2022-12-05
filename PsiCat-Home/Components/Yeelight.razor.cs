namespace PsiCat.Home;

using System.Drawing;
using Microsoft.AspNetCore.Components;
using PsiCat.SmartDevices;
using YeelightAPI;


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
        this.IsDisabled = this.light == null
                     || !this.light.IsConnected;

        if (this.IsDisabled)
            return;
                                       
        this.IsOn = await this.light.IsOn();

        this.Color = ColorTranslator.ToHtml(
            await this.light.GetColor());

        this.Brightness = await this.light.GetBrightness();
    }


    private YeelightDevice? GetLight()
    {
        if (this.SmartDevice == null)
            return null;
        
        return !this.SmartLights.Lights.ContainsKey(this.SmartDevice.IP) 
                   ? null 
                   : this.SmartLights.Lights[this.SmartDevice.IP];
    }
}