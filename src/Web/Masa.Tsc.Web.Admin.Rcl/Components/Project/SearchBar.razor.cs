// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class SearchBar
{
    [Parameter]
    public List<AppDto> Apps { get; set; } = new List<AppDto>
    {
        new AppDto
        {
            Name="aaa",
            Identity="111"
        }
    };

    [Parameter]
    public string AppId
    {
        get { return _value.AppId; }
        set { 
            _defaultAppId = value;
            if(_value.AppId==null)
                _value.AppId = _defaultAppId;
        }
    }

    [Parameter]
    public ProjectAppSearchModel Value
    {
        get { return _value; }
        set
        {
            if (value != null)
                _value = value;
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

    private string _searchIconClass = "fas fa-rotate";
    //private bool _loading = false;
    private ProjectAppSearchModel _value = new();
    private string _defaultAppId;
    private List<string> _selectDataSource = new List<string>
    {
        "15分钟",
        "30分钟",
        "1小时",
        "5小时"
    };

    private async Task SearchAsync()
    {
        //_searchIconClass = "fas fa-circle-notch fa-spin";
        //StateHasChanged();
        //_loading = true;
        if (OnSearch.HasDelegate)
            await OnSearch.InvokeAsync(Value);
        //_loading = false;
        //Thread.Sleep(500);
        //_searchIconClass = "fas fa-rotate";
        //StateHasChanged();
        //await Task.CompletedTask;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (OnSearch.HasDelegate)
                await OnSearch.InvokeAsync(Value);
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}