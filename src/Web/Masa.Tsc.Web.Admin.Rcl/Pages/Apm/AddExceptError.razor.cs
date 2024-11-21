// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Apm;

public partial class AddExceptError
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public LogResponseDto Log { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback<RequestAddExceptError> OnSubmitSuccess { get; set; }

    public MForm? Form { get; set; }

    RequestAddExceptError model = new();

    private async Task UpdateVisible(bool visible)
    {
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(visible);
        }
        else
        {
            Visible = false;
        }
    }

    protected override void OnParametersSet()
    {
        if (Visible)
        {
            SetValue();
        }
    }

    public async Task AddAsync()
    {
        if (string.IsNullOrEmpty(model.Comment))
        {
            await PopupService.EnqueueSnackbarAsync(I18n.Apm("Comment is Reqired"), AlertTypes.Error);
            return;
        }
        var success = Form!.Validate();
        if (success)
        {
            await ApiCaller.ExceptErrorService.AddAsync(model);
            await PopupService.EnqueueSnackbarAsync(I18n.Apm("Add except error success"), AlertTypes.Success);
            await UpdateVisible(false);
        }
    }

    private void SetValue()
    {
        model = new RequestAddExceptError();
        if (Log == null)
            return;
        int resourceLength = nameof(Log.Resource).Length + 1, attributeLength = nameof(Log.Attributes).Length + 1;
        model.Environment = GetValue(Log.Resource, StorageConst.Current.Environment.Substring(resourceLength));
        model.Service = GetValue(Log.Resource, "service.name");
        model.Type = GetValue(Log.Attributes, "exception.type");
        model.Message = GetValue(Log.Attributes, "exception.message");
        model.Project = GetService(model.Service)?.ProjectId!;
    }

    private static string GetValue(Dictionary<string, object> dic, string key)
    {
        if (dic == null || dic.Count == 0) return default!;
        if (dic.TryGetValue(key, out var value))
            return value.ToString()!;
        return default!;
    }
}
