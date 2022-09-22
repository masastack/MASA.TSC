// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscInstrumentDetail
{
    [Parameter]
    public string Id { get; set; }

    [Inject]
    public AddInstrumentsDto Data { get; set; }

    private List<BDragItem> _items = new();
    private AddPanelDto? _editPannel = default;

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
            if (values[1] is AddPanelDto pannel)
            {
                var find = _mDragZone.Items.FirstOrDefault(x => (Guid)x.Attributes["key"] == pannel.Id);
                switch (cmd)
                {
                    case "save":
                        if (find == null)
                        {
                            _items.Add(ToDragItem(pannel));
                            if (Data.Panels == null)
                                Data.Panels = new List<AddPanelDto> { pannel };
                            else
                                Data.Panels.Add(pannel);
                        }
                        else
                        {
                            var findPannel = Data.Panels.FirstOrDefault(m => m.Id == pannel.Id);
                            findPannel = pannel;

                            var index = _items.IndexOf(find);
                            _items.Remove(find);
                            _items.Insert(index, ToDragItem(pannel));
                        }
                        values = new[] { "close" };
                        break;
                    case "edit":
                        await ShowEdit(pannel);
                        values = null;
                        break;

                    case "remove":
                        if (find != null)
                        {
                            _items.Remove(find);
                            Data.Panels.RemoveAll(m => m.Id == pannel.Id);
                            find.Dispose();
                        }
                        values = new[] { "close" };
                        break;
                }
            }
        }

        if (values != null)
            _editPannel = null;
        await base.ChildCallHandler(values);
    }

    private BDragItem ToDragItem(AddPanelDto pannel)
    {
        return new BDragItem
        {
            Id = Guid.NewGuid().ToString(),
            Attributes = new Dictionary<string, object> { { "key", pannel.Id } },
            ChildContent = builder =>
            {
                builder.OpenComponent<TscPannelEdit>(0);
                builder.AddAttribute(1, "Item", pannel);
                builder.AddAttribute(2, "OnCallParent", new EventCallback<object[]>(this, ChildCallHandler));
                builder.CloseComponent();
            }
        };
    }

    private async Task ShowEdit(AddPanelDto pannel)
    {
        _editPannel = pannel;
        OpenDialog();
        await Task.CompletedTask;
    }

    private async Task SaveAsync()
    {
        await Task.CompletedTask;
    }
}