// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Pages.Components;

public partial class TscTraceChart
{
    private object _options;

    private void ConvertOption()
    {
        _options = new
        {
            Legend = new
            {
                Data = new[] { "Span个数", "延迟时间" }
            },
            XAxis = new
            {
                Data = new[] { "7.1", "7.1", "7.3", "7.4", "7.5", "7.6" }
            },
            YAxis = new { },
            Series = new[]
            {
                new
                {
                    Name= "Span个数",
                    Type= "bar",
                    Data= new []{ 5, 20, 3600, 100000, 10, 20 }
                },
                new
                {
                    Name= "延迟时间",
                    Type= "bar",
                    Data= new []{ 20, 20, 3006, 10, 10, 20 }
                }
            }
        };
    }

    [Parameter]
    public StringNumber Width { get; set; }

    [Parameter]
    public StringNumber Height { get; set; }

    [Parameter]
    public ChartDto Value { get; set; }

    protected override void OnParametersSet()
    {
        ConvertOption();
        base.OnParametersSet();
    }
}