// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using GraphQL.Client.Http;
global using GraphQL.Client.Serializer.SystemTextJson;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Trace;
global using Masa.Tsc.Storage.Clickhouse.Apm.Shared.Models;
global using Masa.Tsc.Storage.Clickhouse.Apm.Shared.Models.Request;
global using Masa.Tsc.Storage.Clickhouse.Apm.Shared.Models.Response;
global using Masa.Tsc.Storage.Clickhouse.Apm.Shared.Service;
global using Masa.Tsc.Storage.Cubejs.Apm;
global using Masa.Tsc.Storage.Cubejs.Apm.Request;
global using Masa.Tsc.Storage.Cubejs.Apm.Response;
global using Masa.Utils.Models;
global using System.Net.Http.Headers;
global using System.Text;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
