// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Aggregates;

public class Project : Entity<string>
{

    public string Identity { get; set; }

    public string Name { get; set; }

    public string LabelName { get; set; }

    public string Description { get; set; }

    public List<App> Apps { get; set; }
}
