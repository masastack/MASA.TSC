// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class SearchBar
{
    [Parameter]
    public List<AppDto> Apps
    {
        get { return _apps; }
        set
        {
            _apps = value ?? new();
            if (_apps != null && _apps.Any() && string.IsNullOrEmpty(_value.AppId))
            {
                _value.AppId = _apps.First().Identity;
            }
        }
    }

    [Parameter]
    public ProjectAppSearchModel Value
    {
        get { return _value; }
        set
        {
            if (value != null)
            {
                _value = value;
            }
            else
            {
                _value.AppId = default!;
                _value.Interval = default!;
                _value.Start = null;
                _value.End = null;
            }
        }
    }

    [Parameter]
    public EventCallback<ProjectAppSearchModel> OnSearch { get; set; }

    private ProjectAppSearchModel _value = new();
    private List<AppDto> _apps = new();

    private AppDto GetApp() => Apps?.FirstOrDefault(item => item.Identity == _value.AppId);

    private async Task SearchAsync()
    {
        if (OnSearch.HasDelegate)
            await OnSearch.InvokeAsync(Value);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && OnSearch.HasDelegate)
        {
            await SearchAsync();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnSelectItemUpdate(AppDto item)
    {
        _value.AppId = item.Identity;
        StateHasChanged();
        await SearchAsync();
    }
}