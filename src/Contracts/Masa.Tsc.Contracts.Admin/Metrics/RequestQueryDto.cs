// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Metrics;

public class RequestQueryDto
{
    public Guid Id { get; set; }

    public string Query { get; set; }

    public DateTime Time { get; set; } = DateTime.Now;
}