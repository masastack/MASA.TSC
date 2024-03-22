// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model;

internal static class BaseRequestDtoExtensions
{
    public static void SetValues(this BaseRequestDto dto)
    {
        if (dto.Conditions != null)
        {
            foreach (var item in dto.Conditions)
            {
                if (item.Type == ConditionTypes.In && item.Value is JsonElement json)
                {
                    item.Value = json.EnumerateArray().Select(value => value.ToString());
                }
            }
        }
    }

    public static void SetEnv(this BaseRequestDto dto, string envName, bool isMustService = true)
    {
        if (isMustService && string.IsNullOrEmpty(dto.Service))
            return;

        var list = dto.Conditions?.ToList() ?? new();

        if (!list.Any() || !list.Exists(item => item.Name == StorageConst.Environment))
        {
            list.Add(new FieldConditionDto
            {
                Name = StorageConst.Environment,
                Type = ConditionTypes.Equal,
                Value = envName
            });
        }
        dto.Conditions = list;
    }
}