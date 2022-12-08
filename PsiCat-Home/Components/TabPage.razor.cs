namespace PsiCat.Home;

using Microsoft.AspNetCore.Components;


public partial class TabPage : ComponentBase
{
    [CascadingParameter]
    private TabControl Parent { get; set; }
    
    [Parameter]
    public RenderFragment ChildContent { get; set; }
    
    [Parameter]
    public string Label { get; set; }

    protected override void OnInitialized()
    {
        if (this.Parent == null)
        {
            throw new ArgumentNullException(
                nameof(this.Parent),
                "TabPage must exist within a TabControl");
        }

        this.Parent.AddPage(this);
            
        base.OnInitialized();
    }
}