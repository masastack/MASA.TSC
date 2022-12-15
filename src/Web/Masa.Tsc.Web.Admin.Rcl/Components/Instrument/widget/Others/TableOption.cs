// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

internal static class TableOption
{
    public static int ItemsPerPage { get; set; } = 10;

    public static bool ShowTableHeader { get; set; } = true;

    public static bool ShowTableFooter { get; set; }

    public static bool EnablePaginaton { get; set; }

    public static string ColumnAlignment { get; set; }
}
