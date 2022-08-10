// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Instruments;

public class DirectoryQueryHandler
{
    private readonly IDirectoryRepository _directoryRepository;

    public DirectoryQueryHandler(IDirectoryRepository directoryRepository)
    {
        _directoryRepository = directoryRepository;
    }

    [EventHandler]
    public async Task GetAsync(DirectoryQuery query)
    {
        var directory = await _directoryRepository.FindAsync(t => t.Id == query.Id && t.UserId == query.UserId);
        if (directory == null)
            throw new UserFriendlyException($"directory \"{query.Id}\" is not exists");

        query.Result = new()
        {
            Id = directory.Id,
            ParentId = directory.ParentId,
            Name = directory.Name,
            Sort = directory.Sort
        };
    }

    [EventHandler]
    public async Task GetTreeAsync(DirectoryTreeQuery query)
    {
        var list = await _directoryRepository.ToQueryable().Where(t => t.UserId == query.UserId).ToListAsync();
        if (list == null || !list.Any())
        {
            query.Result = Array.Empty<DirectoryTreeDto>();
            return;
        }

        list = list.OrderBy(t => t.ParentId).ThenBy(t => t.Sort).ToList();
        query.Result = ToTree(list, Guid.Empty);
    }

    private IEnumerable<DirectoryTreeDto> ToTree(List<Domain.Aggregates.Directory> directories, Guid parentId)
    {
        var children = directories.Where(t => t.ParentId == parentId).ToList();
        if (!children.Any())
            return default!;
        directories.RemoveAll(t => t.ParentId == parentId);
        var result = new List<DirectoryTreeDto>();
        foreach (var item in children)
        {
            var model = new DirectoryTreeDto
            {
                Id = item.Id,
                ParentId = parentId,
                Name = item.Name,
                Sort = item.Sort,
                Children = ToTree(directories, item.Id)
            };
            result.Add(model);
        }

        return result;
    }
}
