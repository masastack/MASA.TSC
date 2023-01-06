// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class ProjectCharts
{
    private ErrorWarnChart? _errorWarnChart;
    private ServiceCallChart? _traceLogChart;
    private LogTraceChart? _traceLogChart1;
    private AvgResponseChart? _avgResponseChart;
    private GrowthChart? _growthChart;

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
        tasks.Add(_growthChart?.OnLoadAsync(query));
        //tasks.Add(_traceErrorChart?.OnLoadAsync(query));
        //tasks.Add(_traceWarnChart?.OnLoadAsync(query));     
        await Task.WhenAll(tasks);
    }
}