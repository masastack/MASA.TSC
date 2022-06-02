// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Text.Json.Serialization;

namespace Masa.Tsc.Contracts.Admin.Logs;

public class BaseLogModel
{
    [JsonPropertyName("@timestamp")]
    public DateTime Time { get; set; }
}
