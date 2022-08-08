// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl;

internal class MenuConfigs
{
    private static Dictionary<string, Tuple<string, string>> _menus = new();

    public static void Register()
    { 
        //_menus.Add(new)
    }

    //public static void Register(string text, string url, string parent)
    //{
    
    //}

    public static List<BreadcrumbItem> GetNavigations(string url)
    {
        var key = url;
        var result = new List<BreadcrumbItem>();
        while (_menus.ContainsKey(key))
        {
            result.Insert(0, new BreadcrumbItem { Text = _menus[key].Item1 });
            key = _menus[key].Item2;
            if (string.IsNullOrEmpty(key))
                break;
        };

        return result;
    }
}
