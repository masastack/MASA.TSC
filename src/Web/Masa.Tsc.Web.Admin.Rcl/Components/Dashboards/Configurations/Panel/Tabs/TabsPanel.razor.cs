﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Panel.Tabs;

public partial class TabsPanel
{
    MTabs? _tabs;
    bool _oldIsEdit;

    [Parameter]
    public UpsertTabsPanelDto Panel { get; set; }

    [CascadingParameter]
    public ConfigurationRecord ConfigurationRecord { get; set; }

    public bool IsEdit => ConfigurationRecord.IsEdit;

    [DisallowNull]
    StringNumber? CurrentTab
    {
        get => Panel.CurrentTabItem?.Id.ToString();
        set
        {
            Panel.SetCurrentTabItem(Guid.Parse(value.ToString()!));
        }
    }

    protected override void OnParametersSet()
    {      
        if(_oldIsEdit != IsEdit)
        {
            _oldIsEdit = IsEdit;
            if (_oldIsEdit)
            {
                NextTick(() =>
                {
                    _tabs?.CallSlider();
                });
            }
        }
    }

    void AddTabItem()
    {
        Panel.AddTabItem();
    }

    void CloseTabItem(UpsertPanelDto panel)
    {
        Panel.RemoveTabItem(panel);
        NextTick(() =>
        {
            _tabs?.CallSlider();
        });
    }
}
