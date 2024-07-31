// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.Const;

internal static class TopologyConstants
{
    #region keys

    /// <summary>
    /// 当前拓朴图任务状态
    /// </summary>
    public const string TOPOLOGY_TASK_KEY = "topology_state";

    /// <summary>
    /// 每个trace的明细数据
    /// </summary>
    public const string TOPOLOGY_TRACE_KEY = $"topology_trace_{{0}}";

    public const string TOPOLOGY_SERVICES_KEY = "topology_services";

    public const string TOPOLOGY_SERVICES_RELATIONS_KEY = "topology_services_restions";

    #endregion

    #region es

    /// <summary>
    /// 拓朴图的es连接客户端名称
    /// </summary>
    public const string ES_CLINET_NAME = "masa.tsc.service.admin.topology";

    public const string SERVICE_INDEX_NAME = "masa-stack-topology-service-v1";

    public const string SERVICE_RELATION_INDEX_NAME = "masa-stack-topology-service-relations-v1";

    public const string SERVICE_STATEDATA_INDEX_NAME = "masa-stack-topology-service-statedata-v1";

    #endregion
}