﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public class EnumSelect<TValue> : SSelect<KeyValuePair<string, TValue>, TValue, TValue> where TValue : struct, Enum
{
    [Inject]
    public I18n I18N { get; set; } = default!;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Clearable = true;
        await base.SetParametersAsync(parameters);
        Items = Enum.GetValues<TValue>().Select(e => new KeyValuePair<string, TValue>(e.ToString(), e)).ToList();
        ItemText = kv => I18N.T(kv.Key, true);
        ItemValue = kv => kv.Value;
    }
}

