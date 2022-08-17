// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data.Trace;

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

    private long _timeUs;
    public long TimeUs
    {
        get { return _timeUs; }
        set
        {
            _timeUs = value;
            SetDisplay();
        }
    }

    private int _unit;
    private string _unitStr;

    public string TimeUsString
    {
        get
        {
            double duration = TimeUs * Unit * 1.0 / _unit;
            return $"{Math.Round(duration, FloorLength)}{_unitStr}";
        }
    }

    public int FloorLength { get; set; } = 3;

    private void SetDisplay()
    {
        double duration = TimeUs * Unit;
        double result = Math.Round(duration * 1.0 / MS, 3);
        if (duration - 1 < 0)
        {
            _unitStr = "us";
            _unit = 1;
            return;
        }

        duration = result;
        result = Math.Round(duration * 1.0 / S, 3);
        if (result - 1 < 0)
        {
            _unitStr = "ms";
            _unit = MS;
            return;
        }

        duration = result;
        result = Math.Round(duration * 1.0 / Min, 3);
        if (result - 1 < 0)
        {
            _unitStr = "s";
            _unit = S;
            return;
        }

        _unitStr = "min";
        _unit = Min;
    }
}
