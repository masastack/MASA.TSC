// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.ComponentModel;

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public static class ObjectExtensions
{
    public static IDictionary<string, object> ToDictionary(this object source, params string[] excludedProperties)
    {
        return source.ToDictionary<object>(excludedProperties);
    }

    public static IDictionary<string, T> ToDictionary<T>(this object source, params string[] excludedProperties)
    {
        if (source == null) ThrowExceptionWhenSourceArgumentIsNull();

        var dictionary = new Dictionary<string, T>();
        foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
        {
            if (excludedProperties.Contains(property.Name))
            {
                continue;
            }

            object value = property.GetValue(source);
            if (IsOfType<T>(value))
            {
                dictionary.Add(property.Name, (T)value);
            }
        }

        return dictionary;
    }

    private static bool IsOfType<T>(object value)
    {
        return value is T;
    }

    private static void ThrowExceptionWhenSourceArgumentIsNull()
    {
        throw new NullReferenceException("Unable to convert anonymous object to a dictionary. The source anonymous object is null.");
    }
}
