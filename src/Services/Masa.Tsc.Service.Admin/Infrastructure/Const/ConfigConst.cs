// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.Const;

internal class ConfigConst
{
    private const string ConfigRoot = "Appsettings";

    public const string TraceErrorPort = $"{ConfigRoot}:Trace:ErrorStatus";

    public const string LogIndex = $"{ConfigRoot}:LogIndex";

    public const string TraceIndex = $"{ConfigRoot}:TraceIndex";
}
