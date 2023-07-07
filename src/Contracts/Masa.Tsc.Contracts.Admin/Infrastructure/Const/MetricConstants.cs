// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Infrastructure.Const;

public sealed class MetricConstants
{
    private MetricConstants() { }

    public const string DEFAULT_LAYER = "General";

    public const string ALL_METRICS_KEY = "all_metrics";

    public const string DAPR_LAYER = "Dapr";

    public const string TIME_PERIOD = "1m";

    public const string METRIC_TEMPLATE_PREF = "metic_template_{0}";

    public const string APPEND_TEMPLATE = "{template_condition}";

    public const string Environment = "service_namespace";

    public const string BlazorEnvironment = "$BLAZORENV";
}