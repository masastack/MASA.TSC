// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages;

public partial class Directory : IDisposable
{
    private bool _isLoading = true;
    private string _keyword;
    private bool _expand = true;
    private DirectoryTypes _opType = DirectoryTypes.Directory;

    private DirectoryTreeDto _current = new();
    private IEnumerable<DirectoryTreeDto> _data;
    private IEnumerable<DirectoryTreeDto> _searchData;

    private string _title;
    private bool _isUpdate = false;
    private bool _fullScreen = false;
    private StringNumber _dialogWidth = 480;

    public AddInstrumentDto _addDto = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadDataAsync();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void OnExpandChangeAsync(StringNumber value)
    {
        _expand = value.ToInt32() == 1;
        SetExpand(_data);
        StateHasChanged();
    }

    private void SetExpand(IEnumerable<DirectoryTreeDto> data)
    {
        if (data == null || !data.Any())
            return;
        foreach (var item in data)
        {
            item.Expand = _expand;
            SetExpand(item.Children);
        }
    }

    private async Task LoadDataAsync()
    {
        _isLoading = true;
        _data = await ApiCaller.DirectoryService.GetTreeAsync(CurrentUserId);
        SetExpand(_data);
        _searchData = Array.Empty<DirectoryTreeDto>();
        _isLoading = false;
        StateHasChanged();
    }

    private void SetSeachData()
    {
        if (string.IsNullOrEmpty(_keyword)) _searchData = default!;
    }

    private IEnumerable<DirectoryTreeDto> Search(IEnumerable<DirectoryTreeDto> data)
    {
        foreach (var item in data)
        {
            if (item.Children != null && item.Children.Any())
            {

            }
        }
        return Array.Empty<DirectoryTreeDto>();

    }

    private void AddDirectory()
    {
        _title = "Add Directory";
        _fullScreen = false;
        _opType = DirectoryTypes.Directory;
        _isUpdate = false;
        OpenDialog();
    }

    private async void AddInstrument()
    {
        if (_current == null || _current.Id == Guid.Empty || _current.DirectoryType != DirectoryTypes.Directory)
        {
            await PopupService.AlertAsync("请选择目录", AlertTypes.Warning);
            return;
        }
        _opType = DirectoryTypes.Instrument;
        _title = "Add Instrument";
        _addDto = new() { DirectoryId = _current.Id };
        _isUpdate = false;
        _dialogWidth = 480;
        _fullScreen = false;
        OpenDialog();
    }

    private async Task OnAddInstrument(AddInstrumentDto model)
    {
        var list = _current.Children?.ToList() ?? new();
        list.Add(new DirectoryTreeDto
        {
            ParentId = _current.Id,
            DirectoryType = DirectoryTypes.Instrument,
            Id = model.Id,
            Name = model.Name,
            Sort = model.Sort,
        });
        _current.Children = list;
        _addDto = model;
        _fullScreen = true;
        _isUpdate = true;
        _dialogWidth = "100%";
        StateHasChanged();
        await Task.CompletedTask;
    }

    private async Task OnClose()
    {
        CloseDialog();
        _addDto = new();
        _dialogWidth = 480;
        _fullScreen = false;
        await Task.CompletedTask;
    }

    private async Task OpenUpdateAsync(DirectoryTreeDto item)
    {
        if (item.DirectoryType == DirectoryTypes.Directory)
        {
            _title = "Update Directory";
            _dialogWidth = 480;
            _fullScreen = false;
            _addDto = new();
        }
        else
        {
            _fullScreen = true;
            _dialogWidth = "100%";
            _title = "Update Instrument";
            _addDto.Id = item.Id;
        }
        _opType = item.DirectoryType;
        _isUpdate = true;
        _current = item;
        OpenDialog();
        StateHasChanged();
        await Task.CompletedTask;
    }

    private async Task RemoveDirectoryAsync(DirectoryTreeDto item)
    {
        if (!await PopupService.ConfirmAsync("Delete Confirm", "Are you sure you want to delete this data!"))
            return;

        if (item.DirectoryType == DirectoryTypes.Directory)
        {
            if (item.Children != null && item.Children.Any())
            {
                await PopupService.AlertAsync("Has Children Can't Delete", AlertTypes.Warning);
                return;
            }

            await ApiCaller.DirectoryService.DeleteAsync(item.Id, CurrentUserId);
            await PopupService.ToastAsync("Delete Ok", AlertTypes.Success);
            var list = _data.ToList();
            list.Remove(item);
            _data = list;
            if (_searchData.Any())
            {
                var seachList = _searchData.ToList();
                if (seachList.Contains(item))
                {
                    seachList.Remove(item);
                    _searchData = seachList;
                }
            }
            StateHasChanged();
        }
        else
        {
            await ApiCaller.InstrumentService.DeleteAsync(new CommonRemoveDto<Guid>
            {
                Ids = new Guid[] { item.Id },
                UserId = CurrentUserId
            });
        }

        await PopupService.ToastAsync("delete success", AlertTypes.Success);
        await LoadDataAsync();
    }

    private async Task AddUpdateCallback(DirectoryDto dto)
    {
        CloseDialog();
        StateHasChanged();
        await PopupService.ToastAsync("Oprate Success", AlertTypes.Success);
        await Task.CompletedTask;
    }

    private async Task OnRowSelectedAsync(DirectoryTreeDto dto)
    {
        if (dto.Selected)
        {
            _current = dto;
        }
        else
        {
            _current = new();
        }
        await Task.CompletedTask;
    }

    private string GetParentName(IEnumerable<DirectoryTreeDto> data)
    {
        var id = _isUpdate ? _current.ParentId : _current.Id;
        if (id == Guid.Empty)
            return "\\";
        foreach (var item in data)
        {
            if (item.Id == id)
                return item.Name;
            if (item.Children != null && item.Children.Any())
            {
                var parentName = GetParentName(item.Children);
                if (!string.IsNullOrEmpty(parentName))
                    return parentName;
            }
        }
        return default!;
    }

    protected override async Task ChildCallHandler(params object[] values)
    {
        //add instruments success
        if (values != null && values.Length == 3)
        {
            if (values[0] is string op && values[1] is string type)
            {
                if (op == "add" && type == "instrument")
                {
                    await OnAddInstrument((AddInstrumentDto)values[2]);
                }
            }
        }


        await base.ChildCallHandler(values!);
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}