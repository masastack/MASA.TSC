// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Nest.Extensions;

internal static class IElasticClientExtenstion
{
    public static async Task BulkAllAsync<T>(this IElasticClient client, IEnumerable<T> data, string indexName, ILogger logger) where T : class
    {
        int numberOfSlices = Environment.ProcessorCount;
        if (numberOfSlices <= 1)
            numberOfSlices = 2;
        int size = 1000;
        var bulkAllObservable = client.BulkAll(data, buk => buk.Index(indexName)
         .Size(size)
         .MaxDegreeOfParallelism(numberOfSlices)
         .BackOffRetries(3)
         .BackOffTime(TimeSpan.FromSeconds(10))
         .RefreshOnCompleted()
         .BufferToBulk((r, buffer) => r.IndexMany(buffer)));

        var name = typeof(T).Name;
        var waitHandle = new ManualResetEvent(false);
        ExceptionDispatchInfo? info = null;

        var scrollAllObserver = new BulkAllObserver(
            onNext: response =>
            {
                logger.LogInformation($"{name} bulk insert: Indexed {response.Page * size} with {response.Retries} retries");
            },
            onError: ex =>
            {
                logger.LogError($"{name} bulk all Error : {0}", ex);
                info = ExceptionDispatchInfo.Capture(ex);
                waitHandle.Set();
            },
            onCompleted: () =>
            {
                logger.LogInformation($"{name} bulk all successed.");
                waitHandle.Set();
            }
        );

        bulkAllObservable.Subscribe(scrollAllObserver);
        waitHandle.WaitOne();
        info?.Throw();
        await Task.CompletedTask;
    }

    public static async Task<IEnumerable<T>> ScrollAllAsync<T>(this IElasticClient client, string indexName, string scroll) where T : class
    {
        ISearchResponse<T> searchResponse;
        string scrollId = default!;
        List<T> result = new();
        bool isEnd = false;
        int pageSize = 5000;
        do
        {
            if (string.IsNullOrEmpty(scrollId))
            {
                searchResponse = await client.SearchAsync<T>(searchDescriptor => searchDescriptor.Index(indexName).Scroll(scroll).Size(pageSize));
                scrollId = searchResponse.ScrollId;
            }
            else
            {
                searchResponse = await client.ScrollAsync<T>(scroll, scrollId);
            }
            if (!searchResponse.IsValid)
                break;
            if (!searchResponse.Documents.Any() || searchResponse.Documents.Count - pageSize < 0)
                isEnd = true;
            if (searchResponse.Documents.Any())
                result.AddRange(searchResponse.Documents);
        } while (!isEnd);

        return result;
    }

    public static async Task<string> GetByMetricAsync(this IElasticClient client, string service, string url, DateTime start, DateTime end, string env)
    {
        ISearchResponse<object> searchResponse = await client.SearchAsync<object>(searchDescriptor => searchDescriptor.Index(AppSettings.Get("Masa:Elastic:TraceIndex"))
                .Query(q => q.Bool(
                    b => b.Must(
                            q1 => q1.Term(f => f.Field($"{ElasticConstant.ServiceName}.keyword").Value(service)),
                            q2 => q2.DateRange(f => f.Name(ElasticConstant.Trace.Timestamp).LessThanOrEquals(end).GreaterThanOrEquals(start)),
                            q4 => q4.Term(f => f.Field($"{ElasticSearchConst.Environment}.keyword").Value(env)),
                            q3 => url.Contains('*') ? q3.QueryString(f => f.Name($"{ElasticSearchConst.URL}").Query(url)) : q3.Term(f => f.Field($"{ElasticSearchConst.URL}.keyword").Value(url))
                            )
                ))
                .Sort(sort => sort.Script(script => script.Order(SortOrder.Descending).Script(s => s.Source("doc['EndTimestamp'].value.toEpochSecond()-doc['@timestamp'].value.toEpochSecond()")).Type("number")))
                .Size(1));

        if (!searchResponse.IsValid || !searchResponse.Documents.Any())
            return default!;
        var obj = (Dictionary<string, object>)searchResponse.Documents.First();

        foreach (var item in obj)
        {
            if (item.Key == ElasticConstant.TraceId)
                return item.Value.ToString();
        }

        return default!;
    }
}