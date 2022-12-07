// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class PanelService : BaseService
{
    public PanelService(ICaller caller, TokenProvider tokenProvider) : base(caller, "/api/Instrument/panel", tokenProvider) { }

    public async Task AddAsync(PanelDto param) => await Caller.PostAsync($"{RootPath}/{param.InstrumentId}", param);

    public async Task UpdateAsync(PanelDto param) => await Caller.PutAsync($"{RootPath}/{param.InstrumentId}", param);

    public async Task DeleteAsync(Guid instrumentId, Guid id) => await Caller.DeleteAsync($"{RootPath}/{instrumentId}/{id}", default);

    public async Task<List<PanelDto>> ListAsync(Guid instrumentId) => (await Caller.GetAsync<List<PanelDto>>($"{RootPath}/getList/{instrumentId}", default))!;
}