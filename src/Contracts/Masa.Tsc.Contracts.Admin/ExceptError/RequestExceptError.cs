// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class RequestAddExceptError
{
    public string Environment { get; set; }

    public string Project { get; set; }

    public string Service { get; set; }

    public string Type { get; set; }

    public string Message { get; set; }

    public string Comment { get; set; }
}

public class RequestUpdateExceptError
{
    public string Id { get; set; }

    public string Comment { get; set; }
}