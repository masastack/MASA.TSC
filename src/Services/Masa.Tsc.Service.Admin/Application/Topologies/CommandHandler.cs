// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Topologies;

public class CommandHandler
{
    private readonly ITraceService _traceService;
    private readonly IMultilevelCacheClient _multilevelCacheClient;
    private readonly ITraceServiceNodeRepository _traceServiceNodeRepository;
    private readonly ITraceServiceRelationRepository _traceServiceRelationRepository;
    private readonly ITraceServiceStateRepository _traceServiceStateRepository;

    private static List<TraceServiceCache> addNodes = new List<TraceServiceCache>();
    private static List<TraceServiceRelation> addRestions = new List<TraceServiceRelation>();
    private static ConcurrentQueue<TraceNodeCache> traceNodeQueue = new();
    private static bool _readComplete = false;

    public CommandHandler(ITraceService traceService,
        IMultilevelCacheClientFactory multilevelCacheClientFactory,
        ITraceServiceNodeRepository traceServiceNodeRepository,
        ITraceServiceRelationRepository traceServiceRelationRepository,
        ITraceServiceStateRepository traceServiceStateRepository)
    {
        _traceService = traceService;
        _multilevelCacheClient = multilevelCacheClientFactory.Create(MasaStackConsts.TSC_SYSTEM_SERVICE_APP_ID);
        _traceServiceNodeRepository = traceServiceNodeRepository;
        _traceServiceRelationRepository = traceServiceRelationRepository;
        _traceServiceStateRepository = traceServiceStateRepository;
    }

    [EventHandler]
    public async Task StartAsync(StartCommand command)
    {
        var stateModel = await _multilevelCacheClient.GetAsync<TaskRunStateDto>(TopologyConstants.TOPOLOGY_TASK_KEY);
        if (stateModel != null)
        {
            if (stateModel.Status == 1)
                throw new UserFriendlyException("Task has running !");
            else if (stateModel.Status == 2)
            {
                stateModel.Status = 1;
                stateModel.End = command.End;
            }
            else
            {
                if (stateModel.End > DateTime.MinValue)
                    stateModel.Start = stateModel.End;
                stateModel.End = command.End;
                stateModel.Status = 1;
            }
        }
        else
        {
            stateModel = new TaskRunStateDto()
            {
                Start = command.Start,
                End = command.End,
                Status = 1
            };
        }
        await _multilevelCacheClient.SetAsync(TopologyConstants.TOPOLOGY_TASK_KEY, stateModel);

        await GetTraceDataAsync(stateModel);

        await SaveServiceAsync();
        await SetServiceCacheAsync();
        await SaveRelationsAsync();
        await SetRelationCacheASync();
        stateModel.Status = 4;
        await _multilevelCacheClient.SetAsync(TopologyConstants.TOPOLOGY_TASK_KEY, stateModel);
    }

    private async Task GetTraceDataAsync(TaskRunStateDto taskRunState)
    {
        var query = new ElasticsearchScrollRequestDto
        {
            Page = 1,
            PageSize = 9999,
            Scroll = "5m",
            //TraceId = "af80d6fad26ec71def203d489e82f7fc"
        };
        var conditions = new List<FieldConditionDto>();

        if (taskRunState.Start > DateTime.MinValue)
            conditions.Add(new FieldConditionDto
            {
                Name = ElasticConstant.Trace.Timestamp,
                Type = ConditionTypes.Great,
                Value = taskRunState.Start,
            });
        conditions.Add(new FieldConditionDto
        {
            Name = ElasticConstant.Trace.Timestamp,
            Type = ConditionTypes.LessEqual,
            Value = taskRunState.End,
        });

        Task<List<string>> task;
        try
        {
            _readComplete = false;
            task = SaveTraceCacheAsync();
            _traceService.GetAll(query, SetTraceQueue);
        }
        finally
        {
            _readComplete = true;
        }

        var traceKeys = await task;
        await AnalysisTrace(traceKeys);
    }

    private void SetTraceQueue(IEnumerable<TraceResponseDto> result)
    {
        if (result == null || !result.Any())
            return;

        foreach (var item in result)
        {
            var type = item.GetServiceType();
            string service = item.Resource["service.name"].ToString()!, instance = item.Resource["service.instance.id"].ToString()!;
            string endpoint = "";
            bool isSuccess = !item.TryParseException(out _);
            if (type == TraceNodeTypes.Database)
            {
                _ = item.TryParseDatabase(out var database);
                //mssql\elasticsearch
                instance = item.Attributes["peer.service"].ToString()!;
                service = $"{database.System}:{instance}";
            }
            else if (item.TryParseHttp(out var http))
            {
                endpoint = http.Target ?? http.Url ?? String.Empty;
                if (http.StatusCode - 500 >= 0 && http.StatusCode - 600 < 0)
                    isSuccess = false;
            }

            var traceNode = new TraceNodeCache
            {
                TraceId = item.TraceId,
                SpanId = item.SpanId,
                ParentId = item.ParentSpanId,
                Start = item.Timestamp,
                End = item.EndTimestamp,
                Instance = instance!,
                Service = service!,
                EndPoint = endpoint,
                IsServer = item.Kind == "SPAN_KIND_SERVER",
                IsSuccess = isSuccess,
                Type = type,
            };
            traceNodeQueue.Enqueue(traceNode);
        }
    }

    private async Task<List<TraceNodeCache>> GetCacheTraceAsync(string key, TraceNodeCache addItem)
    {
        var values = await _multilevelCacheClient.GetAsync<List<TraceNodeCache>>(key);
        if (values != null)
            values.Add(addItem);
        else
            values = new List<TraceNodeCache>() { addItem };

        return values;
    }

    private async Task<List<string>> SaveTraceCacheAsync()
    {
        var dicValues = new Dictionary<string, List<TraceNodeCache>>();
        do
        {
            var hasData = traceNodeQueue.TryDequeue(out var traceCacheItem);
            if (!hasData)
            {
                if (_readComplete)
                    break;
                else
                    await Task.Delay(1000);
            }
            else
            {
                if (traceCacheItem is null)
                    continue;
                var key = string.Format(TopologyConstants.TOPOLOGY_TRACE_KEY, traceCacheItem.TraceId);
                if (dicValues.ContainsKey(key))
                    dicValues[key].Add(traceCacheItem);
                else
                    dicValues.Add(key, await GetCacheTraceAsync(key, traceCacheItem));
            }
        } while (true);

        //保存trace数据
        if (dicValues.Any())
        {
            int size = 50, start = 0, page = 1, total = dicValues.Count;
            do
            {
                var sliceDic = dicValues.Skip(start).Take(size).ToDictionary(item => item.Key, item => item.Value);
#pragma warning disable CS8620 // 由于引用类型的可为 null 性差异，实参不能用于形参。
                _multilevelCacheClient.SetList(sliceDic, new CacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20) });
#pragma warning restore CS8620 // 由于引用类型的可为 null 性差异，实参不能用于形参。
                page++;
                start += size;
            }
            while (total - start > 0);
        }

        return dicValues.Keys.ToList();
    }

    private async Task AnalysisTrace(List<string> traceIds)
    {
        //获取已有的节点数据
        var services = await _multilevelCacheClient.GetAsync<List<TraceServiceCache>>(TopologyConstants.TOPOLOGY_SERVICES_KEY);
        if (services == null)
            services = new();

        foreach (var traceId in traceIds)
        {
            var key = traceId;
            var traces = await _multilevelCacheClient.GetAsync<List<TraceNodeCache>>(key);
            if (traces == null || !traces.Any() || traces.Count - 2 < 0)
                continue;

            //检查每个节点是否在已知节点列表内
            Dictionary<string, string> dicNodes = new();

            //填充serviceid
            foreach (var traceNode in traces)
            {
                var service = services.FirstOrDefault(m => m.Service == traceNode.Service && m.Type == traceNode.Type && m.Instance == traceNode.Instance);
                if (service == null)
                {
                    lock (addNodes)
                    {
                        service = addNodes.FirstOrDefault(m => m.Service == traceNode.Service && m.Type == traceNode.Type && m.Instance == traceNode.Instance);
                        if (service == null)
                        {
                            service = new TraceServiceCache
                            {
                                Service = traceNode.Service,
                                Type = traceNode.Type,
                                Instance = traceNode.Instance,
                                Id = traceNode.Service
                            };
                            addNodes.Add(service);
                        }
                    }
                }

                traceNode.ServiceId = service.Id;
                dicNodes.Add(traceNode.SpanId, service.Id);
            }

            //设置parentId
            foreach (var traceNode in traces)
            {
                if (!string.IsNullOrEmpty(traceNode.ParentId) && dicNodes.ContainsKey(traceNode.ParentId))
                {
                    traceNode.CallServiceId = dicNodes[traceNode.ParentId];
                }
            }

            //没有一条有效关系依赖关系，全部为自我依赖
            if (!traces.Any(node => node.IsServer
                                    && !string.IsNullOrEmpty(node.ServiceId)
                                    && !string.IsNullOrEmpty(node.CallServiceId)
                                    && node.ServiceId != node.CallServiceId))
                continue;

            var serviceRelations = (await _multilevelCacheClient.GetAsync<List<TraceServiceRelation>>(TopologyConstants.TOPOLOGY_SERVICES_RELATIONS_KEY)) ?? new();
            var addStateList = new List<TraceServiceState>();
            foreach (var trace in traces)
            {
                if (!trace.IsServer || string.IsNullOrEmpty(trace.ServiceId) || string.IsNullOrEmpty(trace.CallServiceId) || trace.ServiceId == trace.CallServiceId)
                    continue;
                var relation = new TraceServiceRelation { ServiceId = trace.CallServiceId, DestServiceId = trace.ServiceId };
                if (!serviceRelations.Any(r => r.ServiceId == relation.ServiceId && r.DestServiceId == relation.DestServiceId))
                {
                    lock (addRestions)
                    {
                        if (!addRestions.Any(r => r.ServiceId == relation.ServiceId && r.DestServiceId == relation.DestServiceId))
                        {
                            addRestions.Add(relation);
                        }
                    }
                }

                var CallTrace = traces.FirstOrDefault(s => !s.IsServer && s.SpanId == trace.ParentId);
                if (CallTrace != null)
                    addStateList.Add(new TraceServiceState
                    {
                        ServiceId = CallTrace.ServiceId,
                        ServiceName = CallTrace.Service,
                        Instance = CallTrace.Instance,
                        Timestamp = CallTrace.Start,

                        DestServiceId = trace.ServiceId,
                        DestEndpint = trace.EndPoint,
                        DestInstance = trace.Instance,
                        DestServiceName = trace.Service,
                        IsSuccess = trace.IsSuccess,
                        Latency = trace.Duration,
                    });
            }

            await _traceServiceStateRepository.AddAsync(addStateList.ToArray());
        }
        await _multilevelCacheClient.RemoveAsync<List<TraceNodeCache>>(traceIds.ToArray());
    }

    private async Task SaveServiceAsync()
    {
        if (!addNodes.Any())
            return;
        await _traceServiceNodeRepository.AddAsync(addNodes.Select(item => new TraceServiceNode
        {
            Id = item.Id,
            Instance = item.Instance,
            Service = item.Service,
            Type = item.Type,
            CreateTime = DateTime.Now
        }).ToArray());
        addNodes.Clear();
    }

    /// <summary>
    /// 需要服务启动时加载
    /// </summary>
    /// <returns></returns>
    private async Task SetServiceCacheAsync()
    {
        var data = await _traceServiceNodeRepository.GetAllAsync();
        if (data != null && data.Any())
        {
            await _multilevelCacheClient.SetAsync(TopologyConstants.TOPOLOGY_SERVICES_KEY, data.Select(item => new TraceServiceCache
            {
                Id = item.Id,
                Service = item.Service,
                Instance = item.Instance,
                Type = item.Type
            }).ToList());
        }
    }

    private async Task SaveRelationsAsync()
    {
        if (!addRestions.Any())
            return;
        await _traceServiceRelationRepository.AddAsync(addRestions.ToArray());
        addRestions.Clear();
    }

    /// <summary>
    /// 需要服务启动时加载
    /// </summary>
    /// <returns></returns>
    private async Task SetRelationCacheASync()
    {
        var data = await _traceServiceRelationRepository.GetAllAsync();
        if (data != null && data.Any())
        {
            await _multilevelCacheClient.SetAsync(TopologyConstants.TOPOLOGY_SERVICES_RELATIONS_KEY, data.ToList());
        }
    }
}