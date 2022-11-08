// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscTraceList : TscComponentBase
{
    [Parameter]
    public RequestTraceListDto Query { get; set; } = default!;

    private IEnumerable<TraceDto> _data = new List<TraceDto>();
    private int _total = 0;
    private MDataTable<TraceDto> _mDataTable = default!;
    private bool _isLoading = false;
    private string _selectTraceId = default!;
    private List<DataTableHeader<TraceDto>> _headers = new()
    {
        new("Service", item => item.Resource["service.name"])
        {
            Align = "start",
            Sortable = false
        },
        new("Endpoint", item => TraceDto.GetDispalyName(item))
        {
            Align = "start",
            Sortable = false
        },
        new("Duration (ms)", item => item.GetDuration())
        {
            Align = "start",
            Sortable = false
        },
        new("Timestamp", item => item.Timestamp)
        {
            Align = "start",
            Sortable = false
        },
        new DataTableHeader<TraceDto>
        {
            Text = "Operate",
            Value = "Operate",
            Align = "start",
            Sortable = false
        }
    };
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    private async Task OpenAsync(TraceDto item)
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

    private void OnItemSelect(TraceDto item, bool selected)
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
                _data = data.Items ?? new();
            }
            else
            {
                _total = 0;
                _data = Array.Empty<TraceDto>();
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
