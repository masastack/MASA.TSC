// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Trace;

public static class TraceResponseDtoExtenstion
{
    public static TraceNodeTypes GetServiceType(this TraceResponseDto dto)
    {
        if (dto.IsDatabase())
            return TraceNodeTypes.Database;
        if (dto.IsApi())
            return TraceNodeTypes.HttpApi;
        if (dto.IsWeb())
            return TraceNodeTypes.Web;
        if (dto.IsDapr())
            return TraceNodeTypes.Dapr;
        return default;
    }

    public static bool IsApi(this TraceResponseDto dto)
    {
        return dto.TryParseHttp(out var _);
    }

    public static bool IsDapr(this TraceResponseDto dto)
    {
        return false;
    }

    public static bool IsWeb(this TraceResponseDto dto)
    {
        return true;
    }

    public static bool IsDatabase(this TraceResponseDto dto)
    {
        if (dto.TryParseDatabase(out _))
            return true;
        return false;
    }
}
