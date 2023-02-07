// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class ProjectCharts
{
    private ErrorWarnChart? _errorWarnChart;
    private ServiceCallChart? _traceLogChart;
    private ServiceResponseTimePercentile? _serviceResponseTimePercentile;
    private ServiceResponseAvgTime? _serviceResponseAvgTime;
    private ApdexChart? _apdexChart;
    private UpsertChartPanelDto _endpoint;
    private UpsertChartPanelDto _slowEndpoint;
    string? _oldConfigRecordKey;

    [Parameter]
    public ConfigurationRecord ConfigurationRecord { get; set; }

    protected override void OnInitialized()
    {
        DateTime start = ConfigurationRecord.StartTime.UtcDateTime,
            end = ConfigurationRecord.EndTime.UtcDateTime;
        var step = start.Interval(end);
        _endpoint = new UpsertChartPanelDto(Guid.Empty)
        {
            ChartType = "table",
            ListType = ListTypes.TopList,
            Title = T("Service Endpoint Load") + "(" + T("calls/min") + ")",
            //Description = T("Service Endpoint") + "(" + T("calls/min") + ")",
            Metrics = new List<PanelMetricDto>
            {
                new PanelMetricDto()
                {
                    Name = $"topk(10, sort_desc(round(sum by (http_target) (increase(http_response_count[{step}])),0.01)>0.01))"
                }
            },
            Height=2,
        };
        _slowEndpoint = new UpsertChartPanelDto(Guid.Empty)
        {
            ChartType = "table",
            ListType = ListTypes.TopList,
            Title = T("Service Slow Endpoint") + "(" + T("ms") + ")",
            //Description = "Service Slow Endpont(ms)",
            Metrics = new List<PanelMetricDto>
            {
                new PanelMetricDto()
                {
                    Name = "topk(10, sort_desc(max by(http_target) (http_response_bucket)))"
                }
            }
        };
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_oldConfigRecordKey != ConfigurationRecord.Key)
        {
            var back = _oldConfigRecordKey;
            _oldConfigRecordKey = ConfigurationRecord.Key;
            if (back is not null)
            {
                await OnLoadDataAsyc();
            }
        }
    }

    internal async Task OnLoadDataAsyc(ProjectAppSearchModel? query = null)
    {
       query = new()
        {
            AppId = ConfigurationRecord.AppName!,
            Start = ConfigurationRecord.StartTime.UtcDateTime,
            End = ConfigurationRecord.EndTime.UtcDateTime,
        };
        var tasks = new List<Task>();
        if (_errorWarnChart != null)
            tasks.Add(_errorWarnChart.OnLoadAsync(query));
        if (_traceLogChart != null)
            tasks.Add(_traceLogChart.OnLoadAsync(query));
        if (_serviceResponseTimePercentile != null)
            tasks.Add(_serviceResponseTimePercentile.OnLoadAsync(query));
        if (_serviceResponseAvgTime != null)
            tasks.Add(_serviceResponseAvgTime.OnLoadAsync(query));
        if (_apdexChart != null)
            tasks.Add(_apdexChart.OnLoadAsync(query));

        await Task.WhenAll(tasks);
    }
}