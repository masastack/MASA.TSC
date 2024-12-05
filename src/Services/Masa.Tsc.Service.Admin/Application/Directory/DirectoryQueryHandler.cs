// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Instruments;

internal class DirectoryQueryHandler
{
    private readonly IDirectoryRepository _directoryRepository;
    private readonly IInstrumentRepository _instrumentRepository;

    public DirectoryQueryHandler(IDirectoryRepository directoryRepository, IInstrumentRepository instrumentRepository)
    {
        _directoryRepository = directoryRepository;
        _instrumentRepository = instrumentRepository;
    }

    [EventHandler]
    public async Task GetListAsync(DirectoryListQuery query)
    {
        var data = await _directoryRepository.GetListIncludeInstrumentsAsync(query.UserId, query.Page, query.PageSize, query.Keyword, query.IsIncludeInstrument);
        if (data.Item2 == null || !data.Item2.Any())
            query.Result = new PaginatedListBase<FolderDto>();
        else
            query.Result = new PaginatedListBase<FolderDto>
            {
                Result = data.Item2.Select(directory => new FolderDto
                {
                    Id = directory.Id,
                    Name = directory.Name,
                    Dashboards = directory.Instruments?.Select(instrument => new DashboardDto
                    {
                        Id = instrument.Id,
                        Name = instrument.Name,
                        IsRoot = instrument.IsRoot,
                        Layer = instrument.Layer,
                        Model = Enum.Parse<ModelTypes>(instrument.Model),
                        Type = instrument.Lable
                    })?.ToList()!
                }).ToList(),
                Total = data.Item1
            };
    }

    [EventHandler]
    public async Task GetAsync(DirectoryQuery query)
    {
        var directory = await _directoryRepository.FindAsync(m => m.Id == query.Id);
        if (directory == null)
            throw new UserFriendlyException($"directory {query.Id} is not exists");

        query.Result = new()
        {
            Id = directory.Id,
            Name = directory.Name
        };
    }

    [EventHandler]
    public async Task GetTreeAsync(DirectoryTreeQuery query)
    {
        var list = (await _directoryRepository.GetListAsync(t => t.UserId == Guid.Empty || t.UserId == query.UserId)).ToList();
        if (list == null || !list.Any())
        {
            query.Result = Array.Empty<DirectoryTreeDto>();
            return;
        }
        list = list.OrderBy(t => t.ParentId).ThenBy(t => t.Name).ToList();
        query.Result = ToTree(list, Guid.Empty);
        if (query.IsContainsInstrument)
        {
            var instruments = (await _instrumentRepository.GetListAsync(t => t.Creator == Guid.Empty || t.Creator == query.UserId)).ToList();
            if (instruments != null && instruments.Any())
            {
                var dic = instruments.GroupBy(item => item.DirectoryId).Select(item => new
                {
                    Key = item.Key,
                    Values = item.Select(it => new DirectoryTreeDto
                    {
                        Name = it.Name,
                        DirectoryType = DirectoryTypes.Instrument,
                        Id = it.Id,
                        ParentId = item.Key,
                        Sort = it.Sort
                    }).ToList()
                }).ToDictionary(item => item.Key, item => item.Values);
                foreach (var item in query.Result)
                {
                    AppendTree(item, dic);
                }
            }
        }
    }

    private IEnumerable<DirectoryTreeDto> ToTree(List<Domain.Instruments.Aggregates.Directory> directories, Guid parentId)
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
                //Sort = item.Sort,
                DirectoryType = DirectoryTypes.Directory,
                Children = ToTree(directories, item.Id)
            };
            result.Add(model);
        }

        return result;
    }

    private void AppendTree(DirectoryTreeDto directory, Dictionary<Guid, List<DirectoryTreeDto>> instruments)
    {
        var children = directory.Children;
        var hasChild = children != null && children.Any();

        var key = directory.Id;
        if (instruments.ContainsKey(key))
        {
            var values = instruments[key];
            instruments.Remove(key);
            if (hasChild)
                values.InsertRange(0, children!);
            directory.Children = values;
        }

        if (!hasChild) return;
        foreach (var item in children!)
        {
            AppendTree(item, instruments);
        }
    }
}
