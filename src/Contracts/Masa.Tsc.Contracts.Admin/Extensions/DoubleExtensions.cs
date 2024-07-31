// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace System;

public static class DoubleExtensions
{
    public static double FloorDouble(this double value, int length)
    {
        return Math.Round(value, length, MidpointRounding.ToNegativeInfinity);
    }
}
