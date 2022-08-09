// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class AppMonitorDto
{
    public int Total { get; set; }

    public int Error { get; set; }

    public int Warn { get; set; }

    public int Normal { get; set; }
}
