// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Pages.Components;

public partial class TscLog
{
    private int _lastedDuration = 1;

    private List<KeyValuePair<int, string>> _dicDurations = default!;

    private List<object> _data = default!;

    private int _totalPage =1;
}
