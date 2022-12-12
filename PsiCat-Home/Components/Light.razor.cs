namespace PsiCat.Home;

using System.Drawing;
using System.Text;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using PsiCat.SmartDevices;
using YeelightAPI;
using YeelightAPI.Models;


public partial class Light : ComponentBase
{
    private Popup popup;

    public int Brightness { get; private set; } = 100;
    public string Color { get; private set; } = "#FFF";

    public bool IsDisabled { get; set; } = true;
    public bool IsOn { get; private set; }

    [Parameter]
    public ISmartLight SmartLight { get; set; }


    public async Task Toggle()
    {
        ISmartLight smartLight = this.SmartLight;
        if (smartLight == null)
            return;
        await smartLight.Toggle();
    }


    public async Task UpdateBrightness(ChangeEventArgs args)
    {
        if (this.SmartLight == null
            || args.Value == null)
        {
            return;
        }

        this.Brightness = int.Parse(args.Value!.ToString()!);
        await this.SmartLight.SetBrightness(this.Brightness);
    }


    public async Task UpdateColor(ChangeEventArgs args)
    {
        if (this.SmartLight == null
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
            await this.SmartLight.SetColor(hue, saturation);
        }
        else
        {
            await this.SmartLight.TurnOff();
        }
    }


    public async Task UpdateState()
    {
        if (this.SmartLight == null)
        {
            this.IsDisabled = true;
            return;
        }
            
        this.IsDisabled = !(await this.SmartLight.IsConnected());
        if (this.IsDisabled)
            return;

        await this.SmartLight.Sync();
        this.IsOn = await this.SmartLight.IsOn();
        this.Color = ColorTranslator.ToHtml(await this.SmartLight.GetColor());
        this.Brightness = await this.SmartLight.GetBrightness();
    }


    private async void GetDetails()
    {
        SmartLightDetails details = await this.SmartLight.GetDetails();
        StringBuilder bodyText = new StringBuilder()
            .AppendLine($"**Name:** {details.Name}")
            .AppendLine($"**Model:** {details.Model}")
            .AppendLine($"**IP:** {details.IP}")
            .AppendLine($"**Port:** {details.Port}")
            .AppendLine($"**Other:** ```{details.Other.ToString()}```");

        this.popup.Show(bodyText.ToString(), $"Smart Light Details");
    }
}