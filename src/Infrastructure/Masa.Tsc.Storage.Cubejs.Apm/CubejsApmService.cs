namespace Masa.Tsc.Storage.Cubejs.Apm;

internal class CubejsApmService(GraphQLHttpClient client) : IApmService
{
    readonly GraphQLHttpClient _client = client;
    const double MILLSECOND = 1e6;


    public async Task<IEnumerable<ChartLineDto>> ChartDataAsync(BaseApmRequestDto query)
    {
        var result = new List<ChartLineDto>();
        bool isService = query is not ApmEndpointRequestDto;
        await GetPreviousChartData(query, isService, result, false);
        if (SetAndCheckPreviousTime(query))
        {
            await GetPreviousChartData(query, isService, result, true);
        }
        return result;
    }

    private static bool SetAndCheckPreviousTime(BaseApmRequestDto query)
    {
        if (!query.ComparisonType.HasValue)
            return false;

        int day = 0;
        switch (query.ComparisonType.Value)
        {
            case ComparisonTypes.DayBefore:
                day = -1;
                break;
            case ComparisonTypes.WeekBefore:
                day = -7;
                break;
        }
        if (day == 0)
            return false;

        query.Start = query.Start.AddDays(day);
        query.End = query.End.AddDays(day);
        return true;
    }


    private async Task GetPreviousChartData(BaseApmRequestDto query, bool isService, List<ChartLineDto> result, bool isPrevious)
    {
        if (isService)
        {
            var request = GraphQLRequestUtils.GetServiceChartRequest(query);
            var list = await _client.SendQueryAsync<CubeListData<ServiceChartItemListResponse>>(request);
            SetChartData(result, list.Data.Items.Select(item => item.Data).ToList(), isService, isPrevious);
        }
        else
        {
            var request = GraphQLRequestUtils.GetEndpointChartRequest((ApmEndpointRequestDto)query);
            var list = await _client.SendQueryAsync<CubeListData<EndpointChartItemListResponse>>(request);
            SetChartData(result, list.Data.Items.Select(item => item.Data).ToList(), isService, isPrevious);
        }
    }


    private void SetChartData(List<ChartLineDto> result, List<ChartItemResponse> data, bool isService, bool isPrevious = false)
    {
        ChartLineDto? current = null;
        var start = DateTime.Now;
        int index = 0;
        if (data == null || data.Count == 0) return;
        do
        {
            var item = data[index];
            string name;
            if (isService)//service
            {
                name = item.ServiceName;
            }
            else
            {
                name = $"{item.Method} {item.Target}";
            }
            var time = new DateTimeOffset(item.Time.Value).ToUnixTimeSeconds();
            if (current == null || current.Name != name)
            {
                if (isPrevious && result.Exists(item => item.Name == name))
                {
                    current = result.First(item => item.Name == name);
                }
                else
                {
                    current = new ChartLineDto
                    {
                        Name = name,
                        Previous = new List<ChartLineItemDto>(),
                        Currents = new List<ChartLineItemDto>()
                    };
                    result.Add(current);
                }
            }

        ((List<ChartLineItemDto>)(isPrevious ? current.Previous : current.Currents)).Add(
            new()
            {
                Latency = (long)Math.Floor(item.Latency),
                Throughput = Math.Round((double)item.Throughput, 2, MidpointRounding.ToZero),
                Failed = Math.Round(item.Failed, 2, MidpointRounding.ToZero),
                P99 = Math.Round(item.P99, 2, MidpointRounding.ToZero),
                P95 = Math.Round(item.P95, 2, MidpointRounding.ToZero),
                Time = time
            });
            index++;
        } while (data.Count - index > 0);
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    public Task<EndpointLatencyDistributionDto> EndpointLatencyDistributionAsync(ApmEndpointRequestDto query)
    {
        throw new NotImplementedException();
    }

    public async Task<PaginatedListBase<EndpointListDto>> EndpointPageAsync(BaseApmRequestDto query)
    {
        var result = new PaginatedListBase<EndpointListDto>();
        if (query.HasPage)
        {
            var pageQuery = GraphQLRequestUtils.GetEndpointListTotalRequest(query);
            var pageResult = await _client.SendQueryAsync<CubeListData<EndpointListTotalResponse>>(pageQuery);
            result.Total = pageResult.Data.Items[0].Total.Dcnt;
        }
        var request = GraphQLRequestUtils.GetEndpointListRequest(query);
        var list = await _client.SendQueryAsync<CubeListData<EndpointListResponse>>(request);
        var totalMinits = Math.Floor((query.End - query.Start).TotalMinutes);
        result.Result = list.Data.Items.Select(item => new EndpointListDto
        {
            Endpoint = item.Data.Target,
            Method = item.Data.Method,
            Service = item.Data.ServiceName,
            Envs = [item.Data.Namespace],
            Name = $"{item.Data.Method} {item.Data.Target}",
            Failed = Math.Round(item.Data.FailedAgg * 100.0, 2),
            Latency = (item.Data.Throughput == 0) ? 0 : (long)Math.Floor(item.Data.LatencyAgg / MILLSECOND),
            Throughput = item.Data.Throughput == 0 ? 0 : Math.Round(item.Data.Throughput / totalMinits, 3)
        }).ToList();
        return result;
    }

    public async Task<PaginatedListBase<ServiceListDto>> ServicePageAsync(BaseApmRequestDto query)
    {
        var result = new PaginatedListBase<ServiceListDto>();
        if (query.HasPage)
        {
            var pageQuery = GraphQLRequestUtils.GetEndpointListTotalRequest(query);
            var pageResult = await _client.SendQueryAsync<CubeListData<EndpointListTotalResponse>>(pageQuery);
            result.Total = pageResult.Data.Items[0].Total.Dcnt;
        }
        var request = GraphQLRequestUtils.GetServiceListRequest(query);
        var list = await _client.SendQueryAsync<CubeListData<EndpointListResponse>>(request);
        var totalMinits = Math.Floor((query.End - query.Start).TotalMinutes);
        result.Result = list.Data.Items.Select(item => new ServiceListDto
        {
            Service = item.Data.ServiceName,
            Envs = [item.Data.Namespace],
            Name = $"{item.Data.ServiceName}",
            Failed = Math.Round(item.Data.FailedAgg * 100.0, 2),
            Latency = (item.Data.Throughput == 0) ? 0 : (long)Math.Floor(item.Data.LatencyAgg / MILLSECOND),
            Throughput = item.Data.Throughput == 0 ? 0 : Math.Round(item.Data.Throughput / totalMinits, 3)
        }).ToList();
        return result;
    }


    public Task<PaginatedListBase<ErrorMessageDto>> ErrorMessagePageAsync(ApmErrorRequestDto query)
    {
        throw new NotImplementedException();
    }

    public Task<PhoneModelDto> GetDeviceModelAsync(string brand, string model)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ChartLineCountDto>> GetEndpointChartAsync(ApmEndpointRequestDto query)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> GetEndpointsAsync(BaseApmRequestDto query)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<string, List<string>>> GetEnvironmentServices(BaseApmRequestDto query)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ChartLineCountDto>> GetErrorChartAsync(ApmErrorRequestDto query)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> GetErrorTypesAsync(BaseApmRequestDto query)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ChartLineCountDto>> GetLogChartAsync(ApmEndpointRequestDto query)
    {
        throw new NotImplementedException();
    }

    public Task<PaginatedListBase<SimpleTraceListDto>> GetSimpleTraceListAsync(ApmTraceLatencyRequestDto query)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> GetStatusCodesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ChartPointDto>> GetTraceErrorsAsync(ApmEndpointRequestDto query)
    {
        throw new NotImplementedException();
    }

    public Task<PaginatedListBase<TraceResponseDto>> TraceLatencyDetailAsync(ApmTraceLatencyRequestDto query)
    {
        throw new NotImplementedException();
    }
}
