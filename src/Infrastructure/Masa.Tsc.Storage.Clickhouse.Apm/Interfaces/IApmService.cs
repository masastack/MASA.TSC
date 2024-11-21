// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse.Apm;

public interface IApmService : IDisposable
{
    /// <summary>
    /// 服务列表页，服务详情页endpoints和instance公用
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PaginatedListBase<ServiceListDto>> ServicePageAsync(BaseApmRequestDto query);

    /// <summary>
    /// trace列表
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PaginatedListBase<EndpointListDto>> EndpointPageAsync(BaseApmRequestDto query);

    /// <summary>
    /// 可共用，service和endpoint公用
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<IEnumerable<ChartLineDto>> ChartDataAsync(BaseApmRequestDto query);

    /// <summary>
    /// endpoint 加载耗时分布
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<EndpointLatencyDistributionDto> EndpointLatencyDistributionAsync(ApmEndpointRequestDto query);

    /// <summary>
    /// tendpoint trace tree line
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PaginatedListBase<TraceResponseDto>> TraceLatencyDetailAsync(ApmTraceLatencyRequestDto query);

    /// <summary>
    /// 错误列表
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PaginatedListBase<ErrorMessageDto>> ErrorMessagePageAsync(ApmErrorRequestDto query);

    /// <summary>
    /// 获取trace下的错误信息统计，按照spanId
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<IEnumerable<ChartPointDto>> GetTraceErrorsAsync(ApmEndpointRequestDto query);

    Task<IEnumerable<ChartLineCountDto>> GetErrorChartAsync(ApmErrorRequestDto query);

    Task<IEnumerable<ChartLineCountDto>> GetEndpointChartAsync(ApmEndpointRequestDto query);

    Task<IEnumerable<ChartLineCountDto>> GetLogChartAsync(ApmEndpointRequestDto query);

    Task<Dictionary<string, List<string>>> GetEnvironmentServices(BaseApmRequestDto query);

    Task<PhoneModelDto> GetDeviceModelAsync(string brand, string model);

    /// <summary>
    /// 获取单个接口的tracelist
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    Task<PaginatedListBase<SimpleTraceListDto>> GetSimpleTraceListAsync(ApmTraceLatencyRequestDto query);

    Task<List<string>> GetErrorTypesAsync(BaseApmRequestDto query);

    Task<List<string>> GetStatusCodesAsync();

    Task<List<string>> GetEndpointsAsync(BaseApmRequestDto query);
}
