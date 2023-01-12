﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class ProjectCharts
{
    private ErrorWarnChart? _errorWarnChart;
    private ServiceCallChart? _traceLogChart;
    private LogTraceChart? _traceLogChart1;
    private AvgResponseChart? _avgResponseChart;
    private ApdexChart? _apdexChart;
    private UpsertChartPanelDto _endpoint;
    private UpsertChartPanelDto _slowEndpoint;
    string? _oldConfigRecordKey;

    [Parameter]
    public ConfigurationRecord ConfigurationRecord { get; set; }

    protected override void OnInitialized()
    {
        _endpoint = new UpsertChartPanelDto(default)
        {
            ChartType = "table",
            ListType = ListTypes.TopList,
            Title = "Service Endpoint(calls/min)",
            Description = "Service Endpoint(calls/min)",
            Metrics = new List<PanelMetricDto>
            {
                new PanelMetricDto()
                {
                    Name = "topk(10, sort_desc(round(sum by (http_target) (increase(http_response_count[1m])),0.01)>0.01))"
                }
            }
        };
        _slowEndpoint = new UpsertChartPanelDto(default)
        {
            ChartType = "table",
            ListType = ListTypes.TopList,
            Title = "Service Slow Endpont(ms)",
            Description = "Service Slow Endpont(ms)",
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
        query ??= new ()
        {
            AppId = ConfigurationRecord.AppName,
            Start = ConfigurationRecord.StartTime.UtcDateTime,
            End = ConfigurationRecord.EndTime.UtcDateTime,
        };
        var tasks = new List<Task>();
        tasks.Add(_errorWarnChart?.OnLoadAsync(query));
        tasks.Add(_traceLogChart?.OnLoadAsync(query));
        tasks.Add(_traceLogChart1?.OnLoadAsync(query));
        tasks.Add(_avgResponseChart?.OnLoadAsync(query));
        tasks.Add(_apdexChart?.OnLoadAsync(query));   
        await Task.WhenAll(tasks);
    }
}