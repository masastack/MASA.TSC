// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Analysis
{
    internal static class TerminalAnalysisData
    {
        private static readonly string[] EchartsBuiltInColor =
            ["#5470c6", "#91cc75", "#fac858", "#ee6666", "#73c0de", "#3ba272", "#fc8452", "#9a60b4", "#ea7ccc"];

        private static readonly string[] KnowBrands =
        [
            "华为", "vivo", "OPPO", "荣耀", "Apple", "小米", "realme", "一加", "三星", "Motorola", "中兴", "努比亚", "黑鲨", "酷派", "联想",
            "索尼", "坚果", "华硕"
        ];

        private static readonly string[] KnowPlatforms = ["Android", "iOS", "HarmonyOS"];

        static TerminalAnalysisData()
        {
            for (int i = 0; i < KnowBrands.Length; i++)
            {
                var brand = KnowBrands[i];
                var colorIndex = i % EchartsBuiltInColor.Length;
                KnowBrandColor[brand] = EchartsBuiltInColor[colorIndex];
            }

            for (int i = 0; i < KnowPlatforms.Length; i++)
            {
                var platform = KnowPlatforms[i];
                var colorIndex = i % EchartsBuiltInColor.Length;
                KnowPlatformColor[platform] = EchartsBuiltInColor[colorIndex];
            }
        }

        internal static Dictionary<string, string> KnowBrandColor { get; } = [];

        internal static Dictionary<string, string> KnowPlatformColor { get; } = [];
    }
}