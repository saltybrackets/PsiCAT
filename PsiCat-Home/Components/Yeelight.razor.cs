namespace PsiCat.Home;

using System.Drawing;
using System.Text;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using PsiCat.SmartDevices;
using YeelightAPI;
using YeelightAPI.Models;


public partial class Yeelight : ComponentBase
{
    private SmartDevice smartDevice;

    private Popup popup;

    public int Brightness { get; private set; } = 100;
    public string Color { get; private set; } = "#FFF";

    public bool IsDisabled { get; set; } = true;
    public bool IsOn { get; private set; }

    [Parameter]
    public SmartDevice SmartDevice
    {
        get { return this.smartDevice; }
        set
        {
            this.smartDevice = value;
        }
    }

    private ISmartLight Light
    {
        get { return GetLight(); }
    }

    private SmartLights SmartLights
    {
        get { return this.SmartDevices.SmartLights; }
    }


    public async Task Toggle()
    {
        ISmartLight smartLight = this.Light;
        if (smartLight == null)
            return;
        await smartLight.Toggle();
    }


    public async Task UpdateBrightness(ChangeEventArgs args)
    {
        if (this.Light == null
            || args.Value == null)
        {
            return;
        }

        this.Brightness = int.Parse(args.Value!.ToString()!);
        await this.Light.SetBrightness(this.Brightness);
    }


    public async Task UpdateColor(ChangeEventArgs args)
    {
        if (this.Light == null
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
            await this.Light.SetColor(hue, saturation);
        }
        else
        {
            await this.Light.TurnOff();
        }
    }


    public async Task UpdateState()
    {
        if (this.Light == null)
        {
            this.IsDisabled = true;
            return;
        }
            
        this.IsDisabled = !(await this.Light.IsConnected());
        if (this.IsDisabled)
            return;

        await this.Light.Sync();
        this.IsOn = await this.Light.IsOn();
        this.Color = ColorTranslator.ToHtml(await this.Light.GetColor());
        this.Brightness = await this.Light.GetBrightness();
    }


    private async void GetDetails()
    {
        SmartLightDetails details = await this.Light.GetDetails();
        StringBuilder bodyText = new StringBuilder()
            .AppendLine($"**Name:** {details.Name}")
            .AppendLine($"**Model:** {details.Model}")
            .AppendLine($"**IP:** {details.IP}")
            .AppendLine($"**Port:** {details.Port}")
            .AppendLine($"**Other:** ```{details.Other.ToString()}```");

        this.popup.Show(bodyText.ToString(), $"Smart Light Details");
    }


    private ISmartLight GetLight()
    {
        if (this.SmartDevice == null)
            return null;
        
        return (!this.SmartLights.Lights.ContainsKey(this.SmartDevice.IP) 
                    ? null 
                    : this.SmartLights.Lights[this.SmartDevice.IP]);
    }
}