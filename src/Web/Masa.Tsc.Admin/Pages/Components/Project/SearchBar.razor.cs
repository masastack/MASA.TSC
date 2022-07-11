// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Pages.Components;

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

    private string TimeStr = "2022/07/01 09:00:00~2022/07/01 10:00:00";
}
