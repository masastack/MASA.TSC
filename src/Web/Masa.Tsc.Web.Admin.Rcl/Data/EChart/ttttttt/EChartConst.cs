// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data.EChart;

internal static class EChartConst
{
    #region init json
    private const string BarBasicJson = @"{
""xAxis"": {
    ""type"": ""category"",
    ""data"": [""Mon"", ""Tue"", ""Wed"", ""Thu"", ""Fri"", ""Sat"", ""Sun""]
  },
  ""yAxis"": {
    ""type"": ""value""
  },
  ""series"": [
    {
      ""data"": [120, 200, 150, 80, 70, 110, 130],
      ""type"": ""bar""
    }
  ]
}";
    private const string LineBasicJson = @"{
  ""xAxis"": {
    ""type"": ""category"",
    ""data"": [""Mon"", ""Tue"", ""Wed"", ""Thu"", ""Fri"", ""Sat"", ""Sun""]
  },
  ""yAxis"": {
    ""type"": ""value""
  },
  ""series"": [
    {
      ""data"": [150, 230, 224, 218, 135, 147, 260],
      ""type"": ""line""
    }
  ]
}";
    private const string LineAreaBasicJson = @"{
  ""title"": {
    ""text"": ""Stacked Area Chart""
  },
  ""tooltip"": {
    ""trigger"": ""axis"",
    ""axisPointer"": {
      ""type"": ""cross"",
      ""label"": {
        ""backgroundColor"": ""#6a7985""
      }
    }
  },
  ""legend"": {
    ""data"": [""Email"", ""Union Ads"", ""Video Ads"", ""Direct"", ""Search Engine""]
  },
  ""toolbox"": {
    ""feature"": {
      ""saveAsImage"": {}
    }
  },
  ""grid"": {
    ""left"": ""3%"",
    ""right"": ""4%"",
    ""bottom"": ""3%"",
    ""containLabel"": true
  },
  ""xAxis"": [
    {
      ""type"": ""category"",
      ""boundaryGap"": false,
      ""data"": [""Mon"", ""Tue"", ""Wed"", ""Thu"", ""Fri"", ""Sat"", ""Sun""]
    }
  ],
  ""yAxis"": [
    {
      ""type"": ""value""
    }
  ],
  ""series"": [
    {
      ""name"": ""Email"",
      ""type"": ""line"",
      ""stack"": ""Total"",
      ""areaStyle"": {},
      ""emphasis"": {
        ""focus"": ""series""
      },
      ""data"": [120, 132, 101, 134, 90, 230, 210]
    },
    {
      ""name"": ""Union Ads"",
      ""type"": ""line"",
      ""stack"": ""Total"",
      ""areaStyle"": {},
      ""emphasis"": {
        ""focus"": ""series""
      },
      ""data"": [220, 182, 191, 234, 290, 330, 310]
    },
    {
      ""name"": ""Video Ads"",
      ""type"": ""line"",
      ""stack"": ""Total"",
      ""areaStyle"": {},
      ""emphasis"": {
        ""focus"": ""series""
      },
      ""data"": [150, 232, 201, 154, 190, 330, 410]
    },
    {
      ""name"": ""Direct"",
      ""type"": ""line"",
      ""stack"": ""Total"",
      ""areaStyle"": {},
      ""emphasis"": {
        ""focus"": ""series""
      },
      ""data"": [320, 332, 301, 334, 390, 330, 320]
    },
    {
      ""name"": ""Search Engine"",
      ""type"": ""line"",
      ""stack"": ""Total"",
      ""label"": {
        ""show"": true,
        ""position"": ""top""
      },
      ""areaStyle"": {},
      ""emphasis"": {
        ""focus"": ""series""
      },
      ""data"": [820, 932, 901, 934, 1290, 1330, 1320]
    }
  ]
}";
    private const string PieBasicJson = @"{
  ""title"": {
    ""text"": ""Referer of a Website"",
    ""subtext"": ""Fake Data"",
    ""left"": ""center""
  },
  ""tooltip"": {
    ""trigger"": ""item""
  },
  ""legend"": {
    ""orient"": ""vertical"",
    ""left"": ""left""
  },
  ""series"": [
    {
      ""name"": ""Access From"",
      ""type"": ""pie"",
      ""radius"": ""50%"",
      ""data"": [
        { ""value"": 1048, ""name"": ""Search Engine"" },
        { ""value"": 735, ""name"": ""Direct"" },
        { ""value"": 580, ""name"": ""Email"" },
        { ""value"": 484, ""name"": ""Union Ads"" },
        { ""value"": 300, ""name"": ""Video Ads"" }
      ],
      ""emphasis"": {
        ""itemStyle"": {
          ""shadowBlur"": 10,
          ""shadowOffsetX"": 0,
          ""shadowColor"": ""rgba(0, 0, 0, 0.5)""
        }
      }
    }
  ]
}";
    private const string GaugeBasicJson = @"{
  ""tooltip"": {
    ""formatter"": ""{a} <br/>{b} : {c}%""
  },
  ""series"": [
    {
      ""name"": ""Pressure"",
      ""type"": ""gauge"",
      ""detail"": {
        ""formatter"": ""{value}""
      },
      ""data"": [
        {
          ""value"": 50,
          ""name"": ""SCORE""
        }
      ]
    }
  ]
}";
    #endregion

    public static EChartType Bar { get; }
    public static EChartType Pie { get; }
    public static EChartType Line { get; }
    public static EChartType Gauge { get; }
    public static EChartType Heatmap { get; }
    public static EChartType LineArea { get; }

    static EChartConst()
    {
        Bar = new EChartType("bar", "", BarBasicJson);
        Line = new EChartType("line", "", LineBasicJson);
        LineArea = new EChartType("line-area", "", LineAreaBasicJson);
        Pie = new EChartType("pie", "", PieBasicJson);
        Gauge = new EChartType("gauge", "", GaugeBasicJson);
    }
}

internal class EChartType
{
    public EChartType(string name, string src, string json)
    {
        Name = name;
        Src = src;
        //try
        //{
        _json = JsonNode.Parse(Regex.Replace(json, @"\s", ""))!;
        //}
        //catch (Exception ex)
        //{ 

        //}
    }

    public string Name { get; private set; }

    public string Src { get; set; }

    public object Option
    {
        get { return _json; }
    }

    private readonly JsonNode _json;

    public void SetValue(string path, object value)
    {
        if (string.IsNullOrEmpty(path))
            return;
        var paths = path.Split('.');
        var target = ConvertJsonNode(value);

        int pathIndex = 0;
        var current = _json;
        do
        {
            current = SetAttr(paths[pathIndex++], current, pathIndex - paths.Length == 0 ? target : null);
            var isLastPath = pathIndex - paths.Length == 0;
            if (isLastPath)
                break;
        }
        while (true);
    }

    private JsonNode SetAttr(string name, JsonNode source, JsonNode? value)
    {
        if (source is JsonArray)
            source = NewObject();

        bool isArray = GetNameArrayAttr(name, out string newName, out int[] arrayLen);
        if (isArray)
        {
            var ddd = source[newName];
            if (ddd == null)
                ddd = NewArray(arrayLen);
            else
            {
                try
                {
                    var array = ddd.AsArray();
                    SetArrayLen(array, arrayLen);
                    ddd = array;
                }
                catch
                {
                    ddd = NewArray(arrayLen);
                }
            }
            source[newName] = ddd;
            return SetArrayValue(ddd.AsArray(), arrayLen, value);
        }
        else
        {
            if (source[name] == null)
                source[name] = NewObject();
            if (value != null)
                source[name] = value;
            else if (source[name] is JsonArray)
                source[name] = NewObject();
            return source[name]!;
        }
    }

    private void SetArrayLen(JsonArray json, int[] len)
    {
        if (len.Length == 0)
            return;

        var count = len[0] + 1 - json.Count;
        if (count > 0)
        {
            do
            {
                json.Add(NewArray(len[1..]));
            }
            while (--count > 0);
        }

        if (len.Length - 1 > 0)
            foreach (var item in json)
            {
                if (item == null)
                    continue;
                SetArrayLen(item.AsArray(), len[1..]);
            }
    }

    private JsonObject NewObject() => new();

    private JsonArray NewArray(params int[] len)
    {
        if (len.Length == 0) return new();

        var array = new JsonArray();
        var index = len[0] + 1;

        while (index-- > 0)
            array.Add(null);

        if (len.Length > 1)
        {
            array[len[0]] = NewArray(len[1..]);
        }
        else
        {
            if (array[len[0]] == null)
                array[len[0]] = NewObject();
        }

        return array;
    }

    private JsonNode SetArrayValue(JsonArray array, int[] len, JsonNode? value)
    {
        var index = 0;
        do
        {
            var find = array[len[index]]!;
            if (index - len.Length + 1 == 0)
            {
                if (value != null)
                {
                    find = value;
                    array[len[index]] = find;
                }
                return find;
            }
            index++;
            array = find.AsArray();
        }
        while (true);
    }

    private JsonNode ConvertJsonNode(object value)
    {
        var text = JsonSerializer.Serialize(value);
        return JsonNode.Parse(text)!;
    }

    /// <summary>
    /// name[0][1] return name int[]{0,1},name[3] return name int[]{3},name return name
    /// </summary>
    /// <param name="name"></param>
    /// <param name="newName"></param>
    /// <param name="indexes"></param>
    /// <returns></returns>
    private bool GetNameArrayAttr(string name, out string newName, out int[] indexes)
    {
        indexes = default!;
        newName = default!;
        var matches = Regex.Matches(name, @"\[\d+\]");
        if (!matches.Any(m => m.Success))
            return false;

        newName = Regex.Replace(name, @"\[\d+\]", "");
        indexes = matches.Where(m => m.Success).Select(m => Convert.ToInt32(m.Value[1..(m.Value.Length - 1)])).ToArray();

        return true;
    }

    public string GetValue()
    {
        return _json.ToJsonString();
    }
}