// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

//public partial class TscWidgetText
//{
//    public override PanelDto Value
//    {
//        get => _panelValue;
//        set
//        {
//            if (value is null) _panelValue = new();
//            else if (value is TextPanelDto dto) _panelValue = dto;
//            else
//            {
//                _panelValue.Id = value.Id;
//                _panelValue.ParentId = value.ParentId;
//                _panelValue.InstrumentId = value.InstrumentId;
//            }
//            SetValue(nameof(_panelValue.Title), _panelValue.Title);
//        }
//    }
//    private TextPanelDto _panelValue = new();

//    private string _theme => ReadOnly ? "bubble" : "snow";

//    private void OnValueChange(string value)
//    {
//        _panelValue.Title = value;
//    }

//}