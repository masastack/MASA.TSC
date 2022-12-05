// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class InstrumentService : BaseService
{
    public InstrumentService(ICaller caller, TokenProvider tokenProvider) : base(caller, "/api/Instrument", tokenProvider) { }

    public async Task AddAsync(AddInstrumentDto param) => await Caller.PostAsync($"{RootPath}", param);

    public async Task UpdateAsync(UpdateInstrumentDto param) => await Caller.PutAsync($"{RootPath}", param);

    public async Task DeleteAsync(CommonRemoveDto<Guid> param) => await Caller.DeleteAsync($"{RootPath}", param);

    public async Task<InstrumentDetailDto> GetAsync(Guid id) => (await Caller.GetAsync<InstrumentDetailDto>($"{RootPath}?id={id}"))!;

    public async Task<PaginatedListBase<InstrumentListDto>> ListAsync(int page, int size, string keyword) => (await Caller.GetAsync<PaginatedListBase<InstrumentListDto>>($"{RootPath}/list/{page}/{size}/{keyword}"))!;
}