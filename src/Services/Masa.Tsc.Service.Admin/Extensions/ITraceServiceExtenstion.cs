// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Nest.Extensions;

public static class ITraceServiceExtenstion
{
    private static Dictionary<Type, FieldInfo> _dic = new();

    public static IElasticClient GetElasticClient(this ITraceService service)
    {
        return GetElasticClient((object)service);
    }

    public static IElasticClient GetElasticClient(this ILogService service)
    {
        return GetElasticClient((object)service);
    }

    private static IElasticClient GetElasticClient(object service)
    {
        var type = service.GetType();
        FieldInfo field;
        if (_dic.ContainsKey(type))
        {
            field = _dic[type];
        }
        else
        {
            field = type.GetField("_client", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField)!;
            _dic.Add(type, field);
        }

        if (field != null && field.GetValue(service) is IElasticClient client)
            return client;

        throw new UserFriendlyException($"{type.Name} not have field _client");
    }
}
