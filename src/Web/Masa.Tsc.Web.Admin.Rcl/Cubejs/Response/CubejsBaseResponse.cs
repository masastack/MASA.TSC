// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Cubejs.Response;

/// <summary>
/// base cube request result
/// </summary>
/// <typeparam name="TData"></typeparam>
/// <param name="Data"></param>
public record CubejsBaseResponse<TData>([property: JsonPropertyName(CubejsConstants.CUBEJS_IDENTITY)] List<TData> Data) where TData : class;


/// <summary>
/// group query timestamp
/// </summary>
/// <param name="DateKey"></param>
public record CubejsAggDateTimeResponse([property: JsonPropertyName(CubejsConstants.TIMESTAMP_AGG)] CubejsAggDateTimeValueResponse DateKey);

/// <summary>
/// group query timestamp value
/// </summary>
/// <param name="Value"></param>

public record CubejsAggDateTimeValueResponse(DateTime? Value, DateTime? Minute, DateTime? Hour, DateTime? Day)
{
    public DateTime? DateTime
    {
        get
        {
            if (Value.HasValue)
                return Value.Value;
            if (Minute != null)
                return Minute.Value;
            if (Hour != null)
                return Hour.Value;
            if (Day != null)
                return Day.Value;
            return default;
        }
    }
}

public record CubejsAggDateTimeMinuteValueResponse([property: JsonPropertyName(CubejsConstants.TIMESTAMP_MINITE_VALUE)] DateTime Value);

public record CubejsAggDateTimeHourValueResponse([property: JsonPropertyName(CubejsConstants.TIMESTAMP_HOUR_VALUE)] DateTime Value);

public record CubejsAggDateTimeDayValueResponse([property: JsonPropertyName(CubejsConstants.TIMESTAMP_DAY_VALUE)] DateTime Value);