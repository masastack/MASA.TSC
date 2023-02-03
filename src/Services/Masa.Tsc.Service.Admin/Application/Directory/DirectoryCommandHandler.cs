// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Instruments;

public class DirectoryCommandHandler
{
    private readonly IDirectoryRepository _directoryRepository;

    public DirectoryCommandHandler(IDirectoryRepository directoryRepository)
    {
        _directoryRepository = directoryRepository;
    }

    [EventHandler]
    public async Task AddAsync(AddDirectoryCommand command)
    {
        if (await _directoryRepository.ToQueryable().AnyAsync(t => t.UserId == command.UserId && t.Name == command.Name))
            throw new UserFriendlyException($"Directory name \"{command.Name}\" is exists");

        await _directoryRepository.AddAsync(new Domain.Aggregates.Directory
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
            throw new UserFriendlyException($"Directory \"{command.Id}\" is not exists");

        //if (directory.UserId != command.UserId)
        //    throw new UserFriendlyException($"No permission");

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
            throw new UserFriendlyException($"directory {directory.Name} contains instruments");
        //if (directory.UserId != command.UserId)
        //    throw new UserFriendlyException($"No permission");

        await _directoryRepository.RemoveAsync(directory);
    }
}