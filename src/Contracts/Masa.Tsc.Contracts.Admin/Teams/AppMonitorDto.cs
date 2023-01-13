// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class AppMonitorDto
{
    public int ServiceTotal { get; set; }

    public int AppTotal { get; set; }

    public int ServiceError { get; set; }

    public int AppError { get; set; }

    public int ServiceWarn { get; set; }

    public int AppWarn { get; set; }

    public int Normal { get; set; }

    public long ErrorCount { get; set; }

    public long WarnCount { get; set; }
}