// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Instruments;

public class InstrumentCommandHandler
{
    private readonly IInstrumentRepository _instrumentRepository;

    public InstrumentCommandHandler(IInstrumentRepository instrumentRepository)
    {
        _instrumentRepository = instrumentRepository;
    }

    #region add

    [EventHandler]
    public async Task AddInstrumentAsync(AddInstrumentCommand command)
    {
        await ValidateAsync(command.Data.Folder, command.Data.Name, Guid.Empty);
        var model = new Instrument
        {
            Name = command.Data.Name,
            Layer = command.Data.Layer,
            IsRoot = command.Data.IsRoot,
            DirectoryId = command.Data.Folder,
            Model = command.Data.Model.ToString(),
            Sort = command.Data.Order,
            Lable = command.Data.Type
        };
        await _instrumentRepository.AddAsync(model);
    }
    #endregion

    #region update

    [EventHandler]
    public async Task UpdateInstrumentAsync(UpdateInstrumentCommand command)
    {
        var entry = await _instrumentRepository.GetAsync(command.Data.Id, command.UserId);
        if (entry == null)
            throw new UserFriendlyException($"instrument {command.Data.Id} is not exists");
        if (!entry.EnableEdit)
            throw new UserFriendlyException(errorCode: ErrorCodes.NOT_ALLOW_EDIT);
        await ValidateAsync(command.Data.Folder, command.Data.Name, entry.Id);
        entry.Update(command.Data);
        await _instrumentRepository.UpdateAsync(entry);
    }

    [EventHandler]
    public async Task SetRootAsync(SetRootCommand command)
    {
        var instrument = await _instrumentRepository.GetAsync(command.Id, command.UserId);
        if (instrument == null)
            throw new UserFriendlyException($"instrument {command.Id} is not exists");

        instrument.SetRoot(command.IsRoot);
        await _instrumentRepository.UpdateAsync(instrument);
    }
    #endregion

    #region upsert
    [EventHandler]
    public async Task UpsertInstrumentAsync(UpInsertCommand command)
    {
        var entry = await _instrumentRepository.GetDetailAsync(command.InstumentId, command.UserId);
        if (entry == null)
            throw new UserFriendlyException($"instrument {command.InstumentId} is not exists");

        if (!entry.EnableEdit)
            throw new UserFriendlyException(errorCode: ErrorCodes.NOT_ALLOW_EDIT);

        entry.UpdatePanels(command.Data);
        await _instrumentRepository.UpdateDetailAsync(entry);
    }
    #endregion

    #region  remove

    [EventHandler]
    public async Task RemoveInstrumentAsync(RemoveInstrumentCommand command)
    {
        var list = await _instrumentRepository.GetListAsync(command.InstrumentIds, command.UserId);
        if (list == null || !list.Any())
            return;

        if (list.Any(item => !item.EnableEdit))
            throw new UserFriendlyException(errorCode: ErrorCodes.CONTAINS_NOT_ALLOW_EDIT);

        await _instrumentRepository.RemoveRangeAsync(list);
    }
    #endregion    

    private async Task ValidateAsync(Guid parentId, string name, Guid id)
    {
        var instrument = await _instrumentRepository.FindAsync(e => e.DirectoryId == parentId && e.Name == name && (id == Guid.Empty || e.Id == id));
        if (instrument != null)
        {
            throw new UserFriendlyException($"instrument {name} is exists");
        }
    }
}