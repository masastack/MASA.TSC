// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Extensions;

public class PanelDtoConverter : JsonConverter<PanelDto>
{
    private static readonly string TYPE_KEY = nameof(PanelDto.Type).ToLower();

    public override PanelDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (JsonDocument.TryParseValue(ref reader, out var doc))
        {
            var jsonObject = doc.RootElement;
            if (doc.RootElement.TryGetProperty(TYPE_KEY, out var propertyValue) && Enum.TryParse(propertyValue.GetRawText(), out InstrumentTypes type))
            {
                var rootText = jsonObject.GetRawText();
                switch (type)
                {
                    case InstrumentTypes.Text:
                        return JsonSerializer.Deserialize<TextPanelDto>(rootText, options);
                    case InstrumentTypes.Table:
                        return JsonSerializer.Deserialize<TablePanelDto>(rootText, options);
                    case InstrumentTypes.Tabs:
                        return JsonSerializer.Deserialize<TabsPanelDto>(rootText, options);
                    case InstrumentTypes.TabItem:
                        return JsonSerializer.Deserialize<TabItemPanelDto>(rootText, options);
                    case InstrumentTypes.Chart:
                        return JsonSerializer.Deserialize<ChartPanelDto>(rootText, options);
                    default:
                        return JsonSerializer.Deserialize<PanelDto>(rootText, options);
                }
            }
        }

        return default;
    }

    public override void Write(Utf8JsonWriter writer, PanelDto value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}

public class PanelDtoEnumerableConverter : JsonConverter<List<PanelDto>>
{
    private static readonly string TYPE_KEY = nameof(PanelDto.Type).ToLower();

    public override List<PanelDto>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (JsonDocument.TryParseValue(ref reader, out var doc) && doc.RootElement.ValueKind == JsonValueKind.Array)
        {
            var result = new List<PanelDto>();
            foreach (var item in doc.RootElement.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Object && item.TryGetProperty(TYPE_KEY, out var propertyValue) && Enum.TryParse(propertyValue.GetRawText(), out InstrumentTypes type))
                {
                    var itemText = item.GetRawText();
                    switch (type)
                    {
                        case InstrumentTypes.Text:
                            result.Add(JsonSerializer.Deserialize<TextPanelDto>(itemText, options)!);
                            break;
                        case InstrumentTypes.Table:
                            result.Add(JsonSerializer.Deserialize<TablePanelDto>(itemText, options)!);
                            break;
                        case InstrumentTypes.Tabs:
                            result.Add(JsonSerializer.Deserialize<TabsPanelDto>(itemText, options)!);
                            break;
                        case InstrumentTypes.TabItem:
                            result.Add(JsonSerializer.Deserialize<TabItemPanelDto>(itemText, options)!);
                            break;
                        case InstrumentTypes.Chart:
                            result.Add(JsonSerializer.Deserialize<ChartPanelDto>(itemText, options)!);
                            break;
                        default:
                            result.Add(JsonSerializer.Deserialize<PanelDto>(itemText, options)!);
                            break;
                    }
                }
            }
            return result;
        }

        return default;
    }

    public override void Write(Utf8JsonWriter writer, List<PanelDto> values, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        var isFirst = true;
        if (values != null && values.Any())
        {
            foreach (var item in values)
            {
                if (!isFirst) writer.WriteStringValue(",");
                else isFirst = false;
                JsonSerializer.Serialize(writer, item, item.GetType(), options);
            }
        }
        writer.WriteEndArray();
    }
}