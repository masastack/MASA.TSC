// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Prometheus;

public interface IMasaPrometheusClient
{
    Task<QueryResultCommonResponse> QueryAsync(QueryRequest query);

    Task<QueryResultCommonResponse> QueryRangeAsync(QueryRangeRequest query);

    Task<SeriesResultResponse> SeriesQueryAsync(MetaDataQueryRequest query);

    Task<LabelResultResponse> LabelsQueryAsync(MetaDataQueryRequest query);

    Task<LabelResultResponse> LabelValuesQueryAsync(LableValueQueryRequest query);

    Task<ExemplarResultResponse> ExemplarQueryAsync(QueryExemplarRequest query);

    Task<MetaResultResponse> MetricMetaQueryAsync(MetricMetaQueryRequest query);
}
