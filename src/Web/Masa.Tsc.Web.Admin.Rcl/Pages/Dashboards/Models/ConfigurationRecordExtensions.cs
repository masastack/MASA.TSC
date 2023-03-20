// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Models;

public static class ConfigurationRecordExtensions
{
    public static async Task BindPanelsAsync(this DashboardConfigurationRecord configurationRecord, TscCaller apiCaller)
    {
        configurationRecord.ModelType = default;
        var detail = await apiCaller.InstrumentService.GetDetailAsync(Guid.Parse(configurationRecord.DashboardId));
        if (detail is not null)
        {
            var panels = detail.Panels;
            if (panels?.Any() is true)
            {
                panels.ConvertToConfigurationFormat();
                configurationRecord.ClearPanels();
                configurationRecord.Panels.AddRange(panels);
            }
            configurationRecord.ModelType = Enum.Parse<ModelTypes>(detail.Model);
        }

        if (configurationRecord.Panels.Any() is false) configurationRecord.IsEdit = true;
    }
}
