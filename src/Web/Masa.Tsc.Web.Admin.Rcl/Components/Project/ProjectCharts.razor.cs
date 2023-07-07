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
    private UpsertChartPanelDto? _endpoint;
    private UpsertChartPanelDto? _slowEndpoint;
    string? _oldConfigRecordKey;
    TscTraceDetail _tscTraceDetail = default!;
    private static bool isNew = false;

    [Parameter]
    public ConfigurationRecord ConfigurationRecord { get; set; }

    protected override void OnInitialized()
    {
        UpSetMetrics();
    }

    protected override async Task OnParametersSetAsync()
    {
        await OnLoadDataAsync();
        await base.OnParametersSetAsync();
    }

    internal async Task OnLoadDataAsync(ProjectAppSearchModel? query = null)
    {
        isNew = true;
        query = new()
        {
            AppId = ConfigurationRecord.Service!,
            Start = ConfigurationRecord.StartTime.UtcDateTime,
            End = ConfigurationRecord.EndTime.UtcDateTime,
        };
        UpSetMetrics();
        isNew = false;
        do
        {
            if (isNew)
                break;
            else if (_errorWarnChart == null || _traceLogChart == null || _serviceResponseAvgTime == null || _apdexChart == null || _serviceResponseTimePercentile == null)
                await Task.Delay(50);
            else
                break;

        } while (true);

        if (!isNew && _oldConfigRecordKey != ConfigurationRecord.Key)
        {
            var tasks = new Task[]
            {
                _errorWarnChart!.OnLoadAsync(query),
                _traceLogChart!.OnLoadAsync(query),
                _serviceResponseAvgTime!.OnLoadAsync(query),
                _apdexChart!.OnLoadAsync(query),
                _serviceResponseTimePercentile!.OnLoadAsync(query)
            };

            _oldConfigRecordKey = ConfigurationRecord.Key;
            await Task.WhenAll(tasks);
        }
    }

    private void UpSetMetrics()
    {
        DateTime start = ConfigurationRecord.StartTime.UtcDateTime,
            end = ConfigurationRecord.EndTime.UtcDateTime;
        if (_endpoint == null)
            _endpoint = new UpsertChartPanelDto(Guid.Empty)
            {
                ChartType = ChartTypes.Table,
                ListType = ListTypes.TopList,
                Title = I18n.Team("Service Endpoint Load(calls/min)"),
                Description = I18n.Team("For HTTP, gRPC, RPC services, this means Calls Per Minute (calls/min)"),
                Metrics = new List<PanelMetricDto>
                {
                    new()
                    {
                        Expression = $"topk(10, sort_desc(round(sum by (http_target) (increase(http_response_count[{MetricConstants.TIME_PERIOD}])),0.01)>0.01))",
                    }
                }
            };
        else
            _endpoint.Metrics[0].Expression = $"topk(10, sort_desc(round(sum by (http_target) (increase(http_response_count[{MetricConstants.TIME_PERIOD}])),1)>0))";

        if (_slowEndpoint == null)
            _slowEndpoint = new UpsertChartPanelDto(Guid.Empty)
            {
                ChartType = ChartTypes.Table,
                ListType = ListTypes.TopList,
                Title = I18n.Team("Service Slow Endpoint") + "(" + I18n.T("ms") + ")",
                //Description = "Service Slow Endpont(ms)",
                Metrics = new List<PanelMetricDto>
                {
                    new()
                    {
                        Expression = $"topk(10, sort_desc( round(sum by(http_target) (increase(http_response_sum[{MetricConstants.TIME_PERIOD}]))/sum by(http_target) (increase(http_response_count[{MetricConstants.TIME_PERIOD}])),1))>0)",
                        Caculate="MAX"
                    }
                },
                TopListOnclick = async options =>
                {
                    var url = options.Text;
                    await _tscTraceDetail.OpenAsync(ConfigurationRecord, url);
                }
            };
    }
}