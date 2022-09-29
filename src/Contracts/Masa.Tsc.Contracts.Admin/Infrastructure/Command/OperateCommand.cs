// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Infrastructure.Command;

public sealed class OperateCommand : IEquatable<OperateCommand>
{
    private static string[] _values;
    private static Dictionary<string, OperateCommand> _dic=new Dictionary<string, OperateCommand>();

    public string Value { get; private set; }

    public static OperateCommand Add { get { return GetCommand(nameof(Add)); } }

    public static OperateCommand Remove { get { return GetCommand(nameof(Remove)); } }

    public static OperateCommand Update { get { return GetCommand(nameof(Update)); } }

    public static OperateCommand View { get { return GetCommand(nameof(View)); } }

    public static OperateCommand Close { get { return GetCommand(nameof(Close)); } }

    public static OperateCommand Open { get { return GetCommand(nameof(Open)); } }

    public static OperateCommand Fail { get { return GetCommand(nameof(Fail)); } }

    public static OperateCommand Success { get { return GetCommand(nameof(Success)); } }

    public static OperateCommand SuccessRefesh { get { return GetCommand(nameof(SuccessRefesh)); } }

    public static OperateCommand SuccessClose { get { return GetCommand(nameof(SuccessClose)); } }

    public static OperateCommand Back { get { return GetCommand(nameof(Back)); } }

    public static bool Parse(object obj, out OperateCommand command)
    {
        command = default!;
        if(obj == null) 
            return false;
        var value = obj.ToString();
        if (string.IsNullOrEmpty(value))
            return false;
        if (!_values.Any(str => string.Equals(value, str, StringComparison.OrdinalIgnoreCase)))
            return false;
        command = GetCommand(value);
        return true;
    }

    public bool Equals(OperateCommand? other)
    {
        return string.Equals(Value, other?.Value, StringComparison.InvariantCultureIgnoreCase);
    }

    static OperateCommand()
    {
        var properties = typeof(OperateCommand).GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        _values = properties.Select(p => p.Name).ToArray();
    }

    private static OperateCommand GetCommand(string name)
    {
        lock (_dic)
        {
            if(!_dic.ContainsKey(name))             
                _dic.Add(name, new OperateCommand { Value = name });

            return _dic[name];
        }
    }
}