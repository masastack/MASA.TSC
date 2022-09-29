// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscDirectoryTreeTable
{
    [Parameter]
    public IEnumerable<DirectoryTreeDto> Data { get; set; }

    [Parameter]
    public int Deep { get; set; }

    [Parameter]
    public bool Expand { get; set; }

    //[Parameter]
    //public Func<DirectoryTreeDto, Task> OnUpdateAsync { get; set; }

    //[Parameter]
    //public Func<DirectoryTreeDto, Task> OnDeleteAsync { get; set; }

    [Parameter]
    public Func<DirectoryTreeDto, Task> OnRowSelected { get; set; }

    private string _class;

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
        await CallParent(OperateCommand.Update, item);
        //if (OnUpdateAsync is not null)
        //    await OnUpdateAsync.Invoke(item);
    }

    private async Task ViewAsync(DirectoryTreeDto item)
    {
        await CallParent(OperateCommand.View, item);
        //if (OnUpdateAsync is not null)
        //    await OnUpdateAsync.Invoke(item);
    }

    private async Task DeleteAsync(DirectoryTreeDto item)
    {
        await CallParent(OperateCommand.Remove, item);
        //if (OnDeleteAsync is not null)
        //    await OnDeleteAsync.Invoke(item);
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
                var selected = FindSelected(Data, item.Id);
                item.Selected = true;
                if (OnRowSelected is not null)
                {
                    if (selected != null)
                        await OnRowSelected.Invoke(selected);
                    await OnRowSelected.Invoke(item);
                }
            }
            StateHasChanged();
        }
    }

    private DirectoryTreeDto FindSelected(IEnumerable<DirectoryTreeDto> data, Guid id)
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
                var selected = FindSelected(item.Children, id);
                if (selected != null)
                    return selected;
            }
        }
        return default!;
    }
}