// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Nest;

internal static class IElasticClientExtenstion
{
    /// <summary>
    /// 批量数据插入
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="client"></param>
    /// <param name="data"></param>
    /// <param name="indexName"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 数据量少量情况下可使用，量较多时禁用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="client"></param>
    /// <param name="indexName"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<T>> ScrollAllAsync<T>(this IElasticClient client, string indexName, string scroll) where T : class
    {
        int numberOfSlices = Environment.ProcessorCount;
        if (numberOfSlices <= 1)
            numberOfSlices = 2;
        var scrollAllObservable = client.ScrollAll<T>(scroll, numberOfSlices, sc => sc
           .MaxDegreeOfParallelism(numberOfSlices)
           .Search(s => s.Index(indexName))
       );

        var waitHandle = new ManualResetEvent(false);
        ExceptionDispatchInfo? info = null;

        var result = new List<T>();
        var scrollAllObserver = new ScrollAllObserver<T>(
            onNext: response => result.AddRange(response.SearchResponse.Documents),
            onError: e =>
            {
                info = ExceptionDispatchInfo.Capture(e);
                waitHandle.Set();
            },
            onCompleted: () => waitHandle.Set()
        );

        scrollAllObservable.Subscribe(scrollAllObserver);
        waitHandle.WaitOne();
        info?.Throw();
        await Task.CompletedTask;
        return result;
    }
}