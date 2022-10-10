// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscWidgetText
{
    public override AddPanelDto Item
    {
        get => _panelValue;
        set
        {
            if (value is null) _panelValue = new();
            else if (value is TextPanelDto) _panelValue = (TextPanelDto)value;
            else
            {
                _panelValue.Id = value.Id;
                _panelValue.ParentId = value.ParentId;
                _panelValue.InstrumentId = value.InstrumentId;
            }
            SetValue(nameof(_panelValue.Title), _panelValue.Title);
        }
    }
    private TextPanelDto _panelValue = new();

    private string _theme => ReadOnly ? "bubble" : "snow";

    private void OnValueChange(string value)
    {
        _panelValue.Title = value;
    }

    public override AddPanelDto ToPanel()
    {
        return _panelValue;
    }

    //protected override void SetParameters()
    //{
    //    if (Item != null)
    //    {
    //        _panelValue = new TextPanelDto
    //            {
    //                Type = InstrumentTypes.Text,
    //                Title = Item.Title,
    //                Height = Height?.ToString()!,
    //                Width = Width?.ToString()!,
    //                Id = Item.Id
    //            };
    //    }
    //    else
    //    {
    //        _panelValue = new();
    //    }
    //    SetValue(nameof(_panelValue.Value), _panelValue.Value);
    //}
}
