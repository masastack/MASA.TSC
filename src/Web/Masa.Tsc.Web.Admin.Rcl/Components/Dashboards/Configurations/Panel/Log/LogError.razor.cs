// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Panel.Log;

public partial class LogError
{
    [Inject]
    IJSRuntime JSRuntime { get; set; }

    [Parameter]
    public ConfigurationRecord ConfigurationRecord { get; set; }

    private string key = string.Empty;
    private List<LogErrorDto> Columns = new();
    private int pageIndex = 1;
    private int pageSize = 10;

    private readonly List<DataTableHeader<LogErrorDto>> headers = new() {
        new (){ Text="Message",Value=nameof(LogErrorDto.Message)},
        new (){ Text="Count",Value=nameof(LogErrorDto.Count)}
    };

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        //ConfigurationRecord.Service = "scheduler-service-dev";
        var newKey = $"{ConfigurationRecord.Service}_{ConfigurationRecord.StartTime}_{ConfigurationRecord.EndTime}";
        if (key == newKey)
            return;
        key = newKey;
        Columns = await ApiCaller.LogService.GetErrorTypesAsync(ConfigurationRecord.Service!, ConfigurationRecord.StartTime.UtcDateTime, ConfigurationRecord.EndTime.UtcDateTime);
    }

    private async Task OpenLogAsync(LogErrorDto item)
    {
        var url = $"/log/{ConfigurationRecord.Service}/{ConfigurationRecord.StartTime.UtcDateTime:yyyy-MM-dd HH:mm:ss}/{ConfigurationRecord.EndTime.UtcDateTime:yyyy-MM-dd HH:mm:ss}/{item.Message.Replace('.','。')}";
        await JSRuntime.InvokeVoidAsync("open", url, "_blank");
    }
}