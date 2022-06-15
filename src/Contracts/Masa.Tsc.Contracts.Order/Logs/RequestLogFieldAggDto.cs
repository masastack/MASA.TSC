﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class RequestLogFieldAggDto : FromUri<RequestLogFieldAggDto>
{
    public string Name { get; set; }

    public string Alias { get; set; }

    public LogAggTypes AggType { get; set; }
}
