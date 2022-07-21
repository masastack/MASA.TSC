// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin;

public class RequestAttrDataDto
{
    public string Name { get; set; }

    public string Keyword { get; set; }

    public IEnumerable<KeyValuePair<string, string>> Query { get; set; }

    public int Max { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }
}
