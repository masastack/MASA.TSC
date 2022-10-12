// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class TraceExceptionDto
{
    [JsonPropertyName("exception.type")]
    public string Type { get; set; }

    [JsonPropertyName("exception.message")]
    public string Message { get; set; }

    [JsonPropertyName("exception.stacktrace")]
    public string StackTrace { get; set; }

    [JsonPropertyName("exception.escaped")]
    public bool Escaped { get; set; }
}