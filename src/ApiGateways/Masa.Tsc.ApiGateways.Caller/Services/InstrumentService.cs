// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class InstrumentService : BaseService
{
    public InstrumentService(ICaller caller, TokenProvider tokenProvider) : base(caller, "/api/Instrument", tokenProvider) { }

    public async Task AddAsync(AddInstrumentDto param)
    {
        await Caller.PostAsync($"{RootPath}", param);
    }

    public async Task UpdateAsync(UpdateInstrumentDto param)
    {
        await Caller.PutAsync($"{RootPath}", param);
    }

    public async Task DeleteAsync(CommonRemoveDto<Guid> param)
    {
        await Caller.DeleteAsync($"{RootPath}", param);
    }

    public async Task<InstrumentDetailDto> GetAsync(Guid userId, Guid id)
    {
        var text = (await Caller.GetAsync<string>($"{RootPath}/{userId}/{id}"))!;
        if (!string.IsNullOrEmpty(text))
        {
            var options = new System.Text.Json.JsonSerializerOptions()
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            };
            options.Converters.Add(new Contracts.Admin.Extensions.PanelDtoConverter());
            return System.Text.Json.JsonSerializer.Deserialize<InstrumentDetailDto>(text, options)!;
        }
        return new();
    }

    public async Task<PaginationDto<InstrumentListDto>> ListAsync(Guid userId,int page,int size,string keyword)
    {
        return (await Caller.GetAsync<PaginationDto<InstrumentListDto>>($"{RootPath}/list/{userId}/{page}/{size}/{keyword}"))!;
    }   
}