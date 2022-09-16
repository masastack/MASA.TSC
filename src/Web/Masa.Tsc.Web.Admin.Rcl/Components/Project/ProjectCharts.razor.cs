// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class ProjectCharts
{
    private ErrorWarnChart _errorWarnChart;
    private ErrorWarnChart _warnErrorChart;
    private LogTraceChart _logTraceChart;
    private LogTraceChart _traceLogChart;
    private ObserveChart _observeChart;
    private GrowthChart _growthChart;
    private LogTraceStatiscChart _traceErrorChart;
    private LogTraceStatiscChart _traceWarnChart;
    private LogTraceStatiscChart _logErrorChart;
    private LogTraceStatiscChart _logWarnChart;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {           
            //StateHasChanged();
        }
        return base.OnAfterRenderAsync(firstRender);
    }

    public async Task OnSearchAsync()
    {
        await Task.CompletedTask;
    }

    private ProjectAppSearchModel _query;

    public async Task OnLoadDataAsyc(ProjectAppSearchModel query)
    {
        _query= query;
        var tasks=new List<Task>();
        tasks.Add(_errorWarnChart.OnLoadAsync(query));
        tasks.Add(_warnErrorChart.OnLoadAsync(query));
        tasks.Add(_logTraceChart.OnLoadAsync(query));
        tasks.Add(_traceLogChart.OnLoadAsync(query));
        tasks.Add(_observeChart.OnLoadAsync(query));
        tasks.Add(_growthChart.OnLoadAsync(query));
        tasks.Add(_traceErrorChart.OnLoadAsync(query));
        tasks.Add(_traceWarnChart.OnLoadAsync(query));
        tasks.Add(_logErrorChart.OnLoadAsync(query));
        tasks.Add(_logWarnChart.OnLoadAsync(query));
        await Task.WhenAll(tasks);
    }
}