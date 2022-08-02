// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Pages.Components;

public partial class TscDirectoryTreeTable
{
    [Parameter]
    public IEnumerable<DirectoryTreeDto> Data { get; set; }

    [Parameter]
    public int Deep { get; set; }

    [Parameter]
    public bool Expand { get; set; }

    [Parameter]
    public Func<DirectoryTreeDto, Task> OnUpdateAsync { get; set; }

    [Parameter]
    public Func<DirectoryTreeDto, Task> OnDeleteAsync { get; set; }

    [Parameter]
    public Func<DirectoryTreeDto, Task> OnRowSelected { get; set; }

    private string _class;
    private DirectoryTreeDto? _selectedItem = null;

    protected override Task OnParametersSetAsync()
    {
        _class = $"pl-{(Deep * 8)}";
        return base.OnParametersSetAsync();
    }

    private async Task TroggleExpandAsync(DirectoryTreeDto item)
    {
        item.Expand = !item.Expand;
        StateHasChanged();
        await Task.CompletedTask;
    }

    private async Task UpdateAsync(DirectoryTreeDto item)
    {
        if (OnUpdateAsync is not null)
            await OnUpdateAsync.Invoke(item);
    }

    private async Task DeleteAsync(DirectoryTreeDto item)
    {
        if (OnDeleteAsync is not null)
            await OnDeleteAsync.Invoke(item);
    }

    private async Task RowSelectAsync(DirectoryTreeDto item)
    {
        if (Deep != 0)
        {
            await OnRowSelected.Invoke(item);
        }
        else
        {
            if (item.Selected)
            {
                item.Selected = false;
                if (OnRowSelected is not null)
                {
                    await OnRowSelected.Invoke(item);
                }
            }
            else
            {
                var find = FindSelect(Data, item.Id);
                item.Selected = true;
                if (OnRowSelected is not null)
                {
                    if (find != null)
                        await OnRowSelected.Invoke(find);
                    await OnRowSelected.Invoke(item);
                }
            }
            StateHasChanged();
        }
    }

    private DirectoryTreeDto FindSelect(IEnumerable<DirectoryTreeDto> data, Guid id)
    {
        if (data is null || !data.Any())
            return default!;
        foreach (var item in data)
        {
            if (item.Selected && item.Id != id)
            {
                item.Selected = false;
                return item;
            }
            else
            {
                var find = FindSelect(item.Children, id);
                if (find != null)
                    return find;
            }
        }
        return default!;
    }
}