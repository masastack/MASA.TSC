// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Apps;

public class EnvironmentAppDto
{
    public string AppId { get; set; }

    public AppTypes AppType { get; set; }

    public string ProjectId { get; set; }

    public string AppDescription { get; set; }
}
