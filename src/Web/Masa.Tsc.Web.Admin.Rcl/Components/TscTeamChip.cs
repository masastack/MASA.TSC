// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public class TscTeamChip : MChip
{
    [Parameter]
    public string ContentClass { get; set; } = default!;

    protected override void SetComponentClass()
    {
        base.SetComponentClass();
        base.CssProvider.Merge("content", delegate (CssBuilder cssBuilder)
        {
            cssBuilder.Add($"{ContentClass}");
        });
    }

}
