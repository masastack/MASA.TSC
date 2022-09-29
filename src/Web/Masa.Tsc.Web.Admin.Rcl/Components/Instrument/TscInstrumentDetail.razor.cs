// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscInstrumentDetail
{
    [Parameter]
    public Guid Id { get; set; }

    [Parameter]
    public List<AddPanelDto> Panels { get; set; } = new();

    [Parameter]
    public bool ReadOnly { get; set; }

    public InstrumentDetailDto Data { get; set; }
    private List<BDragItem> _items = new();
    private AddPanelDto? _editPanel = default;
    private MDragZone _mDragZone;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (Id == Guid.Empty)
        {
            await PopupService.AlertAsync("Id must set", AlertTypes.Error);
            return;
        }
        if (Data == null || Data.Id != Id)
        {
            var dto = await ApiCaller.InstrumentService.GetAsync(CurrentUserId, Id);
            if (dto != null)
            {
                Data = dto;
                Panels.Clear();
                _items.Clear();
                if (dto.Panels != null)
                {
                    foreach (var item in Data.Panels)
                    {
                        var panel = new TextPanelDto
                        {
                            ParentId = item.ParentId,
                            Description = item.Description,
                            Height = item.Height,
                            Id = item.Id,
                            InstrumentId = dto.Id,
                            Sort = item.Sort,
                            Title = item.Title,
                            Type = item.Type,
                            Width = item.Width
                        };
                        _items.Add(ToDragItem(panel));
                        Panels.Add(panel);
                    }
                }
                StateHasChanged();
            }
            else
            {
                await PopupService.AlertAsync("Id or Data must set", AlertTypes.Error);
                return;
            }
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task ExecuteCommondAsync(OperateCommand command, params object[] values)
    {
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

        if (values != null)
            _editPanel = null;
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