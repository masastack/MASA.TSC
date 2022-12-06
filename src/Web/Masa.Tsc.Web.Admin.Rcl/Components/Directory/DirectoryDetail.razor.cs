﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class DirectoryDetail
{
    [Parameter]
    public Guid DirectoryId { get; set; } = Guid.Empty;

    [Parameter]
    public Guid ParentId { get; set; } = Guid.Empty;

    [Parameter]
    public string ParentName { get; set; }

    [Parameter]
    public Func<DirectoryDto, Task> SuccessFn { get; set; }

    private UpdateDirectoryDto _model = new();

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task OnParametersSetAsync()
    {
        //if (DirectoryId != Guid.Empty)
        //{
        //    if (_model.Id != DirectoryId)
        //    {
        //        var data = await ApiCaller.DirectoryService.GetAsync(DirectoryId);
        //        if (data != null)
        //        {
        //            _model.Id = DirectoryId;
        //            _model.ParentId = data.ParentId;
        //            _model.Name = data.Name;
        //            //_model.Sort = data.Sort;
        //            _model.UserId = CurrentUserId;
        //        }
        //        else
        //        {
        //            await PopupService.AlertAsync($"${DirectoryId} is not exists", AlertTypes.Error);
        //        }
        //    }
        //}
        //else
        //{
        //    _model.Id = DirectoryId;
        //    _model.ParentId = ParentId;
        //    _model.UserId = CurrentUserId;
        //    _model.Name = default!;
        //    _model.Sort = default!;
        //}
        await base.OnParametersSetAsync();
    }

    private async Task SaveAsync()
    {
        //if (_model.Id.Equals(Guid.Empty))
        //    await ApiCaller.DirectoryService.AddAsync(_model);
        //else
        //    await ApiCaller.DirectoryService.UpdateAsync(_model);
        //if (SuccessFn is not null)
        //    await SuccessFn.Invoke(new());
    }
}
