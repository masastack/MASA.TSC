// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data.Trace;

internal class TraceDetailModel : TraceTimeUsModel
{
    public Dictionary<string, string> Overview { get; set; } = new Dictionary<string, string>();

    public TraceResponseDto Current { get; set; }

    public Dictionary<string, object> Attributes { get; set; } = default!;

    public Dictionary<string, object> Resources { get; set; } = new Dictionary<string, object>();

    public Dictionary<string, object> Logs { get; set; } = default!;

    public TraceDetailModel(TraceResponseDto value) : base(1)
    {
        SetValue(value);
    }

    private void Reset()
    {
        Current = default!;
        Attributes = new Dictionary<string, object>();
        Overview = new Dictionary<string, string>();
        Resources = default!;
        Logs = default!;
    }

    public void SetValue(TraceResponseDto value)
    {
        if (value == null)
        {
            Reset();
            return;
        }
        Current = value;

        var name = Current.Name;
        Attributes = value.Attributes;
        Resources = value.Resource;

        //if (Current.IsHttp(out var traceHttpDto))
        //    Resources.Add("http", traceHttpDto);
        //else if (Current.ContainsKey("url"))
        //    Resources.Add("url", value["url"]);
        //else if (Current.ContainsKey("db"))
        //    Resources.Add("db", value["db"]);

        Overview.Add("url", value.Name);
        var model = new TraceTimeUsModel(1)
        {
            TimeUs = (long)Math.Floor((value.EndTimestamp - value.Timestamp).TotalMilliseconds)
        };
        Overview.Add("duration", model.TimeUsString);
    }
}
