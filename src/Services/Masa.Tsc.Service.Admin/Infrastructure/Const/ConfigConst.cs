// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.Const;

internal class ConfigConst
{
    public const string ConfigRoot = "Appsettings";

    private static AppSettingConfiguration Configuration;

    private readonly static int[] DefaultErrorStatus = new int[] { 500 };

    public static void SetConfiguration(AppSettingConfiguration config)
    {
        Configuration = config;
    }

    public static string LogIndex => Configuration?.LogIndex ?? "masa-stack-logs-0.6.1";

    public static string TraceIndex => Configuration?.TraceIndex ?? "masa-stack-traces-0.6.1";

    public static int[] TraceErrorStatus => Configuration?.Trace?.ErrorStatus ?? DefaultErrorStatus;

}
