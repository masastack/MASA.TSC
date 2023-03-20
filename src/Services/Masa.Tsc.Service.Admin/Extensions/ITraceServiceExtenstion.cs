// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Nest.Extensions;

public static class ITraceServiceExtenstion
{
    private static Dictionary<Type, FieldInfo> _dic = new();

    public static IElasticClient GetElasticClient(this ITraceService traceService)
    {
        var type = traceService.GetType();
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

        if (field != null && field.GetValue(traceService) is IElasticClient client)
                return client;        

        throw new UserFriendlyException("ITraceService not have field _client");
    }
}
