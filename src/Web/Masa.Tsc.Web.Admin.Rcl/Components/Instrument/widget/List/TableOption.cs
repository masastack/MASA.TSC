// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public class TableOption : NotifyingEntity
{
    int _itemsPerPage = 10;
    bool _showTableHeader = true, _showTableFooter, _enablePaginaton;
    string _columnAlignment;

    public int ItemsPerPage
    {
        get { return _itemsPerPage; }
        set { SetField(ref _itemsPerPage, value); }
    }

    public bool ShowTableHeader
    {
        get { return _showTableHeader; }
        set { SetField(ref _showTableHeader, value); }
    }

    public bool ShowTableFooter
    {
        get { return _showTableFooter; }
        set { SetField(ref _showTableFooter, value); }
    }

    public bool EnablePaginaton
    {
        get { return _enablePaginaton; }
        set { SetField(ref _enablePaginaton, value); }
    }

    public string ColumnAlignment
    {
        get { return _columnAlignment; }
        set { SetField(ref _columnAlignment, value); }
    }
}
