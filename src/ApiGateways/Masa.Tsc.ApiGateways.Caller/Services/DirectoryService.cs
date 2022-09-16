// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class DirectoryService : BaseService
{
    public DirectoryService(ICaller caller, TokenProvider tokenProvider) : base(caller, "/api/Instrument/directory",tokenProvider) { }

    public async Task<IEnumerable<KeyValuePair<string, string>>> AggregateAsync(RequestAggregationDto param)
    {
        return (await Caller.GetByBodyAsync<IEnumerable<KeyValuePair<string, string>>>($"{RootPath}/aggregate", param))!;
    }

    public async Task<IEnumerable<DirectoryTreeDto>> GetTreeAsync(Guid userId)
    {
        return (await Caller.GetAsync<IEnumerable<DirectoryTreeDto>>($"{RootPath}/tree/{userId}"))!;
    }

    public async Task<DirectoryDto> GetAsync(Guid userId, Guid id)
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
