// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Caller.Services;

public class DirectoryService : BaseService
{
    public DirectoryService(ICallerProvider caller) : base(caller, "/api/Instrument/directory") { }

    public async Task<IEnumerable<KeyValuePair<string, string>>> AggegationAsync(RequestAggregationDto param)
    {
        return (await Caller.GetByBodyAsync<IEnumerable<KeyValuePair<string, string>>>($"{RootPath}/aggregation", param))!;
    }

    public async Task<IEnumerable<DirectoryTreeDto>> GetTreeAsync(Guid userId)
    {
        return (await Caller.GetAsync<IEnumerable<DirectoryTreeDto>>($"{RootPath}/tree/{userId}"))!;
    }

    public async Task<DirectoryDto> GetAsync(Guid userId,Guid id)
    {
        return (await Caller.GetAsync<DirectoryDto>($"{RootPath}/{userId}/{id}"))!;
    }

    public async Task AddAsync(AddDirectoryDto param)
    {
        await Caller.PostAsync($"{RootPath}", param);
    }

    public async Task UpdateAsync(UpdateDirectoryDto param)
    {
        await Caller.PutAsync($"{RootPath}", param);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        await Caller.DeleteAsync($"{RootPath}/{id}/{userId}", default);
    }
}
