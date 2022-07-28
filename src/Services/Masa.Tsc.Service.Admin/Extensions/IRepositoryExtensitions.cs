// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Repositories;

public static class IRepositoryExtensitions
{
    private static Dictionary<Type, PropertyInfo> _dic = new Dictionary<Type, PropertyInfo>();

    public static IQueryable<T> ToQueryable<T>(this IRepository<T> repository) where T : class, IEntity
    {
        var type = repository.GetType();
        PropertyInfo property = default!;
        if (_dic.ContainsKey(type))
        {
            property=_dic[type];
        }
        else
        {
            property = type.GetProperty("Context")!;
            _dic.Add(type, property);
        }
        
        if (property != null)
        {
            if (property.GetValue(repository, null) is DbContext dbContext)
                return dbContext.Set<T>().AsQueryable();
        }

        throw new UserFriendlyException("repository not support to queryable");
    }
}
