// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Service.Caller;
global using Masa.BuildingBlocks.StackSdks.Dcc;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Log;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model.Aggregate;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Trace;
global using Masa.Contrib.Service.Caller;
global using Masa.Contrib.Service.Caller.Authentication.OpenIdConnect;
global using Masa.Contrib.Service.Caller.HttpClient;
global using Masa.Tsc.ApiGateways.Caller.Services;
global using Masa.Tsc.Contracts.Admin;
global using Masa.Tsc.Contracts.Admin.Converters;
global using Masa.Tsc.Contracts.Admin.Dashboards;
global using Masa.Tsc.Contracts.Admin.Instruments;
global using Masa.Utils.Data.Prometheus.Model;
global using Masa.Utils.Models;
global using Microsoft.Extensions.DependencyInjection;
global using System.Net.Http.Headers;
global using System.Text;
global using System.Text.Json;