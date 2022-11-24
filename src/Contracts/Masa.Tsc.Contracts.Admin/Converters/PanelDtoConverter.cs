// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Converters;

public class PanelDtoConverter : JsonConverter<PanelDto>
{
    private static readonly string TYPE_KEY = nameof(PanelDto.Type).ToLower();   

    public override PanelDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (JsonDocument.TryParseValue(ref reader, out var doc))
        {
            var jsonObject = doc.RootElement;
            if (jsonObject.TryGetProperty(TYPE_KEY, out var propertyValue) && Enum.TryParse(propertyValue.GetRawText(), out PanelTypes type))
            {
                var rootText = jsonObject.GetRawText();
                switch (type)
                {
                    case PanelTypes.Text:
                        return JsonSerializer.Deserialize<TextPanelDto>(rootText, options);
                    case PanelTypes.Table:
                        return JsonSerializer.Deserialize<TablePanelDto>(rootText, options);
                    case PanelTypes.Tabs:
                        return JsonSerializer.Deserialize<TabsPanelDto>(rootText, options);
                    case PanelTypes.TabItem:
                        return JsonSerializer.Deserialize<TabItemPanelDto>(rootText, options);
                    case PanelTypes.Chart:
                        return JsonSerializer.Deserialize<EChartPanelDto>(rootText, options);
                    default:
                        return JsonSerializer.Deserialize<PanelDto>(rootText, options);
                }
            }
        }

        return default;
    }

    public override void Write(Utf8JsonWriter writer, PanelDto value, JsonSerializerOptions options)
    {
        var panel = value;
        switch (panel.Type)
        {
            case PanelTypes.Text:
                JsonSerializer.Serialize(writer, (TextPanelDto)panel, options);
                break;
            case PanelTypes.Table:
                JsonSerializer.Serialize(writer, (TablePanelDto)panel, options);
                break;
            case PanelTypes.Tabs:
                JsonSerializer.Serialize(writer, (TabsPanelDto)panel, options);
                break;
            case PanelTypes.TabItem:
                JsonSerializer.Serialize(writer, (TabItemPanelDto)panel, options);
                break;
            case PanelTypes.Chart:
                JsonSerializer.Serialize(writer, (EChartPanelDto)panel, options);
                break;
            default:
                JsonSerializer.Serialize(writer, panel, options);
                break;
        }
    }
}