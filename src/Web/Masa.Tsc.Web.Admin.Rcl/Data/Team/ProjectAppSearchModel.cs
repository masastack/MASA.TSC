// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data.Team;

public class ProjectAppSearchModel
{
    public string ProjectId { get; set; }

    public string AppId { get; set; }

    public DateTime? Start { get; set; }

    public DateTime? End { get; set; }

    public string Interval { get; set; }
}
