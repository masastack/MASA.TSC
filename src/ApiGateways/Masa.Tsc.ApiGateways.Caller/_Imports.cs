// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Service.Caller;
global using Masa.Contrib.Service.Caller;
global using Masa.Contrib.Service.Caller.Authentication.OpenIdConnect;
//global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Provider;
global using Masa.Contrib.Service.Caller.HttpClient;
global using Masa.Tsc.ApiGateways.Caller.Services;
global using Masa.Tsc.Contracts.Admin;
global using Masa.Tsc.Contracts.Admin.Charts;
global using Masa.Tsc.Contracts.Admin.Infrastructure.Dtos;
global using Masa.Tsc.Contracts.Admin.Instruments;
global using Masa.Utils.Data.Prometheus.Model;
global using Microsoft.Extensions.DependencyInjection;
global using System.Net.Http.Headers;
global using System.Text;