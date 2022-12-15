// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public class TopListOption : NotifyingEntity
{
    int _maxCount = 5;
    string _desc, _color = "#4318FF";

    public int MaxCount
    {
        get { return _maxCount; }
        set { SetField(ref _maxCount, value); }
    }

    public string Desc
    {
        get { return _desc; }
        set { SetField(ref _desc, value); }
    }

    public string Color
    {
        get { return _color; }
        set { SetField(ref _color, value); }
    }
}
