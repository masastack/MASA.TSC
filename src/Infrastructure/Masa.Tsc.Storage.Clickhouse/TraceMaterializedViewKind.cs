// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse;

/// <summary>
/// Trace 物化视图合并策略：后端主链路 SDK 与 APM 前端 SDK 使用不同 WHERE / SELECT 分支。
/// </summary>
public enum TraceMaterializedViewKind
{
    /// <summary>后端：SDK 1.5.1（旧键）与 1.9.0（OTel semconv：route/url、response.status_code 等），单条物化视图。</summary>
    BackendApi = 0,

    /// <summary>前端：1.15.1-lonsid、1.9.0（同 lonsid）、webjs 1.25.1，单条物化视图。</summary>
    ApmFrontend = 1
}
