namespace PsiCat.Home;

using System.Timers;
using Microsoft.AspNetCore.Components;


public partial class TextConsole : ComponentBase,
                                   IDisposable
{
    [Parameter]
    public IConsoleContents Contents { get; set; }

    [Parameter]
    public float UpdateInterval { get; set; } = 0.25f;

    private ElementReference textAreaReference;
    
    private Timer timer;


    public void Dispose()
    {
        this.timer?.Dispose();
    }


    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            this.timer = new Timer();
            this.timer.Interval = this.UpdateInterval * 1000f;
            this.timer.AutoReset = true;
            this.timer.Elapsed += RefreshContents;
            this.timer.Start();    
        }
    }


    private async void RefreshContents(object? sender, ElapsedEventArgs args)
    {
        await InvokeAsync(StateHasChanged);
        await this.JS.InvokeAsync<object?>("scrollToEnd", new object?[] { this.textAreaReference });
    }
}