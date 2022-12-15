// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Data;

public class TableFieldItemModel : INotifyPropertyChanged
{
    string _name;
    string _unit;
    string _icon;
    string _color = "#4318FF";

    public Guid Id { get; set; } = Guid.NewGuid();

    public string Color
    {
        get { return _color; }
        set
        {
            SetField(ref _color, value);
        }
    }

    public string Name
    {
        get { return _name; }
        set
        {
            SetField(ref _name, value);
        }
    }

    public string Unit
    {
        get { return _unit; }
        set
        {
            SetField(ref _unit, value);
        }
    }

    public string Icon
    {
        get { return _icon; }
        set
        {
            SetField(ref _icon, value);
        }
    }

    public string Range { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
