namespace PsiCat.Home;

using Microsoft.AspNetCore.Components;
using Markdig;


public partial class Popup : ComponentBase
{
    [Parameter]
    public bool IsVisible { get; set; }
    
    [Parameter]
    public EventCallback<bool> IsVisibleChanged { get; set; }
    
    [Parameter]
    public MarkupString BodyText { get; set; }
    
    [Parameter]
    public string HeaderText { get; set; }
    

    public void Show(string bodyText, string headerText = "")
    {
        this.BodyText = (MarkupString)Markdown.ToHtml(bodyText);
        this.HeaderText = headerText;
        this.IsVisible = true;
        StateHasChanged();
    }


    private void Close()
    {
        this.BodyText = (MarkupString)string.Empty;
        this.HeaderText = string.Empty;
        this.IsVisible = false;
        StateHasChanged();
    }
}