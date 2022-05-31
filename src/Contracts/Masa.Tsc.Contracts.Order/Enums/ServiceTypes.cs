// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.ComponentModel;

namespace Masa.Tsc.Contracts.Admin.Enums;

public enum ServiceTypes
{
    [Description("Dapr")]
    Dapr = 1,
    [Description("Web Api")]
    WebApi
}
