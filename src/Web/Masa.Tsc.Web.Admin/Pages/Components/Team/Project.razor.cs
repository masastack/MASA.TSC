// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Components;

public partial class Project
{
    [Parameter]
    public TeamDto Team { get; set; } = default!;

    [Parameter]
    public ProjectDto Value { get; set; } = default!;

    [Parameter]
    public string AppId { get; set; } = default!;
}
