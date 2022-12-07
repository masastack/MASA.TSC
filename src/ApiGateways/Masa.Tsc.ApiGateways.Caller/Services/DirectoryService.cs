// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Tsc.Contracts.Admin.Dashboards;

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class DirectoryService : BaseService
{
    public DirectoryService(ICaller caller, TokenProvider tokenProvider) : base(caller, "/api/Instrument/directory", tokenProvider) { }

    public async Task<IEnumerable<DirectoryTreeDto>> GetTreeAsync(bool isContainsInstrument = true) => (await Caller.GetAsync<IEnumerable<DirectoryTreeDto>>($"{RootPath}/tree/{isContainsInstrument}"))!;

    public async Task<PaginatedListBase<FolderDto>> GetListAsync(int page, int pageSize, string? keyword = default, bool isIncludeInstrument = true) => (await Caller.GetAsync<PaginatedListBase<FolderDto>>($"{RootPath}/list", new { page,pageSize,keyword,isIncludeInstrument }))!;

    public async Task<UpdateFolderDto> GetAsync(Guid id) => (await Caller.GetAsync<UpdateFolderDto>($"{RootPath}?id={id}"))!;

    public async Task AddAsync(AddFolderDto param) => await Caller.PostAsync($"{RootPath}", param);

    public async Task UpdateAsync(UpdateFolderDto param) => await Caller.PutAsync($"{RootPath}", param);

    public async Task DeleteAsync(Guid id) => await Caller.DeleteAsync($"{RootPath}?id={id}", default);
}