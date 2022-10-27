// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscInstrumentDetail
{
    [Parameter]
    public Guid InstrumentId { get; set; }

    [Parameter]
    public Guid ParentId { get; set; }

    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public List<PanelDto> Panels
    {
        get { return _panels; }
        set
        {
            if (value == null || !value.Any())
            {
                _panels.Clear();
            }
            else
                _panels = value;
        }
    }

    [Parameter]
    public EventCallback<List<PanelDto>> PanelsChanged { get; set; }

    private List<PanelDto> _panels = new();

    private List<BDragItem> _items => _mDragZone?.Items;
    private PanelDto? _editPanel = default;
    private MDragZone _mDragZone;

    private Guid _id;
    private Guid _parentId;

    void asdasdasd(SorttableOptions option)
    {
        option.OnAdd = OnDragAdd;
        option.OnRemove = OnDragRemove;
    }

    protected override async Task OnParametersSetAsync()
    {
        /*
        //tab item
        if (ParentId != Guid.Empty && ParentId != _parentId)
        {
            if (_mDragZone != null)
                _items.Clear();
            foreach (var item in Panels)
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
                _mDragZone.Add(ToDragItem(item));
                //_items.Add(ToDragItem(item));
            }
            _parentId = ParentId;
            StateHasChanged();
        }
        else if (Id == Guid.Empty)
        {
            await PopupService.AlertAsync("Id must set", AlertTypes.Error);
        }
        if (ParentId == Guid.Empty && Id != _id)
        {
            var dto = await ApiCaller.InstrumentService.GetAsync(CurrentUserId, Id);
            if (dto != null)
            {
                Panels.Clear();
                if (_mDragZone != null)
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
                        _mDragZone.Add(ToDragItem(item));
                        //_items.Add(ToDragItem(item));
                        Panels.Add(item);
                    }
                }
                StateHasChanged();
            }
            else
            {
                await PopupService.AlertAsync("Id or Data must set", AlertTypes.Error);
            }
            //_reload = false;
        }
        _id = Id;

        */
        await base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        bool _reload = false;
        if (_parentId != ParentId)
        {
            _parentId = ParentId;
            _reload = true;
        }
        if (ParentId == Guid.Empty && _id != InstrumentId)
        {
            var dto = await ApiCaller.InstrumentService.GetAsync(CurrentUserId, InstrumentId);
            if (dto != null && dto.Panels != null)
                Panels = dto.Panels;
            _id = InstrumentId;
            _reload = true;
        }
        if (_reload)
        {
            if (_items.Any())
                _mDragZone.Clear();
            foreach (var item in Panels)
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
                _mDragZone.Add(ToDragItem(item));
                //_items.Add(ToDragItem(item));
                //Panels.Add(item);
            }
            StateHasChanged();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task<bool> ExecuteCommondAsync(OperateCommand command, params object[] values)
    {
        _editPanel = null;
        if (values != null && values.Length > 0 && values[0] is PanelDto panel)
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
                    //var findPanel = Panels.FirstOrDefault(m => m.Id == panel.Id);
                    //findPanel = panel;

                    var index = _mDragZone.GetIndex(find);
                    _mDragZone.Remove(find);
                    _mDragZone.Add(ToDragItem(panel), index);
                    //_items.Remove(find);
                    //_items.Insert(index, ToDragItem(panel));
                }
                CloseDialog();
                return true;
            }
            else if (command == OperateCommand.Update)
            {
                ReadOnly = false;
                //_editPanel = panel;
                await ShowEdit(panel);
                return true;
            }
            else if (command == OperateCommand.Remove)
            {
                if (find != null)
                {
                    _items.Remove(find);
                    Panels.RemoveAll(m => m.Id == panel.Id);
                    find.Dispose();
                    CloseDialog();
                    return true;
                }
            }
        }
        return false;
    }

    private BDragItem ToDragItem(PanelDto panel)
    {
        return new BDragItem
        {
            Id = Guid.NewGuid().ToString(),
            Attributes = new Dictionary<string, object> { { "key", panel.Id } },
            ChildContent = builder =>
            {
                builder.OpenComponent<TscPanelEdit>(0);
                builder.AddAttribute(1, "Value", panel);
                builder.AddAttribute(2, "ReadOnly", ReadOnly);
                builder.AddAttribute(3, "CallParent", new EventCallback<object[]>(this, ChildCallHandler));
                builder.AddAttribute(4, "ValueChanged", new EventCallback<PanelDto>(this, (PanelDto dto) => { panel = dto; }));
                builder.CloseComponent();
            }
        };
    }

    private async Task ShowEdit(PanelDto panel)
    {
        _editPanel = panel;
        OpenDialog();
        await Task.CompletedTask;
    }

    private async void OnDragAdd(SorttableEventArgs sorttableEventArgs)
    {
        var item = _mDragZone.DragDropService.DragItem;
        var panel = ((PanelDto)((dynamic)item.ChildContent.Target!).panel)!;
        var id = (Guid)item.Attributes["key"];

        //_items = _mDragZone.Items;
        //_items.Insert(sorttableEventArgs.NewIndex, item);
        //var index = _mDragZone.GetIndex(item);       
        Panels.Insert(sorttableEventArgs.NewIndex, panel);
        var newItem = ToDragItem(panel);
        newItem.Id = item.Id;
        _mDragZone.Items.Insert(sorttableEventArgs.NewIndex, newItem);

        await ApiCaller.PanelService.UpdateParentAsync(panel.Id, ParentId, CurrentUserId);
        await ApiCaller.PanelService.UpdateSortAsync(CurrentUserId, new UpdatePanelsSortDto { 
             InstrumentId = InstrumentId,
            ParentId = ParentId,
            PanelIds=Panels.Select(t=>t.Id).ToList()
        });

        //var item = _items.FirstOrDefault(t => t.Id == sorttableEventArgs.ItemId);
        //if (item != null)
        //{
        //    var id = (Guid)item.Attributes["key"];
        //    var panel = Panels.FirstOrDefault(t => t.Id == id);
        //    _items.Remove(item);
        //    if (panel != null)
        //        Panels.Remove(panel);
        //}
    }

    private async void OnDragRemove(SorttableEventArgs sorttableEventArgs)
    {
        var item = _mDragZone.DragDropService?.DragItem;
        if (item != null)
        {
            var id = (Guid)item.Attributes["key"];
            var panel = Panels.FirstOrDefault(t => t.Id == id);
            //_items = _mDragZone.Items;
            //_items.Remove(item);
            if (panel != null)
                Panels.Remove(panel);

            await ApiCaller.PanelService.UpdateSortAsync(CurrentUserId, new UpdatePanelsSortDto
            {
                InstrumentId = InstrumentId,
                ParentId = ParentId,
                PanelIds = Panels.Select(t => t.Id).ToList()
            });
        }
    }

    private void SetItems()
    {
        //if (_mDragZone != null)
        //    _items = _mDragZone.Items;
    }
}