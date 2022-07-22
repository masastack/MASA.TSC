// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Data.Trace;

public class TraceTimeUsModel
{
    private const int MS = 1000;
    private const int S = 1000_1000;
    private const int Min = 60_000_000;

    public TraceTimeUsModel(int unit)
    {
        Unit = unit;
    }

    public int Unit { get; private set; } = 1;

    public long TimeUs { get; set; }


    public string TimeUsString
    {
        get
        {
            double duration = TimeUs * Unit;
            double result = Math.Round(duration * 1.0 / MS, 3);
            if (duration - 1 < 0)
                return $"{duration}us";

            duration = result;
            result = Math.Round(duration * 1.0 / S, 3);
            if (result - 1 < 0)
                return $"{duration}ms";

            duration = result;
            result = Math.Round(duration * 1.0 / Min, 3);
            if (result - 1 < 0)
                return $"{duration}s";

            //result = Math.Round(duration * 1.0 / Min, 3);
            //if (result - MS < 0)
            return $"{result}min";
        }
    }
}
