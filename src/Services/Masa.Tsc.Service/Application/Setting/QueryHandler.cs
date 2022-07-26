// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Setting;

public class QueryHandler
{
    private readonly ISettingRepository _settingRepository;
    public QueryHandler(ISettingRepository settingRepository)
    {
        _settingRepository = settingRepository;
    }

    [EventHandler]
    public async Task GetAsync(SettingQuery query)
    {
        var data = await _settingRepository.FindAsync(m => m.UserId == query.UserId);
        if (data is not null)
        {
            query.Result = new SettingDto
            {
                Interval = data.Interval,
                IsEnable = data.IsEnable,
                Language = data.Language,
                TimeZone = data.TimeZone,
                TimeZoneOffset = data.TimeZoneOffset
            };
        }
        else
        {
            query.Result = new SettingDto();
        }
    }
}