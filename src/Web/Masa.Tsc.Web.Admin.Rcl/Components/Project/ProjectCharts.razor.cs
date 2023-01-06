// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Configurations.Models;

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
    private ConfigurationRecord _configurationRecord;

    protected override void OnInitialized()
    {
        _configurationRecord = new ConfigurationRecord();
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
                    Name = "topk(10, avg by(service_name) (http_server_duration_bucket))"
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
                    Name = "topk(10, count by(service_name) (increase(http_server_duration_count[5m])))"
                }
            }
        };
    }

    public async Task OnSearchAsync()
    {
        await Task.CompletedTask;
    }

    private ProjectAppSearchModel _query;

    internal async Task OnLoadDataAsyc(ProjectAppSearchModel query)
    {
        _query = query;
        var tasks = new List<Task>();
        tasks.Add(_errorWarnChart?.OnLoadAsync(query));
        tasks.Add(_traceLogChart?.OnLoadAsync(query));
        tasks.Add(_traceLogChart1?.OnLoadAsync(query));
        tasks.Add(_avgResponseChart?.OnLoadAsync(query));
        tasks.Add(_apdexChart?.OnLoadAsync(query));
        //tasks.Add(_traceErrorChart?.OnLoadAsync(query));
        //tasks.Add(_traceWarnChart?.OnLoadAsync(query));     
        await Task.WhenAll(tasks);
    }
}