// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Projects;

public partial class ProjectTabs
{
    [Parameter]
    public string Value { get; set; }

    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }

    List<KeyValuePair<string, string>> ProjectTypes { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var data = await ApiCaller.ProjectService.GetProjectTypesAsync();
        if (data != null)
        {
            ProjectTypes.Clear();
            ProjectTypes.Add(new("", "All"));
            ProjectTypes.AddRange(data);
        }
    }
}
