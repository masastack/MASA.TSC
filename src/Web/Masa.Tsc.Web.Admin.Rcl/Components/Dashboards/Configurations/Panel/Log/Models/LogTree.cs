// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Panel.Log.Models;

public class LogTree
{
    string? _stringValue;
    Dictionary<string, LogTree>? _objectValue;
    List<LogTree>? _arrayValue;
    bool _expanded;
    bool _autoExpanded;

    JsonElement JsonElement { get; set; }

    public bool Expanded
    {
        get => _expanded || AutoExpanded;
        set
        {
            _expanded = value;
            _autoExpanded = value;
        }
    }

    public bool AutoExpanded
    {
        get => _autoExpanded;
        set
        {
            _autoExpanded = value;
            if (Parent is not null && value is true) Parent.AutoExpanded = true;
        }
    }

    public bool IsValueType => JsonElement.ValueKind is JsonValueKind.String or JsonValueKind.Null or JsonValueKind.True or JsonValueKind.False or JsonValueKind.Number or JsonValueKind.Undefined;

    public bool IsObject => JsonElement.ValueKind is JsonValueKind.Object;

    public bool IsArray => JsonElement.ValueKind is JsonValueKind.Array;

    public LogTree? Parent { get; set; }

    public LogTree(JsonElement jsonElement, LogTree? parent = null)
    {
        JsonElement = jsonElement;
        Parent = parent;
    }

    public override string ToString()
    {
        if (_stringValue is not null) return _stringValue;

        return JsonElement.ValueKind switch
        {
            JsonValueKind.False or JsonValueKind.True => JsonElement.GetBoolean().ToString(),
            JsonValueKind.Number => JsonElement.GetDecimal().ToString(),
            JsonValueKind.String => JsonElement.GetString() ?? "",
            _ => "",
        };
    }

    public MarkupString ToMarkUpString(string? search)
    {
        var text = ToString();
        if (string.IsNullOrEmpty(search) is false && string.IsNullOrEmpty(text) is false && text.Contains(search, StringComparison.OrdinalIgnoreCase))
        {
            return new MarkupString($"<mark>{text}</mark>");
        }
        else return new MarkupString(text);
    }

    public Dictionary<string, LogTree> ToObject()
    {
        if (_objectValue is null)
        {
            var objectEnumerator = JsonElement.EnumerateObject();
            _objectValue = objectEnumerator.ToDictionary(jsonProperty => jsonProperty.Name, jsonProperty => new LogTree(jsonProperty.Value, this));
        }

        return _objectValue;
    }

    public List<LogTree> ToArray()
    {
        if (_arrayValue is null)
        {
            var arrayEnumerator = JsonElement.EnumerateArray();
            _arrayValue = arrayEnumerator.Select(JsonElement => new LogTree(JsonElement, this)).ToList();
        }

        return _arrayValue;
    }
}
