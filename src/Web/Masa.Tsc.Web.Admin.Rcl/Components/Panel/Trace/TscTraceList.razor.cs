// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceList : TscComponentBase
{
    [Parameter]
    public RequestTraceListDto Query { get; set; } = default!;

    private IEnumerable<TraceResponseDto> _data = new List<TraceResponseDto>();
    private int _total = 0;
    private MDataTable<TraceResponseDto> _mDataTable = default!;
    private bool _isLoading = false;
    private string _selectTraceId = default!;
    private List<DataTableHeader<TraceResponseDto>> _headers = new()
    {
        new("Service", item => item.Resource["service.name"])
        {
            Align = DataTableHeaderAlign.Start,
            Sortable = false
        },
        new("Endpoint", item => item.GetDispalyName())
        {
            Align= DataTableHeaderAlign.Start,
            Sortable = false
        },
        new("Duration (ms)", item => item.Duration)
        {
            Align = DataTableHeaderAlign.Start,
            Sortable = false
        },
        new("Timestamp", item => item.Timestamp)
        {
            Align = DataTableHeaderAlign.Start,
            Sortable = false
        },
        new DataTableHeader<TraceResponseDto>
        {
            Text = "Operate",
            Value = "Operate",
            Align = DataTableHeaderAlign.Start,
            Sortable = false
        }
    };
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    private async Task OpenAsync(TraceResponseDto item)
    {
        _selectTraceId = item.TraceId;
        OpenDialog();
        await Task.CompletedTask;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            //_isLoading = false;
            //await QueryAsync();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnUpdateOptionsAsync(DataOptions options)
    {
        Query.Page = options.Page;
        Query.PageSize = options.ItemsPerPage;
        await QueryAsync(false);
    }

    private void OnItemSelect(TraceResponseDto item, bool selected)
    {
        OpenAsync(item).Wait();
    }

    public async Task QueryAsync(bool isStateChange = true)
    {
        //_isLoading = true;
        //StateHasChanged();
        //await Task.Delay(1000);
        //_isLoading = false;
        //StateHasChanged();

        try
        {
            //if (isStateChange)
            //{
            //    _mDataTable.Options.Page = 1;
            //    Query.Page = 1;
            //}

            if (_isLoading) return;
            _isLoading = true;
            StateHasChanged();
            var data = await ApiCaller.TraceService.GetListAsync(Query, _cancellationTokenSource.Token);
            if (data != null)
            {
                _total = (int)data.Total;
                _data = data.Result ?? new();
            }
            else
            {
                _total = 0;
                _data = Array.Empty<TraceResponseDto>();
            }
            _isLoading = false;
            if (isStateChange)
                StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"message:{ex.Message}");
            Console.WriteLine($"trace: {ex.StackTrace}");
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _mDataTable.Dispose();
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        base.Dispose(disposing);
    }
}
