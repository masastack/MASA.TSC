﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscInstrumentPanels
{
    [Parameter]
    public Guid InstrumentId { get; set; }

    [Parameter]
    public Guid ParentId { get; set; }

    private static List<PanelTypeDto> _types = new List<PanelTypeDto> {
        new PanelTypeDto(){
            Index=1,
            Name="Text",
            Key=PanelTypes.Text.ToString()
        },
        new PanelTypeDto(){
            Index=2,
            Name="Chart",
            Key=PanelTypes.Chart.ToString()
        },
        new PanelTypeDto(){
            Index=5,
            Name="Tabs",
            Key=PanelTypes.Tabs.ToString()
        },
        new PanelTypeDto(){
            Index =3,
            Name="Log",
            Key =PanelTypes.Log.ToString()
        },
        new PanelTypeDto(){
         Index =4,
            Name="Trace",
            Key =PanelTypes.Trace.ToString()
        }
    };

    private int _step = 1;
    private PanelTypes _widgetType;

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        return base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnSubmitAsync()
    {
        await PopupService.AlertAsync("Success", AlertTypes.Success);
    }

    private void ValueChange(StringNumber value)
    {
        if (value != null)
            _widgetType = Enum.Parse<PanelTypes>(value.Value.ToString()!);
        else
            _widgetType = 0;
    }

    protected override async Task<bool> ExecuteCommondAsync(OperateCommand command, params object[] values)
    {
        if (command == OperateCommand.Back)
        {
            _step = 1;
        }
        else if (command == OperateCommand.Close)
        {
            await OnCallParent(command);
            return true;
        }
        return false;
    }
}