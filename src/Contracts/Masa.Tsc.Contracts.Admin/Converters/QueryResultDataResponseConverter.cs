// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Converters;

public class QueryResultDataResponseConverter : JsonConverter<QueryResultDataResponse>
{
    public override QueryResultDataResponse? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = new QueryResultDataResponse();
        if (JsonDocument.TryParseValue(ref reader, out var doc))
        {
            var jsonObject = doc.RootElement;
            if (jsonObject.TryGetProperty(nameof(QueryResultDataResponse.ResultType), out var propertyValue) && Enum.TryParse(propertyValue.GetRawText(), out ResultTypes type))
            {
                result.ResultType = type;
                jsonObject.TryGetProperty(nameof(QueryResultDataResponse.Result), out var jsonElement);
                var dataList = new List<object>();
                foreach (var json in jsonElement.EnumerateArray())
                {
                    switch (type)
                    {
                        case ResultTypes.Matrix:
                            {
                                var model = JsonSerializer.Deserialize<QueryResultMatrixRangeResponse>(json.GetRawText(), options);
                                if (model != null && model.Metric != null)
                                {
                                    foreach (var key in model.Metric.Keys)
                                    {
                                        model.Metric[key] = ((JsonElement)model.Metric[key]).GetString()!;
                                    }
                                    model.Values = model.Values?.Select(ConvertObject)?.ToArray();
                                    dataList.Add(model);
                                }
                            }
                            break;
                        case ResultTypes.Vector:
                            {
                                var model = JsonSerializer.Deserialize<QueryResultInstantVectorResponse>(json.GetRawText(), options);
                                if (model != null && model.Metric != null)
                                {
                                    foreach (var key in model.Metric.Keys)
                                    {
                                        model.Metric[key] = ((JsonElement)model.Metric[key]).GetString()!;
                                    }
                                    model.Value = ConvertObject(model.Value!);
                                    dataList.Add(model);
                                }
                            }
                            break;
                        default:
                            dataList.Add(ConvertJson(json));
                            break;
                    }
                }
                result.Result = dataList.ToArray();
            }
        }
        return result;
    }

    private object[] ConvertJson(JsonElement json)
    {
        var result = new List<object>();
        if (json.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in json.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Number)
                    result.Add(item.GetDouble());
                else if (item.ValueKind == JsonValueKind.String)
                    result.Add(item.GetString()!);
            }
        }
        return result.ToArray();
    }

    private object[] ConvertObject(object[] values)
    {
        return new object[] { ((JsonElement)values[0]).GetDouble(), ((JsonElement)values[1]).GetString()! };
    }

    public override void Write(Utf8JsonWriter writer, QueryResultDataResponse value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
