// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Components;

public partial class SearchBar
{
    [Parameter]
    public List<AppDto> Apps { get; set; } = new List<AppDto>
    {
        new AppDto
        {
            Name="aaa",
            Identity="111"
        }
    };

    [Parameter]
    public string AppId { get; set; }

    private List<string> _selectDataSource = new List<string>
    {
        "15分钟",
        "30分钟",
        "1小时",
        "5小时"
    };

    public DateTime? Start { get; set; }

    public DateTime? End { get; set; }

    private string sasas { get; set; }

    private string _searchIconClass= "fas fa-rotate";

    private async Task SearchAsync()
    {
        _searchIconClass = "fas fa-circle-notch fa-spin";
        StateHasChanged();
        Thread.Sleep(500);
        _searchIconClass = "fas fa-rotate";
        StateHasChanged();
        await Task.CompletedTask;
    }
}
