// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Pages.Analysis
{
    internal static class TerminalAnalysisData
    {
        internal static IReadOnlyDictionary<string, string> ADCodeName = new Dictionary<string, string>()
        {
            { "110000", "北京" },
            { "120000", "天津" },
            { "130000", "河北" },
            { "140000", "山西" },
            { "150000", "内蒙古" },
            { "210000", "辽宁" },
            { "220000", "吉林" },
            { "230000", "黑龙江" },
            { "310000", "上海" },
            { "320000", "江苏" },
            { "330000", "浙江" },
            { "340000", "安徽" },
            { "350000", "福建" },
            { "360000", "江西" },
            { "370000", "山东" },
            { "410000", "河南" },
            { "420000", "湖北" },
            { "430000", "湖南" },
            { "440000", "广东" },
            { "450000", "广西" },
            { "460000", "海南" },
            { "500000", "重庆" },
            { "510000", "四川" },
            { "520000", "贵州" },
            { "530000", "云南" },
            { "540000", "西藏" },
            { "610000", "陕西" },
            { "620000", "甘肃" },
            { "630000", "青海" },
            { "640000", "宁夏" },
            { "650000", "新疆" },
            { "710000", "台湾" },
            { "810000", "香港" },
            { "820000", "澳门" }
        };
        
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