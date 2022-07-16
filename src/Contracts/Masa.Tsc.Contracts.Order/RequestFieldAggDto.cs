// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class RequestFieldAggDto : FromUri<RequestFieldAggDto>
{
    public string Name { get; set; }

    public string Alias { get; set; }

    public AggTypes AggType { get; set; }
}
