// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Clickhouse.Apm.Models.Request;

public class ExceptErrorDto
{
    public string Id { get; set; }

    public string Environment { get; set; }

    public string Project { get; set; }

    public string Service { get; set; }

    public string Type { get; set; }

    public string Message { get; set; }

    public string Comment { get; set; }

    public bool IsDeleted { get; set; }

    public string Creator { get; set; }

    public string Modifier { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime ModificationTime { get; set; }
}
