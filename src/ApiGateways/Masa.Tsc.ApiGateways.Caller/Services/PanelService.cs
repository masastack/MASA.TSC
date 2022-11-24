// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class PanelService : BaseService
{
    public PanelService(ICaller caller, TokenProvider tokenProvider) : base(caller, "/api/Instrument/panel", tokenProvider) { }

    public async Task AddAsync(PanelDto param) => await Caller.PostAsync($"{RootPath}", param);

    public async Task UpdateAsync(PanelDto param) => await Caller.PutAsync($"{RootPath}", param);

    public async Task DeleteAsync(Guid userId, Guid instrumentId, Guid id) => await Caller.DeleteAsync($"{RootPath}/{userId}/{id}/{instrumentId}", default);

    public async Task<List<PanelDto>> ListAsync(Guid userId, Guid instrumentId, Guid id) => (await Caller.GetAsync<List<PanelDto>>($"{RootPath}/{userId}/{instrumentId}/{id}", default))!;


    public async Task UpdateParentAsync(Guid panelId, Guid parentId, Guid userId) => await Caller.PutAsync($"{RootPath}/{userId}/{panelId}/{parentId}", default);

    public async Task UpdateWidthHeightAsync(Guid panelId, Guid userId, string width, string height) => await Caller.PutAsync($"{RootPath}/{userId}/{panelId}/{width}/{height}", default);

    public async Task UpdateSortAsync(Guid userId, UpdatePanelsSortDto param) => await Caller.PutAsync($"{RootPath}/{userId}", param);
}