// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Log.Models;

public class LogTree
{
    string? stringValue;
    Dictionary<string, LogTree>? _objectValue;
    List<LogTree>? _arrayValue;

    JsonElement JsonElement { get; set; }

    public bool Expanded { get; set; }

    public bool IsValueType => JsonElement.ValueKind is JsonValueKind.String or JsonValueKind.Null or JsonValueKind.True or JsonValueKind.False or JsonValueKind.Number or JsonValueKind.Undefined;

    public bool IsObject => JsonElement.ValueKind is JsonValueKind.Object;

    public bool IsArray => JsonElement.ValueKind is JsonValueKind.Array;

    public LogTree(JsonElement jsonElement)
    {
        JsonElement = jsonElement;
    }

    public override string ToString()
    {
        return JsonElement.ValueKind switch
        {
            JsonValueKind.False or JsonValueKind.True => JsonElement.GetBoolean().ToString(),
            JsonValueKind.Number => JsonElement.GetDecimal().ToString(),
            JsonValueKind.String => JsonElement.GetString() ?? "",
            _ => "",
        };
    }

    public Dictionary<string,LogTree> ToObject()
    {
        if(_objectValue is null)
        {
            var objectEnumerator = JsonElement.EnumerateObject();
            _objectValue = objectEnumerator.ToDictionary(jsonProperty => jsonProperty.Name, jsonProperty => new LogTree(jsonProperty.Value));
        }

        return _objectValue;
    }

    public List<LogTree> ToArray()
    {
        if (_arrayValue is null)
        {
            var arrayEnumerator = JsonElement.EnumerateArray();
            _arrayValue = arrayEnumerator.Select(JsonElement => new LogTree(JsonElement)).ToList();
        }
        return _arrayValue;
    }
}
