﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Log.Models;

public class LogModel
{
    public DateTime Timestamp { get; set; }

    public Dictionary<string, LogTree> ExtensionData { get; set; }

    public LogModel(DateTime timestamp, Dictionary<string, LogTree> extensionData)
    {
        Timestamp = timestamp;
        ExtensionData = extensionData;
    }
}
