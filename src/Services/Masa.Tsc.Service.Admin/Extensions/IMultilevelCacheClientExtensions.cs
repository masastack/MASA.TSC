// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public static class IMultilevelCacheClientExtensions
{
    private static readonly object lockObj = new();
    private static readonly object lockReadTemplate = new();

    public static async Task<List<string>> GetAllMetricsAsync(this IMultilevelCacheClient _multilevelCacheClient, IMasaPrometheusClient _prometheusClient, ILogger? _logger = null)
    {
        List<string>? data = null;
        lock (lockObj)
        {
            int max = 3;
            do
            {
                try
                {
                    data = _multilevelCacheClient.Get<List<string>>(MetricConstants.ALL_METRICS_KEY);
                    break;
                }
                catch (Exception ex)
                {
                    _logger?.LogError("GetAllMetricsAsync", ex);
                    max--;
                    Task.Delay(10).ConfigureAwait(false).GetAwaiter().GetResult();
                }
            } while (max > 0);
        }
        if (data == null)
        {
            var result = await _prometheusClient.LabelValuesQueryAsync(new() { Lable = "__name__" });
            if (result.Status == ResultStatuses.Success)
            {
                data = result.Data?.ToList() ?? new();
            }
        }
        if (data != null)
            await _multilevelCacheClient.SetAsync(MetricConstants.ALL_METRICS_KEY, data, new CacheEntryOptions(new DateTimeOffset(System.DateTime.UtcNow.AddMinutes(5))));
        return data!;
    }

    public static string? GetMetricTemplateAsync(this IMultilevelCacheClient _multilevelCacheClient, string expression, ILogger? _logger = null)
    {
        if (string.IsNullOrEmpty(expression))
            return default;

        var key = MD5Utils.Encrypt(expression);

        lock (lockReadTemplate)
        {
            int max = 3;
            do
            {
                var cacheKey = string.Format(MetricConstants.METRIC_TEMPLATE_PREF, key);
                try
                {
                    return _multilevelCacheClient.Get<string>(cacheKey);
                }
                catch (Exception ex)
                {
                    _logger?.LogError("GetAllMetricsAsync", ex);
                    max--;
                    Task.Delay(10).ConfigureAwait(false).GetAwaiter().GetResult();
                }
            } while (max > 0);
        }

        return null;
    }

    public static void SetMetricTemplate(this IMultilevelCacheClient _multilevelCacheClient, string expression, string template, ILogger? _logger = null)
    {
        if (string.IsNullOrEmpty(expression))
            return;
        var key = MD5Utils.Encrypt(expression);
        var cacheKey = string.Format(MetricConstants.METRIC_TEMPLATE_PREF, key);
        _multilevelCacheClient.Set(cacheKey, template, new CacheEntryOptions(new DateTimeOffset(System.DateTime.UtcNow.AddDays(3))));
    }
}