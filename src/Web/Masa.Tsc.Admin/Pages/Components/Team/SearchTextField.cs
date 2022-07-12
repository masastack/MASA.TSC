﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using BlazorComponent.I18n;
using Masa.Blazor;
using Masa.Stack.Components.Models;
using Microsoft.AspNetCore.Components.Rendering;

namespace Masa.Tsc.Admin.Rcl.Pages.Components;

public class SearchTextField : MTextField<string>
{
    [Inject]
    public I18n? I18n { get; set; }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Flat = true;
        Dense = true;
        Solo = true;
        HideDetails = "auto";
        BackgroundColor = "fill-background";
        Style = "max-width:540px;";
        Placeholder = I18n!.T("Search");
        PrependInnerContent = builder =>
        {
            builder.OpenComponent<MIcon>(0);
            builder.AddAttribute(1, "Size", (StringNumber)16);
            builder.AddAttribute(2, "Class", "mr-2 emphasis2--text");
            builder.AddAttribute(3, "ChildContent", (RenderFragment)delegate (RenderTreeBuilder builder2) {
                builder2.AddContent(4, IconConstants.Search);
            });
            builder.CloseComponent();
        };

        await base.SetParametersAsync(parameters);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Class ??= "";
        //if (Class.Contains("rounded-2 search") is false)
        //    Class += " rounded-2 search";
    }
}