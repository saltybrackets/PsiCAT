namespace PsiCat.Home;

using Microsoft.AspNetCore.Components;


public partial class TabControl : ComponentBase
{
    public TabPage ActivePage { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    private List<TabPage> Pages { get; } = new List<TabPage>();


    private void ActivatePage(TabPage tabPage)
    {
        this.ActivePage = tabPage;
    }


    private string GetButtonClass(TabPage page)
    {
        return page == this.ActivePage
                   ? "btn-primary"
                   : "btn-secondary";
    }


    internal void AddPage(TabPage tabPage)
    {
        this.Pages.Add(tabPage);
        if (this.Pages.Count == 1)
            this.ActivePage = tabPage;
        StateHasChanged();
    }
}