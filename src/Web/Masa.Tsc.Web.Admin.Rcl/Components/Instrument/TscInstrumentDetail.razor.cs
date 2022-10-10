// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscInstrumentDetail
{
    [Parameter]
    public Guid Id { get; set; }

    [Parameter]
    public Guid ParentId { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public List<AddPanelDto> Panels { get; set; } = new();

    private List<BDragItem> _items = new();
    private AddPanelDto? _editPanel = default;
    private MDragZone _mDragZone;

    private bool _reload = false;

    //public override Task SetParametersAsync(ParameterView parameters)
    //{
    //    if (parameters.TryGetValue(nameof(Id), out Guid id) && parameters.TryGetValue(nameof(ParentId), out Guid parentId))
    //    {
    //        if (parameters.TryGetValue(nameof(Panels), out List<AddPanelDto> panels))
    //        {
    //            _reload = true;
    //        }
    //        else
    //        {
    //            if (id != Id)
    //                _reload = true;
    //        }
    //    }

    //    return base.SetParametersAsync(parameters);
    //}


    protected override async Task OnParametersSetAsync()
    {
        //tab item
        if (ParentId != Guid.Empty)
        {
            StateHasChanged();
            //if(Panels)
        }//
        else if (Id == Guid.Empty)
        {
            await PopupService.AlertAsync("Id must set", AlertTypes.Error);
        }
        if (ParentId == Guid.Empty)
        {
            var dto = await ApiCaller.InstrumentService.GetAsync(CurrentUserId, Id);
            if (dto != null)
            {
                Panels.Clear();
                _items.Clear();
                if (dto.Panels != null)
                {
                    foreach (var item in dto.Panels)
                    {
                        //var panel = new TextPanelDto
                        //{
                        //    ParentId = item.ParentId,
                        //    Description = item.Description,
                        //    Height = item.Height,
                        //    Id = item.Id,
                        //    InstrumentId = dto.Id,
                        //    Sort = item.Sort,
                        //    Title = item.Title,
                        //    Type = item.Type,
                        //    Width = item.Width
                        //};
                        _items.Add(ToDragItem(item));
                        Panels.Add(item);
                    }
                }
                StateHasChanged();
            }
            else
            {
                await PopupService.AlertAsync("Id or Data must set", AlertTypes.Error);
            }
            _reload = false;
        }
        await base.OnParametersSetAsync();
    }




    protected override async Task OnAfterRenderAsync(bool firstRender)
    {

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task ExecuteCommondAsync(OperateCommand command, params object[] values)
    {
        _editPanel = null;
        if (values != null && values.Length > 0 && values[0] is AddPanelDto panel)
        {
            var find = _mDragZone.Items.FirstOrDefault(x => (Guid)x.Attributes["key"] == panel.Id);
            if (command == OperateCommand.Success)
            {
                if (find == null)
                {
                    _items.Add(ToDragItem(panel));
                    Panels.Add(panel);
                }
                else
                {
                    var findPanel = Panels.FirstOrDefault(m => m.Id == panel.Id);
                    findPanel = panel;

                    var index = _items.IndexOf(find);
                    _items.Remove(find);
                    _items.Insert(index, ToDragItem(panel));
                }
                CloseDialog();
            }
            else if (command == OperateCommand.Update)
            {
                ReadOnly = false;
                //_editPanel = panel;
                await ShowEdit(panel);
            }
            else if (command == OperateCommand.Remove)
            {
                if (find != null)
                {
                    _items.Remove(find);
                    Panels.RemoveAll(m => m.Id == panel.Id);
                    find.Dispose();
                    CloseDialog();
                }
            }
        }
    }

    private BDragItem ToDragItem(AddPanelDto panel)
    {
        return new BDragItem
        {
            Id = Guid.NewGuid().ToString(),
            Attributes = new Dictionary<string, object> { { "key", panel.Id } },
            ChildContent = builder =>
            {
                builder.OpenComponent<TscPanelEdit>(0);
                builder.AddAttribute(1, "Item", panel);
                builder.AddAttribute(2, "ReadOnly", ReadOnly);
                builder.AddAttribute(3, "OnCallParent", new EventCallback<object[]>(this, ChildCallHandler));
                builder.CloseComponent();
            }
        };
    }

    private async Task ShowEdit(AddPanelDto panel)
    {
        _editPanel = panel;
        OpenDialog();
        await Task.CompletedTask;
    }
}