// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Storage.Cubejs.Apm.Response
{
    internal record CubeListData<T>([property: JsonPropertyName("cube")] List<T> Items) where T : class;

    internal record CubeData<T>([property: JsonPropertyName("cube")] T Item) where T : class;
}
