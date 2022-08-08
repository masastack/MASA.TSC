// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Nest;

namespace Masa.Tsc.Service.Admin;

internal class IElasticClientExtenstion
{
    public static IAggregationContainer Aggregation(AggregationContainerDescriptor<object> aggContainer, IEnumerable<RequestFieldAggregationDto> FieldMaps, string? inertval = null, bool isDesc = true, string? sortField = null)
    {
        if (FieldMaps == null || !FieldMaps.Any())
            return aggContainer;


        foreach (var item in FieldMaps)
        {
            switch (item.AggegationType)
            {
                case AggregationTypes.Count:
                    {
                        aggContainer.ValueCount(item.Alias, agg => agg.Field(item.Name));
                    }
                    break;
                case AggregationTypes.Sum:
                    {
                        aggContainer.Sum(item.Alias, agg => agg.Field(item.Name));
                    }
                    break;
                case AggregationTypes.Avg:
                    {
                        aggContainer.Average(item.Alias, agg => agg.Field(item.Name));
                    }
                    break;
                case AggregationTypes.DistinctCount:
                    {
                        aggContainer.Cardinality(item.Alias, agg => agg.Field(item.Name));
                    }
                    break;
                case AggregationTypes.Histogram:
                    {
                        aggContainer.Histogram(item.Alias, agg => agg.Field(item.Name).Interval(Convert.ToDouble(inertval)).Order(isDesc ? HistogramOrder.KeyDescending : HistogramOrder.KeyAscending));
                    }
                    break;
                case AggregationTypes.DateHistogram:
                    {
                        aggContainer.DateHistogram(item.Alias, agg => agg.Field(item.Name).FixedInterval(new Time(inertval)).Order(isDesc ? HistogramOrder.KeyDescending : HistogramOrder.KeyAscending));
                    }
                    break;

            }
        }
        return aggContainer;
    }

    public static Dictionary<string, string>? AggResult(ISearchResponse<object> response, IEnumerable<RequestFieldAggregationDto> FieldMaps)
    {
        if (response.Aggregations == null || !response.Aggregations.Any())
            return null;

        var result = new Dictionary<string, string>();
        foreach (var item in response.Aggregations)
        {
            var find = FieldMaps.First(m => m.Alias == item.Key);
            if (find.AggegationType - AggregationTypes.DistinctCount <= 0 && item.Value is ValueAggregate value && value != null)
            {
                string tem = default!;
                if (!string.IsNullOrEmpty(value.ValueAsString))
                    tem = value.ValueAsString;
                else if (value.Value.HasValue)
                    tem = value.Value.Value.ToString();

                if (string.IsNullOrEmpty(tem))
                    continue;

                result.Add(item.Key, tem);
            }
            else if (find.AggegationType == AggregationTypes.DateHistogram && item.Value is BucketAggregate bucketAggregate)
            {
                foreach (DateHistogramBucket bucket in bucketAggregate.Items)
                {
                    result.Add(bucket.KeyAsString, (bucket.DocCount ?? 0).ToString());
                }
            }
        }
        return result;
    }
}
