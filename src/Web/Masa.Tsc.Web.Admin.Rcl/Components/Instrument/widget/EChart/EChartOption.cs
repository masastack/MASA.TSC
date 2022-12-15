// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

internal static class EChartOption
{
    public static EChartType EChartType { get; set; }

    public static Tooltip Tooltip { get; set; } = new();

    public static Legend Legend { get; set; } = new();

    public static Toolbox Toolbox { get; set; } = new();

    public static Axis XAxis { get; set; } = new();

    public static Axis YAxis { get; set; } = new();

    public static Title Title { get; set; } = new();

    public static event EChartOptionChangedEventHandler? EChartOptionChanged;

    static EChartOption()
    {
        EChartType = EChartConst.Line;
        Tooltip.PropertyChanged += Tooltip_PropertyChanged;
        Legend.PropertyChanged += Legend_PropertyChanged;
        Toolbox.PropertyChanged += Toolbox_PropertyChanged;
        XAxis.PropertyChanged += XAxis_PropertyChanged;
        YAxis.PropertyChanged += YAxis_PropertyChanged;
    }

    private static void YAxis_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(YAxis.Show):
                EChartType.SetValue("yAxis.show", YAxis.Show);
                break;
            case nameof(YAxis.ShowLine):
                EChartType.SetValue("yAxis.axisLine.show", YAxis.ShowLine);
                break;
            case nameof(YAxis.ShowTick):
                EChartType.SetValue("yAxis.axisTick.show", YAxis.ShowTick);
                break;
            case nameof(YAxis.ShowLabel):
                EChartType.SetValue("yAxis.axisLabel.show", YAxis.ShowLabel);
                break;
            default: break;
        }
        EChartOptionChanged?.Invoke();
    }

    private static void XAxis_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(XAxis.Show):
                EChartType.SetValue("xAxis.show", XAxis.Show);
                break;
            case nameof(XAxis.ShowLine):
                EChartType.SetValue("xAxis.axisLine.show", XAxis.ShowLine);
                break;
            case nameof(XAxis.ShowTick):
                EChartType.SetValue("xAxis.axisTick.show", XAxis.ShowTick);
                break;
            case nameof(XAxis.ShowLabel):
                EChartType.SetValue("xAxis.axisLabel.show", XAxis.ShowLabel);
                break;
            default: break;
        }
        EChartOptionChanged?.Invoke(); ;
    }

    private static void Toolbox_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Toolbox.Show):
                EChartType.SetValue("toolbox.show", Toolbox.Show);
                break;
            case nameof(Toolbox.Orient):
                EChartType.SetValue("toolbox.orient", Toolbox.Orient);
                break;
            case nameof(Toolbox.XPositon):
                EChartType.SetValue("toolbox.left", Toolbox.XPositon);
                break;
            case nameof(Toolbox.YPositon):
                EChartType.SetValue("toolbox.top", Toolbox.YPositon);
                break;
            case nameof(Toolbox.Feature):
                EChartType.SetValue("toolbox.feature", Toolbox.Feature.ToDictionary(f => f.AsT0, f => new object()));
                break;
            default: break;
        }
        EChartOptionChanged?.Invoke();
    }

    private static void Legend_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Legend.Show):
                EChartType.SetValue("legend.show", Legend.Show);
                break;
            case nameof(Legend.Orient):
                EChartType.SetValue("legend.orient", Legend.Orient);
                break;
            case nameof(Legend.XPositon):
                EChartType.SetValue("legend.left", Legend.XPositon);
                break;
            case nameof(Legend.YPositon):
                EChartType.SetValue("legend.top", Legend.YPositon);
                break;
            case nameof(Legend.Type):
                EChartType.SetValue("legend.type", Legend.Type);
                break;
            default: break;
        }
        EChartOptionChanged?.Invoke();
    }

    private static void Tooltip_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(Tooltip.Show):
                EChartType.SetValue("tooltip.show", Tooltip.Show);
                break;
            case nameof(Tooltip.RenderModel):
                EChartType.SetValue("tooltip.renderMode", Tooltip.RenderModel);
                break;
            case nameof(Tooltip.ClassName):
                EChartType.SetValue("tooltip.className", Tooltip.ClassName);
                break;
            case nameof(Tooltip.Trigger):
                EChartType.SetValue("tooltip.trigger", Tooltip.Trigger);
                break;
            default: break;
        }
        EChartOptionChanged?.Invoke();
    }
}

public class Tooltip : NotifyingEntity
{
    bool _show = true;
    string _className;
    string _renderModel = "html";
    string _trigger = "axis";

    public bool Show
    {
        get => _show;
        set => SetField(ref _show, value);
    }

    public string RenderModel
    {
        get => _renderModel;
        set => SetField(ref _renderModel, value);
    }

    [Description("className")]
    public string ClassName
    {
        get => _className;
        set => SetField(ref _className, value);
    }

    public string Trigger
    {
        get => _trigger;
        set => SetField(ref _trigger, value);
    }
}

public class Legend : NotifyingEntity
{
    bool _show = true;
    string _orient = "vertical";
    string _xPositon = "center";
    string _yPositon = "bottom";
    string _type = "plain";

    public bool Show
    {
        get => _show;
        set => SetField(ref _show, value);
    }

    public string Orient
    {
        get => _orient;
        set => SetField(ref _orient, value);
    }

    public string XPositon
    {
        get => _xPositon;
        set => SetField(ref _xPositon, value);
    }

    public string YPositon
    {
        get => _yPositon;
        set => SetField(ref _yPositon, value);
    }

    public string Type
    {
        get => _type;
        set => SetField(ref _type, value);
    }
}

public class Toolbox : NotifyingEntity
{
    bool _show = true;
    string _orient = "vertical";
    string _xPositon = "right";
    string _yPositon = "top";
    List<StringNumber> _feature = new();

    public bool Show
    {
        get => _show;
        set => SetField(ref _show, value);
    }

    public string Orient
    {
        get => _orient;
        set => SetField(ref _orient, value);
    }

    public string XPositon
    {
        get => _xPositon;
        set => SetField(ref _xPositon, value);
    }

    public string YPositon
    {
        get => _yPositon;
        set => SetField(ref _yPositon, value);
    }

    public List<StringNumber> Feature
    {
        get => _feature;
        set => SetField(ref _feature, value);
    }
}

public class Title
{
    public string Show { get; set; }

    public string Text { get; set; }

    public string Subtext { get; set; }
}

public class Axis : NotifyingEntity
{
    bool _show = true, _showLine = true, _showTick = true, _showLabel = true;


    public bool Show
    {
        get => _show;
        set => SetField(ref _show, value);
    }

    public bool ShowLine
    {
        get => _showLine;
        set => SetField(ref _showLine, value);
    }

    public bool ShowTick
    {
        get => _showTick;
        set => SetField(ref _showTick, value);
    }

    public bool ShowLabel
    {
        get => _showLabel;
        set => SetField(ref _showLabel, value);
    }
}

public abstract class NotifyingEntity : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        //compatible List ObservableCollection not work
        if (!typeof(T).IsClass && EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public delegate void EChartOptionChangedEventHandler();