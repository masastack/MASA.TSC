// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Converters;

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
                if (item.ValueKind == JsonValueKind.Object && item.TryGetProperty(TYPE_KEY, out var propertyValue) && Enum.TryParse(propertyValue.GetRawText(), out PanelTypes type))
                {
                    var itemText = item.GetRawText();
                    switch (type)
                    {
                        case PanelTypes.Text:
                            result.Add(JsonSerializer.Deserialize<TextPanelDto>(itemText, options)!);
                            break;
                        case PanelTypes.Table:
                            result.Add(JsonSerializer.Deserialize<TablePanelDto>(itemText, options)!);
                            break;
                        case PanelTypes.Tabs:
                            result.Add(JsonSerializer.Deserialize<TabsPanelDto>(itemText, options)!);
                            break;
                        case PanelTypes.TabItem:
                            result.Add(JsonSerializer.Deserialize<TabItemPanelDto>(itemText, options)!);
                            break;
                        case PanelTypes.Chart:
                            result.Add(JsonSerializer.Deserialize<EChartPanelDto>(itemText, options)!);
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
