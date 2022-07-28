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
            throw new UserFriendlyException($"directory name \"{command.Name}\" is exists");

        await _directoryRepository.AddAsync(new Domain.Aggregates.Directory
        {
            Name = command.Name,
            ParentId = command.ParentId,
            Sort = command.Sort,
            UserId = command.UserId,
        });
    }

    [EventHandler]
    public async Task UpdateAsync(UpdateDirectoryCommand command)
    {
        var find = await _directoryRepository.FindAsync(t => t.Id == command.Id && t.UserId == command.UserId);
        if (find == null)
            throw new UserFriendlyException($"directory \"{command.Id}\" is not exists");

        if (command.Name == find.Name && command.Sort - find.Sort == 0)
            return;

        if (await _directoryRepository.ToQueryable().Where(t => t.UserId == command.UserId && t.Name == command.Name).AnyAsync(t => t.Id != command.Id))
            throw new UserFriendlyException($"directory name \"{command.Name}\" is exists");

        find.Name = command.Name;
        find.Sort = command.Sort;

        await _directoryRepository.UpdateAsync(find);
    }

    [EventHandler]
    public async Task DeleteAsync(RemoveDirectoryCommand command)
    {
        var find = await _directoryRepository.FindAsync(t => t.Id == command.Id);
        if (find == null)
            return;
        if (find.UserId != command.UserId)
            throw new UserFriendlyException($"no permission");

        await _directoryRepository.RemoveAsync(find);
    }
}
