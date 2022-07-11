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
    public async Task SetSettingAsync(SetSettingCommand command)
    {
        var find = await _settingRepository.FindAsync(m => m.UserId == command.Setting.UserId);
        if (find == null)
        {
            await _settingRepository.AddAsync(new Domain.Setting.Aggregates.Setting
            {
                Interval = command.Setting.Interval,
                IsEnable = command.Setting.IsEnable,
                Language = command.Setting.Langauge,
                TimeZone = command.Setting.TimeZone,
                TimeZoneOffset = command.Setting.TimeZoneOffset,
                UserId = command.Setting.UserId
            });
        }
        else
        {
            find.Update(command.Setting.Langauge, command.Setting.Interval, command.Setting.IsEnable, command.Setting.TimeZone, command.Setting.TimeZoneOffset);
            await _settingRepository.UpdateAsync(find);
        }
    }
}
