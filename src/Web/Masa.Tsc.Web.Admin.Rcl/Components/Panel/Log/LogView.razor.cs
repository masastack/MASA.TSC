// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Panel.Log;

public partial class LogView
{
    string? _oldSearch;

    [Parameter]
    public Dictionary<string, LogTree>? JsonObject { get; set; }

    [Parameter]
    public List<LogTree>? JsonArray { get; set; }

    [Parameter]
    public string? Search { get; set; }

    [Parameter]
    public bool CloseAotoExpand { get; set; }

    protected override void OnParametersSet()
    {
        //if(CloseAotoExpand is false && string.IsNullOrEmpty(Search) is false && Search != _oldSearch)
        //{
        //    _oldSearch = Search;
        //    if(JsonObject is not null) ExpandTree(JsonObject.Select(item => item.Value));
        //    else if(JsonArray is not null) ExpandTree(JsonArray);
        //}
        if (CloseAotoExpand is false && string.IsNullOrEmpty(Search) is false)
        {
            if (JsonObject is not null) ExpandTree(JsonObject.Select(item => item.Value));
            else if (JsonArray is not null) ExpandTree(JsonArray);
        }
    }

    void ExpandTree(IEnumerable<LogTree> trees)
    {
        if(trees.Any() is false) return;
        if (trees.Where(tree => tree.IsValueType && tree.Parent is not null).Any(tree => tree.ToString().Contains(Search!, StringComparison.OrdinalIgnoreCase)))
        {
            trees.First().Parent!.AutoExpanded = true;
        }
        else
        {
            var tree = trees.First().Parent;
            if (tree is not null)
            {
                tree.AutoExpanded = false;
            }
        }
            
        foreach (var tree in trees.Where(tree => tree.IsValueType is false))
        {
            if (tree.IsObject)
            {
                ExpandTree(tree.ToObject().Select(item => item.Value));
            }
            else if (tree.IsArray)
            {
                ExpandTree(tree.ToArray());
            }
        }
    }
}
