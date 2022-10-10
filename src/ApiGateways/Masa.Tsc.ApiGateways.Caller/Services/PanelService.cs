// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller.Services;

public class PanelService : BaseService
{
    public PanelService(ICaller caller, TokenProvider tokenProvider) : base(caller, "/api/Instrument/panel", tokenProvider) { }

    public async Task AddAsync(AddPanelDto param)
    {
        await Caller.PostAsync($"{RootPath}", param);
    }

    public async Task UpdateAsync(UpdatePanelDto param)
    {
        await Caller.PutAsync($"{RootPath}", param);
    }

    public async Task DeleteAsync(Guid userId, Guid instrumentId, Guid id)
    {
        await Caller.DeleteAsync($"{RootPath}/{userId}/{id}/{instrumentId}", default);
    }

    public async Task<List<PanelDto>> ListAsync(Guid userId, Guid instrumentId, Guid id)
    {
        var text= (await Caller.GetAsync<string>($"{RootPath}/{userId}/{instrumentId}/{id}", default))!;
        if (!string.IsNullOrEmpty(text))
        {
            var options = new System.Text.Json.JsonSerializerOptions() {
                 PropertyNamingPolicy= System.Text.Json.JsonNamingPolicy.CamelCase
            };
            options.Converters.Add(new Contracts.Admin.Extensions.PanelDtoConverter());
            return System.Text.Json.JsonSerializer.Deserialize<List<PanelDto>>(text, options)!;
        }
        return new();
    }
}