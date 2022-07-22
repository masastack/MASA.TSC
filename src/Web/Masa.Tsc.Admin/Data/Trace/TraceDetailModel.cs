// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Data.Trace;

public class TraceDetailModel : TraceTimeUsModel
{
    public Dictionary<string, string> OverView { get; set; } = new Dictionary<string, string>();

    public Dictionary<string, object> Current { get; set; }

    public Dictionary<string, object> Attributes { get; set; } = default!;

    public Dictionary<string, object> Resources { get; set; } = new Dictionary<string, object>();

    public Dictionary<string, object> Logs { get; set; } = default!;

    public TraceDetailModel(Dictionary<string, object> value) : base(1)
    {
        SetValue(value);
    }

    private void Reset()
    {
        Current = default!;
        Attributes = new Dictionary<string, object>();
        OverView = new Dictionary<string, string>();
        Resources = default!;
        Logs = default!;
    }

    public void SetValue(Dictionary<string, object> value)
    {
        if (value == null)
        {
            Reset();
            return;
        }
        Current = value;

        var name = Current.ContainsKey("transaction") ? "transaction" : "span";
        Attributes = (Dictionary<string, object>)TscComponentBase.GetDictionaryValue(value, name);


        if (Current.ContainsKey("http"))
            Resources.Add("http", value["http"]);
        else if (Current.ContainsKey("url"))
            Resources.Add("url", value["url"]);
        else if (Current.ContainsKey("db"))
            Resources.Add("db", value["db"]);

        OverView.Add("url", TscComponentBase.GetDictionaryValue(value, $"{name}.name").ToString()!);
        var model = new TraceTimeUsModel(1)
        {
            TimeUs = Convert.ToInt64(TscComponentBase.GetDictionaryValue(value, $"{name}.duration.us"))
        };
        OverView.Add("duration", model.TimeUsString);
    }
}
