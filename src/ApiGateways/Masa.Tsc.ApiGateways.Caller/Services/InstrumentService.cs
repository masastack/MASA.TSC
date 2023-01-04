// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class InstrumentService : BaseService
{
    public InstrumentService(ICaller caller, TokenProvider tokenProvider) : base(caller, "/api/Instrument", tokenProvider) { }

    public async Task AddAsync(AddDashboardDto param) => await Caller.PostAsync($"{RootPath}", param);

    public async Task UpdateAsync(UpdateDashboardDto param) => await Caller.PutAsync($"{RootPath}", param);

    public async Task DeleteAsync(params Guid[] ids) => await Caller.DeleteAsync($"{RootPath}", new { ids });

    public async Task SetRootAsync(Guid id, bool isRoot = true) => await Caller.PostAsync($"{RootPath}/set-root/{id}/{isRoot}", default);

    public async Task UpsertPanelAsync(Guid instrumentId,params UpsertPanelDto[] panels) => await Caller.PostAsync($"{RootPath}/upsert/{instrumentId}", panels);

    public async Task<UpdateDashboardDto> GetAsync(Guid id) => (await Caller.GetAsync<UpdateDashboardDto>($"{RootPath}/{id}"))!;

    public async Task<InstrumentDetailDto> GetDetailAsync(Guid id) => (await Caller.GetAsync<InstrumentDetailDto>($"{RootPath}/detail/{id}"))!;

    public async Task<PaginatedListBase<InstrumentListDto>> ListAsync(int page, int size, string keyword) => (await Caller.GetAsync<PaginatedListBase<InstrumentListDto>>($"{RootPath}/list/{page}/{size}/{keyword}"))!;

    public async Task<LinkResultDto> GetLinkAsync(MetricValueTypes type) => (await Caller.GetAsync<LinkResultDto>($"{RootPath}/link", new { type }))!;
}