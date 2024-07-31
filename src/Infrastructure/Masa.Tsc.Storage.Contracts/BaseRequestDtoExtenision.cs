// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Contracts;

public static class BaseRequestDtoExtenision
{
    const string NoPageKey = "__noPage";

    public static void SetHasPage(this BaseRequestDto requestDto, bool hasPage = true)
    {
        if (hasPage)
        {
            if (requestDto.Conditions == null || !requestDto.Conditions.Any(item => item.Name == NoPageKey))
                return;
            else
            {
                var list = requestDto.Conditions.ToList();
                list.RemoveAll(item => item.Name == NoPageKey);
                requestDto.Conditions = list;
            }
        }
        else
        {
            if (requestDto.Conditions == null || !requestDto.Conditions.Any(item => item.Name == NoPageKey))
            {
                var list = requestDto.Conditions?.ToList() ?? new();
                list.Add(new FieldConditionDto { Name = NoPageKey, Value = true });
                requestDto.Conditions = list;
            }
            else
            {
                var find = requestDto.Conditions.First(item => item.Name == NoPageKey);
                find.Value = true;
            }
        }
    }

    public static bool HasPage(this BaseRequestDto requestDto)
    {
        var value = requestDto.Conditions?.FirstOrDefault(item => item.Name == NoPageKey)?.Value;
        return value == null || !((bool)value);
    }
}