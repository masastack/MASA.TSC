// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscInstrumentPannels
{
    [Inject]
    public AddInstrumentsDto _model { get; set; }
    private static List<PannelTypeDto> _types = new List<PannelTypeDto> {
        new PannelTypeDto(){
            Index=1,
            Name="Text",
            Key=InstrumentTypes.Text.ToString()
        },
        new PannelTypeDto(){
            Index=2,
            Name="Widget",
            Key=InstrumentTypes.Widget.ToString()
        },
        new PannelTypeDto(){
            Index =3,
            Name="Log",
            Key =InstrumentTypes.Log.ToString()
        },
        new PannelTypeDto(){
         Index =4,
            Name="Trace",
            Key =InstrumentTypes.Trace.ToString()
        }
    };

    private int _step = 1;
    private InstrumentTypes _widgetType;

    private async Task OnSubmitAsync()
    {
        await PopupService.AlertAsync("Success", AlertTypes.Success);
    }

    private async Task ValueChange(StringNumber value)
    {
        _widgetType = Enum.Parse<InstrumentTypes>(value.Value.ToString());
    }

    private async Task ValuesChange(List<StringNumber> values)
    {
        var find = _types.FirstOrDefault(m => string.Equals(m.Key.Value,values[0].Value));
        find.Checked = true;
    }
}
