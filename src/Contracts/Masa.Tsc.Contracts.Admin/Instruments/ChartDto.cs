// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class ChartDto
{
    public IEnumerable<string> Labels { get; set; }

    public IEnumerable<string> XPoints { get; set; }

    public IEnumerable<KeyValuePair<string, IEnumerable<string>>> YPoints { get; set; }
}
