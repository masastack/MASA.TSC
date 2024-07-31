// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

//namespace Masa.Tsc.Web.Admin.Rcl.Components.Apm;

//public partial class ApmInputMuiltiSearchComponent
//{
//    [Parameter]
//    public EventCallback<TextSearchModel> ValueChanged { get; set; }

//    [Parameter]
//    public TextSearchModel Value { get; set; }

//    private List<string> endpoints = new();
//    private List<string> statuses = new();
//    private List<string> exceptions = new();

//    protected override async Task OnInitializedAsync()
//    {
//        await base.OnInitializedAsync();
//        await LoadAsync();
//    }

//    private async Task OnEndpointChange(string value)
//    {
//        await OnValueChanged();
//    }

//    private async Task OnStatusCodeChange(string value)
//    {
//        await OnValueChanged();
//    }

//    private async Task OnExceptionChange(string value)
//    {
//        await OnValueChanged();
//    }

//    private async Task OnMessageEnter()
//    {
//        await OnValueChanged();
//    }

//    private async Task OnValueChanged()
//    {
//        if (ValueChanged.HasDelegate)
//            await ValueChanged.InvokeAsync(Value);
//    }

//    public async Task OnServiceChanged(string serviceName)
//    {
//        endpoints = await ApiCaller.ApmService.GetEndpointsAsync(new BaseRequestDto { Service = serviceName, End = DateTime.UtcNow, Start = DateTime.UtcNow.AddDays(-30) });
//        exceptions = await ApiCaller.ApmService.GetExceptionTypesAsync(new BaseRequestDto { End = DateTime.UtcNow, Start = DateTime.UtcNow.AddDays(-30) });
//    }

//    private async Task LoadAsync()
//    {
//        statuses = await ApiCaller.ApmService.GetStatusCodesAsync();
//        exceptions = await ApiCaller.ApmService.GetExceptionTypesAsync(new BaseRequestDto { End = DateTime.UtcNow, Start = DateTime.UtcNow.AddDays(-30) });
//    }
//}