// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data.Apm;

public class TreeLineDto
{
    public string SpanId { get; set; }

    public string ParentSpanId { get; set; }

    public List<TreeLineDto> Children { get; set; }

    public bool IsClient { get; set; }   

    public string Name { get; set; }

    public string Type { get; set; }

    public string Label
    {
        get
        {
            return $"{Type} {Name} {Latency}";
        }
    }

    public string NameClass { get; set; }

    public bool Faild { get; set; } = false;

    public bool IsError
    {
        get
        {
            return ErrorCount > 0 || !string.IsNullOrEmpty(ErrorMessage);
        }
    }

    /// <summary>
    /// 多条error
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// 单条error
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// 单位毫秒
    /// </summary>
    public string Latency => ((double)Trace.Duration).FormatTime();

    public string Icon { get; set; }

    public TraceResponseDto Trace { get; set; }

    public int Left { get; set; }

    public int Right { get; set; }

    public int Process { get; set; }

    public bool Show { get; set; } = true;

    public string ShowIcon
    {
        get
        {
            return $"fa:fas fa-chevron-{(Show ? "down" : "up")}";
        }
    }

    public void SetValue(TraceResponseDto trace, DateTime start, DateTime end, int total, int[] errorStatus)
    {
        if (trace.TryParseDatabase(out var database))
        {
            IsClient = true;
            Name = $"{database.Name}";
            Icon = "fa:fas fa-database";

            var sqlKey = "db.statement";
            if (trace.Attributes.ContainsKey(sqlKey))
            {
                var regAction = @"(?<=\s*)(select|update|insert|delete)(?=\s+)";
                var sql = trace.Attributes[sqlKey].ToString();
                var action = Regex.Match(sql!, regAction, RegexOptions.IgnoreCase).Value;
                string table = "unkown";
                if (!string.IsNullOrEmpty(action))
                {
                    bool isSelect = action.Equals("select", StringComparison.CurrentCultureIgnoreCase);
                    var regTable = @$"(?<={(isSelect ? "from" : action)}\s+[\[`])\S+(?=[`\]]\s*)";
                    var regTable2 = @$"(?<={(isSelect ? "from" : action)}\s+)\S+(?=\s*)";
                    var matches = Regex.Matches(sql, regTable, RegexOptions.IgnoreCase);
                    if (matches.Count == 0) matches = Regex.Matches(sql, regTable2, RegexOptions.IgnoreCase);

                    if (matches.Count > 0)
                        table = matches[0].Value;
                }
                else
                {
                    table = database.System;
                }

                Type = $"{action} {table}";
            }

        }
        else if (trace.Attributes.ContainsKey("http.scheme"))
        {
            IsClient = trace.Kind == "SPAN_KIND_CLIENT";
            _ = trace.Attributes.TryGetValue("http.status_code", out var statusCode) || trace.Attributes.TryGetValue("http.response.status_code", out statusCode);
            _ = trace.Attributes.TryGetValue("http.method", out var method) || trace.Attributes.TryGetValue("http.request.method", out method);
            _ = trace.Attributes.TryGetValue("http.target", out var target) || trace.Attributes.TryGetValue("url.path", out target) || trace.Attributes.TryGetValue("url.full", out target) || trace.Attributes.TryGetValue("http.url", out target) || trace.Attributes.TryGetValue("http.route", out target);

            bool isMaui = trace.Attributes.ContainsKey("client.type") && trace.Attributes["client.type"].ToString() == "maui-blazor";
            bool isDapr = target!.ToString()!.StartsWith("http://127.0.0.1:3500/");
            if (isMaui)
            {
                if (trace.Attributes.ContainsKey("client.title") && trace.Attributes["client.title"].ToString()!.Length > 0)
                    Name = trace.Attributes["client.title"].ToString()!;
                else if (trace.Attributes.ContainsKey("http.target"))
                    Name = trace.Attributes["http.target"].ToString()!;
                else
                    Name = target.ToString()!;
                Icon = "fas fa-mobile";
                Type = "MAUI Client";
            }
            else
            {

                Name = $"{method} {target} ";
                if (isDapr && IsClient)
                {
                    Icon = "fas fa-grip";
                    Type = $"Dapr Client {statusCode}";
                }
                else
                {
                    Icon = "md:http";
                    Type = $"Http {(IsClient ? "Client " : "")} {statusCode}";
                }
            }

            NameClass = "font-weight-black";
            Faild = errorStatus.Contains(Convert.ToInt32(statusCode!.ToString()));
        }
        else
        {
            Name = trace.Name;
        }
        if (trace.TryParseException(out var exception))
        {
            Faild = true;
        }
        if (total == 0)
        {
            Process = 100;
        }
        else
        {
            Left = (int)Math.Floor(Math.Floor((trace.Timestamp - start).TotalMilliseconds) * 100 / total);
            Process = (int)Math.Floor(Math.Floor((trace.EndTimestamp - trace.Timestamp).TotalMilliseconds) * 100 / total);
            if (Process - 1 < 0) Process = 1;
            if (Process + Left - 100 > 0)
            {
                Left = 100 - Process;
            }
            else
            {
                Right = 100 - Left - Process;
                if (Right < 0)
                {
                    Right = 0;
                    Left = 100 - Process;
                }
            }
        }
    }
}
