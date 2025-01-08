// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Analysis
{
    public static class TerminalAnalysisData
    {
        private const string KnowBrandColorJson
            = """
              {
                  "华为": "#ff4d4f",
                  "vivo": "#66b3ff",
                  "OPPO": "#66ff66",
                  "荣耀": "#99ccff",
                  "Apple": "#333333",
                  "小米": "#ff9f00",
                  "realme": "#ffe74c",
                  "一加": "#ff6b6b",
                  "三星": "#6699ff",
                  "Motorola": "#cd5c5c",
                  "中兴": "#6699ff",
                  "努比亚": "#ff6666",
                  "黑鲨": "#4d4d4d",
                  "酷派": "#6699ff",
                  "联想": "#4d4d4d",
                  "索尼": "#4d4d4d",
                  "坚果": "#ff6666",
                  "华硕": "#ff6666"
              }
              """;

        private const string KnowPlatformColorJson
            = """
              {
                "Android": "#c9d89a",
                "iOS": "#333333",
                "HarmonyOS": "#66a3ff"
              }
              """;

        public static IReadOnlyDictionary<string, string> KnowBrandColor { get; } =
            JsonSerializer.Deserialize<Dictionary<string, string>>(KnowBrandColorJson)!;

        public static IReadOnlyDictionary<string, string> KnowPlatformColor { get; } =
            JsonSerializer.Deserialize<Dictionary<string, string>>(KnowPlatformColorJson)!;
    }
}