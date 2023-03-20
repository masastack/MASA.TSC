// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Panel.Chart.Models;

public interface ITablePanelValue : ITopListPanelValue
{
    public int ItemsPerPage { get; set; }

    public bool ShowTableHeader { get; set; }

    public bool ShowTableFooter { get; set; }

    public bool EnablePaginaton { get; set; }

    public string ColumnAlignment { get; set; }

    public ListTypes ListType { get; set; }

    public List<List<Dessert>> GetTableOption();

    public void SetTableOption(List<string> services, string jumpName, string jumpId);
}
