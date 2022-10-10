// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Extensions;

public class PanelDtoConverter : JsonConverter<PanelDto>
{
    private static string TYPE_KEY = nameof(PanelDto.Type).ToLower();

    //public override bool CanConvert(Type typeToConvert)
    //{
    //    return typeToConvert.IsAssignableFrom(typeof(PanelDto));
    //}

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
        JsonSerializer.Serialize(writer, value, options);
    }
}