// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages;

public partial class Directory
{
    private bool _isLoading = true;
    private string _keyword;
    private bool _expand = true;
    private bool _showDialog = false;
    private string _title;
    private bool _isUpdate = false;
    private DirectoryTreeDto _current = new();
    private IEnumerable<DirectoryTreeDto> _data;
    private IEnumerable<DirectoryTreeDto> _searchData;

    protected override async Task OnInitializedAsync()
    {
        _isLoading = true;
        await LoadDataAsync();
        _isLoading = false;
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
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
        _data = await ApiCaller.DirectoryService.GetTreeAsync(CurrentUserId);
        SetExpand(_data);
        _searchData = Array.Empty<DirectoryTreeDto>();
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
        _current.DirectoryType = DirectoryTypes.Directory;
        _isUpdate = false;
        _showDialog = true;
    }

    private void AddInstrument()
    {
        _title = "Add Instrument";
        _current.DirectoryType = DirectoryTypes.Instrument;
        _isUpdate = false;
        _showDialog = true;
    }

    private async Task OpenUpdateAsync(DirectoryTreeDto item)
    {
        if (item.DirectoryType == DirectoryTypes.Directory)
        {
            _title = "Update Directory";
        }
        else
        {
            _title = "Update Instrument";
        }
        _isUpdate = true;
        _current = item;
        _showDialog = true;
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
            //_title = "Update Instrument";
            //_currentParentId = item.ParentId;
        }

        item.Id = Guid.Empty;
        item.ParentId = Guid.Empty;
        item.DirectoryType = DirectoryTypes.Directory;
    }

    private async Task AddUpdateCallback(DirectoryDto dto)
    {
        _showDialog = false;
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
                var find = GetParentName(item.Children);
                if (!string.IsNullOrEmpty(find))
                    return find;
            }
        }
        return default!;
    }
}