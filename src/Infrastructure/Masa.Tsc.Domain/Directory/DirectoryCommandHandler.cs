// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Commands;

internal class DirectoryCommandHandler
{
    private readonly IDirectoryRepository _directoryRepository;

    public DirectoryCommandHandler(IDirectoryRepository directoryRepository)
    {
        _directoryRepository = directoryRepository;
    }

    [EventHandler]
    public async Task AddAsync(AddDirectoryCommand command)
    {
        if (await _directoryRepository.GetCountAsync(t => t.Name == command.Name) > 0)
            throw new UserFriendlyException("Directory name {0} is exists", command.Name);

        await _directoryRepository.AddAsync(new Shared.Entities.Directory
        {
            Name = command.Name,
            ParentId = command.ParentId,
            UserId = command.UserId,
        });
    }

    [EventHandler]
    public async Task UpdateAsync(UpdateDirectoryCommand command)
    {
        var directory = await _directoryRepository.FindAsync(t => t.Id == command.Id);
        if (directory == null)
            throw new UserFriendlyException("Directory {0} is not exists", command.Id);

        if (await _directoryRepository.GetCountAsync(t => t.Id != directory.Id && t.Name == command.Name) > 0)
            throw new UserFriendlyException("Directory name {0} is exists", command.Name);

        directory.Update(command.Name);
        await _directoryRepository.UpdateAsync(directory);
    }

    [EventHandler]
    public async Task DeleteAsync(RemoveDirectoryCommand command)
    {
        var directory = await _directoryRepository.GetIncludeInstrumentsAsync(command.Id);
        if (directory == null)
            return;
        if (directory.Instruments != null && directory.Instruments.Any())
            throw new UserFriendlyException($"Deleted directory {0} contains instruments", directory.Name);

        await _directoryRepository.RemoveAsync(directory);
    }
}