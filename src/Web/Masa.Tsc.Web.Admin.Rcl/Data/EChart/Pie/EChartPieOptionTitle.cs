// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data
{
    public class EChartPieOptionTitle
    {
        [JsonPropertyName("text")]
        public string Title { get; set; }

        [JsonPropertyName("subtext")]
        public string SubTitle { get; set; }

        public bool? Left { get; set; } = true;

        public bool? Right { get; set; } = true;

        public bool? Top { get; set; } = true;

        public bool? Bottom { get; set; } = true;
    }
}