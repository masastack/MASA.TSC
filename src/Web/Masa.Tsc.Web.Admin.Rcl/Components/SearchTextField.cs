// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public class SearchTextField : STextField<string>
{
    [Parameter]
    public bool FillBackground { get; set; } = true;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Flat = true;
        Solo = true;
        Small = true;
        BackgroundColor = FillBackground ? "fill-background" : "white";
        Style = "max-width:340px;";
        Placeholder = I18n!.T("Search");
        PrependInnerContent = builder =>
        {
            builder.OpenComponent<SIcon>(0);
            builder.AddAttribute(1, "Size", (StringNumber)16);
            builder.AddAttribute(2, "Class", "mr-2 emphasis2--text");
            builder.AddAttribute(3, "ChildContent", (RenderFragment)delegate (RenderTreeBuilder builder2)
            {
                builder2.AddContent(4, IconConstants.Search);
            });
            builder.CloseComponent();
        };

        await base.SetParametersAsync(parameters);
        DebounceInterval = 500;
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        BackgroundColor = FillBackground ? "fill-background" : "white";
        Class ??= "";
        if (Class.Contains("rounded-2 search") is false)
            Class += " rounded-2 search";
    }
}
