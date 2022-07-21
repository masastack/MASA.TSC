// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Admin.Rcl.Pages.Components;

public partial class TreeTable /*where T : class*/
{
    [Parameter]
    public Func<object, string> KeyFunc { get; set; }

    [Parameter]
    public Func<object, string> ParentFunc { get; set; }

    [Parameter]
    public IEnumerable<object> Items
    {
        get
        {
            return _items;
        }
        set
        {
            _items = value;
            SetDeeep();
        }
    }

    [Parameter]
    public bool Expand { get; set; } = true;

    private List<string> _header;

    [Parameter]
    public RenderFragment ChildFragment { get; set; }

    private Func<object, long> TimeUsFunc = obj =>
    {
        var dic = (Dictionary<string, object>)obj;
        dic = (Dictionary<string, object>)(dic.ContainsKey("transaction") ? dic["transaction"] : dic["span"]);
        return Convert.ToInt64(((Dictionary<string, object>)dic["duration"])["us"]);

    };

    private Func<object, DateTime> TimeFunc = obj =>
    {
        var dic = (Dictionary<string, object>)obj;
        return DateTime.Parse(dic["@timestamp"].ToString());
    };

    private IEnumerable<object> _items;

    private Dictionary<string, asdasdasdasd> _keyDeeps = new Dictionary<string, asdasdasdasd>();

    private Dictionary<string, List<string>> _dicChild = new Dictionary<string, List<string>>();

    private void asdasdasdasd()
    {
        var dd = _keyDeeps.Values.OrderBy(t => t.Time).First();
        foreach (var item in _keyDeeps)
        {
            if (item.Key == dd.Id)
            {
                item.Value.Left = "0";
                item.Value.Width = "100%";
                item.Value.Right = "0";
                continue;
            }
            var t1 = (long)Math.Floor((item.Value.Time - dd.Time).TotalMilliseconds * 1000);         
            double left = Math.Round(t1 * 1.0 / dd.Ms, 4), width = Math.Round(item.Value.Ms * 1.0 / dd.Ms, 4), right = 1 - left - width;

            if (left - 1 > 0)
            {
                left = 0;
                width = 1;
                right = 0;
            }
            else if (width - 1 > 0)
            {
                width = 1 - left;
                right = 0;
            }
           
            item.Value.Left = left.ToString("0.####%");
            item.Value.Width = width.ToString("0.####%");
            item.Value.Right = right.ToString("0.####%");
        }
    }

    private void SetDeeep()
    {
        if (Items == null || !Items.Any())
            return;
        _keyDeeps.Clear();
        _dicChild.Clear();

        var list = new List<object>();
        foreach (var item in Items)
        {
            string id = KeyFunc(item), parentId = ParentFunc(item);
            long us = TimeUsFunc(item);
            DateTime time = TimeFunc(item);

            if (_keyDeeps.ContainsKey(id))
                continue;
            list.Add(item);
            int deep = 0;

            var add = new asdasdasdasd
            {
                Id = id,
                ParentId = parentId,
                Time = time,
                Deep = deep,
                Ms = us
            };

            if (string.IsNullOrEmpty(parentId))
                _keyDeeps.Add(id, add);
            else
            {
                if (_keyDeeps.ContainsKey(parentId))
                    deep = _keyDeeps[parentId].Deep;

                deep++;

                add.Deep = deep;
                _keyDeeps.Add(id, add);

                if (_dicChild.ContainsKey(parentId))
                    _dicChild[parentId].Add(id);
                else
                    _dicChild.Add(parentId, new List<string> { id });
            }
        }
        _items = list;

        asdasdasdasd();
    }

    private string GetClass(object item)
    {
        string id = KeyFunc(item), parentId = ParentFunc(item);
        int deep = _keyDeeps[id].Deep;
        bool hasChild = _dicChild.ContainsKey(id);

        if (hasChild)//floder
        {
            if (deep == 0)//根
            {

            }
            else //子
            {

            }
        }

        return $"padding-left:{deep * 20}px";
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        //if (firstRender)
        //{
        //    SetDeeep();
        //}

        return base.OnAfterRenderAsync(firstRender);
    }
}


public class asdasdasdasd
{
    public string Id { get; set; }

    public string ParentId { get; set; }

    public int Deep { get; set; }

    public DateTime Time { get; set; }

    public long Ms { get; set; }

    public string Left { get; set; }

    public string Width { get; set; }

    public string Right { get; set; }
}
