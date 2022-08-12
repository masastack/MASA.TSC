// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Setting.Commands;

public class CommandHandler
{
    private readonly ISettingRepository _settingRepository;

    public CommandHandler(ISettingRepository settingRepository)
    {
        _settingRepository = settingRepository;
    }

    [EventHandler]
    public async Task SetAsync(SetSettingCommand command)
    {
        var setting = await _settingRepository.FindAsync(m => m.UserId == command.UserId);
        if (setting == null)
        {
            await _settingRepository.AddAsync(new Domain.Aggregates.Setting
            {
                Interval = command.Interval,
                IsEnable = command.IsEnable,
                Language = command.Language,
                TimeZone = command.TimeZone,
                TimeZoneOffset = command.TimeZoneOffset,
                UserId = command.UserId
            });
        }
        else
        {
            setting.Update(command.Language, command.Interval, command.IsEnable, command.TimeZone, command.TimeZoneOffset);
            await _settingRepository.UpdateAsync(setting);
        }
    }
}
