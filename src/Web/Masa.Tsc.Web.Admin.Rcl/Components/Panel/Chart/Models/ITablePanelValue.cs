﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Chart.Models;

public interface ITablePanelValue : IPanelValue
{
    public string SystemIdentity { get; set; }

    public int ItemsPerPage { get; set; }

    public bool ShowTableHeader { get; set; }

    public bool ShowTableFooter { get; set; }

    public bool EnablePaginaton { get; set; }

    public string ColumnAlignment { get; set; }
}
