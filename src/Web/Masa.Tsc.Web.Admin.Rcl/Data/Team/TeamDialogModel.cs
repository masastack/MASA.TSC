// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data.Team;

public class TeamDialogModel
{
    public string ProjectId { get; set; }

    public Guid TeamId { get; set; }

    public int TeamProjectCount { get; set; }

    public int TeamServiceCount { get; set; }

    public DateTimeOffset Start { get; set; }

    public DateTimeOffset End { get; set; }

    public QuickRangeKey? QuickRangeKey { get; set; }
}