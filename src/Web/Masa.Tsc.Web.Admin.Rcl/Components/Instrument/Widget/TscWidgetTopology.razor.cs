// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public partial class TscWidgetTopology : TscWidgetBase
{
    private PanelDto _panelValue = CreateDefault<PanelDto>();

    protected override void OnInitialized()
    {
        _panelValue.Type = PanelTypes.Topology;
        base.OnInitialized();
    }

    public override PanelDto Value
    {
        get { return _panelValue; }
        set
        {
            if (value == null)
            {
                value = CreateDefault<PanelDto>();
                value.Type = PanelTypes.Topology;
            }

            if (value.Id != _panelValue.Id || value.Sort != _panelValue.Sort)
                _panelValue = value;
        }
    }
}