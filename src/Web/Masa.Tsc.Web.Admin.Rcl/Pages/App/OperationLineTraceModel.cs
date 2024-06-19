// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.App;

public class OperationLineTraceModel
{
    public OperationLineTraceModel(TraceResponseDto trace)
    {
        Data = trace;
    }

    public DateTime Time => Data.Timestamp;

    public string Text
    {
        get
        {
            if (Data.Attributes.TryGetValue("client.title", out var value) && !string.IsNullOrEmpty(value?.ToString()))
            {
                return value.ToString()!;
            }
            return Url;
        }
    }

    public string Url
    {
        get
        {
            if (Data.Attributes.TryGetValue("client.path", out var path) && !string.IsNullOrEmpty(path?.ToString()))
            {
                return path.ToString()!;
            }

            if (Data.Attributes.TryGetValue("client.path.route", out var url) && !string.IsNullOrEmpty(url?.ToString()))
            {
                return url.ToString()!;
            }
            return "未匹配到路由";
        }
    }

    public string? ToUrl => Data.Attributes.TryGetValue("to.path", out var value) ? value.ToString() : default;

    public TraceResponseDto Data { get; }

    /// <summary>
    /// 一定是log
    /// </summary>
    public List<OperationLineLogModel> Children { get; set; }

    /// <summary>
    /// log数据，为快速查找错误做准备
    /// </summary>
    public List<LogResponseDto> Logs { get; set; } = new();

    public bool IsError => Logs.Exists(log => log.Attributes.ContainsKey("exception.type"));
}

public class OperationLineLogModel
{
    public OperationLineLogModel(LogResponseDto log)
    {
        Data = log;
    }

    public DateTime Time => Data.Timestamp;

    public string Text
    {
        get
        {
            if (Data.Attributes.TryGetValue("Label", out var label) && !string.IsNullOrEmpty(label?.ToString()))
                return label.ToString()!;
            if (Data.Attributes.TryGetValue("Name", out var name) && !string.IsNullOrEmpty(name?.ToString()))
                return name.ToString()!;
            return "unkown";
        }
    }

    public bool IsError => Data.Attributes.TryGetValue("exception.type", out var value) && !string.IsNullOrEmpty(value?.ToString());

    public LogResponseDto Data { get; }
}