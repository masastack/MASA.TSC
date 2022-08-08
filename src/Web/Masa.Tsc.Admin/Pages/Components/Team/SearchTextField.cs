// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Pages.Components;

public class SearchTextField : MTextField<string>
{
    [Inject]
    public I18n? I18n { get; set; }

    [Parameter]
    public string IconClass { get; set; } = "mt-1 mr-1 emphasis2--text";

    [Parameter]
    public int IconSize { get; set; } = 16;

    private string _defaultClass = "search";

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        HideDetails = "auto";
        BackgroundColor = "fill-background";
        Style = "max-width:540px;";
        Placeholder = I18n!.T("Search");
        PrependInnerContent = builder =>
        {
            builder.OpenComponent<MIcon>(0);
            builder.AddAttribute(1, "Size", (StringNumber)IconSize);
            builder.AddAttribute(2, "Class", IconClass);
            builder.AddAttribute(3, "ChildContent", (RenderFragment)delegate (RenderTreeBuilder builder2)
            {
                builder2.AddContent(4, IconConstants.Search);
            });
            builder.CloseComponent();
        };
        await base.SetParametersAsync(parameters);
    }    

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (string.IsNullOrEmpty(Class))
        {
            Class = _defaultClass;
        }
        else if (!Class.Contains("search"))
        {
            Class = $"{Class} {_defaultClass}";
        }
    }
}
