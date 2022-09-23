// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscInstrumentDetail
{
    [Parameter]
    public string Id { get; set; }

    [Inject]
    public AddInstrumentDto Data { get; set; }

    private List<AddPanelDto> _panels = new();
    private List<BDragItem> _items = new();
    private AddPanelDto? _editPanel = default;
    private MDragZone _mDragZone;

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrEmpty(Id) && Data == null)
        {
            await PopupService.AlertAsync("Id or Data must set", AlertTypes.Error);
            return;
        }

        //todo load data

        if (Data == null && !string.IsNullOrEmpty(Id))
        {
            await PopupService.AlertAsync("Id or Data must set", AlertTypes.Error);
            return;
        }

        await base.OnInitializedAsync();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {

        }
        return base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task ChildCallHandler(params object[] values)
    {
        if (values != null && values.Length - 2 >= 0)
        {
            var cmd = values[0] as string;
            if (values[1] is AddPanelDto panel)
            {
                var find = _mDragZone.Items.FirstOrDefault(x => (Guid)x.Attributes["key"] == panel.Id);
                switch (cmd)
                {
                    case "save":
                        if (find == null)
                        {
                            _items.Add(ToDragItem(panel));
                            _panels.Add(panel);
                        }
                        else
                        {
                            var findPanel = _panels.FirstOrDefault(m => m.Id == panel.Id);
                            findPanel = panel;

                            var index = _items.IndexOf(find);
                            _items.Remove(find);
                            _items.Insert(index, ToDragItem(panel));
                        }
                        values = new[] { "close" };
                        break;
                    case "edit":
                        await ShowEdit(panel);
                        values = default!;
                        break;

                    case "remove":
                        if (find != null)
                        {
                            _items.Remove(find);
                            _panels.RemoveAll(m => m.Id == panel.Id);
                            find.Dispose();
                        }
                        values = new[] { "close" };
                        break;
                }
            }
        }

        if (values != null)
            _editPanel = null;
        await base.ChildCallHandler(values);
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
                builder.AddAttribute(2, "OnCallParent", new EventCallback<object[]>(this, ChildCallHandler));
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

    private async Task SaveAsync()
    {
        await Task.CompletedTask;
    }
}