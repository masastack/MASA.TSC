// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm.Services;

public partial class ServiceErrors
{
    [CascadingParameter]
    public SearchData SearchData { get; set; }

    [Parameter]
    public MetricTypes MetricType { get; set; }

    private List<DataTableHeader<ErrorMessageDto>> headers => new()
    {
        new() { Text = I18n.Apm("Error.List.Type"), Value = nameof(ErrorMessageDto.Type)},
        new() { Text = I18n.Apm("Error.List.LastTime"), Value = nameof(ErrorMessageDto.LastTime) },
        new() { Text = I18n.Apm("Error.List.Total"), Value = nameof(ErrorMessageDto.Total)}
    };

    private int defaultSize = 5;
    private int total = 0;
    private int page = 1;
    private List<ErrorMessageDto> data = new();
    private bool isTableLoading = false;
    private string? sortFiled;
    private bool? sortBy;
    private string lastKey = string.Empty;
    private ChartData chart = new();

    private async Task OnTableOptionsChanged(DataOptions sort)
    {
        if (sort.SortBy.Any())
            sortFiled = sort.SortBy.First();
        else
            sortFiled = default;
        if (sort.SortDesc.Any())
            sortBy = sort.SortDesc.First();
        else
            sortBy = default;
        await LoadASync();
        StateHasChanged();
    }
  
    protected override async Task OnParametersSetAsync()
    {
        var key = MD5Utils.Encrypt(JsonSerializer.Serialize(SearchData));
        if (lastKey != key)
        {
            lastKey = key;
            await LoadASync();
            await LoadChartDataAsync();
        }
        await base.OnParametersSetAsync();
    }

    private async Task OnPageChange(int page)
    {
        this.page = page;
        await LoadPageDataAsync();
    }

    private async Task LoadASync()
    {
        if (isTableLoading)
        {
            return;
        }
        await LoadPageDataAsync();
    }

    private async Task LoadChartDataAsync()
    {
        var query = new ApmEndpointRequestDto
        {
            Page = page,
            PageSize = defaultSize,
            Start = SearchData.Start,
            End = SearchData.End,
            OrderField = sortFiled,
            Service = SearchData.Service,
            Endpoint = SearchData.Endpoint!,
            Env = SearchData.Enviroment,
            IsDesc = sortBy
        };
        var result = await ApiCaller.ApmService.GetErrorChartAsync(query);
        chart.Data = ConvertLatencyChartData(result, lineName: I18n.Apm("Chart.ErrorCount")).Json;
        chart.ChartLoading = false;
    }

    private async Task LoadPageDataAsync()
    {
        if (isTableLoading) return;
        isTableLoading = true;
        var query = new ApmEndpointRequestDto
        {
            Page = page,
            PageSize = defaultSize,
            Start = SearchData.Start,
            End = SearchData.End,
            OrderField = sortFiled,
            Service = SearchData.Service,
            Env = SearchData.Enviroment,
            IsDesc = sortBy,
            Endpoint = SearchData.Endpoint!
        };
        var result = await ApiCaller.ApmService.GetErrorsPageAsync(query);
        data = result.Result?.ToList() ?? new();
        total = (int)result.Total;
        isTableLoading = false;
    }

    private static EChartType ConvertLatencyChartData(List<ChartLineCountDto> data, string lineColor = null, string areaLineColor = null, string? unit = null, string? lineName = null)
    {
        var chart = EChartConst.Line;
        chart.SetValue("tooltip", new { trigger = "axis" });
        if (!string.IsNullOrEmpty(lineName))
        {
            chart.SetValue("legend", new { data = new string[] { $"{lineName}" }, bottom = "2%" });
        }

        chart.SetValue("yAxis", new object[] {
            new {type="value",axisLabel=new{formatter=$"{{value}} {unit}" } }
        });
        chart.SetValue("grid", new { top = "10%", left = "2%", right = "5%", bottom = "15%", containLabel = true });
        //if (data != null && data.Any())
        {
            chart.SetValue("xAxis", new object[] {
                new { type="category",boundaryGap=false,data=data?.Select(item=>item.Name)}
            });
            chart.SetValue($"series[0]", new { name = $"{lineName}", type = "line", smooth = true, areaStyle = new { }, lineStyle = new { width = 1 }, symbol = "none", data = data?.Select(item => item.Currents.First().Value) });
        }

        return chart;
    }
}
