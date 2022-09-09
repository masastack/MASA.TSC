﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data.Team;

public class TeamSearchModel
{
    public DateTime? Start { get; set; }

    public DateTime? End { get; set; }

    public string Keyword { get; set; }

    public string AppId { get; set; }
}
