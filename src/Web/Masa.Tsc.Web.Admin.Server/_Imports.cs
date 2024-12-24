// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.StackSdks.Config;
global using Masa.Contrib.Caching.Distributed.StackExchangeRedis;
global using Masa.Contrib.StackSdks.Caller;
global using Masa.Contrib.StackSdks.Config;
global using Masa.Contrib.StackSdks.Tsc;
global using Masa.Stack.Components;
global using Masa.Stack.Components.Extensions.OpenIdConnect;
global using Masa.Tsc.ApiGateways.Caller;
global using Masa.Tsc.Contracts.Admin.Converters;
global using Masa.Tsc.Contracts.Admin.Infrastructure.Const;
global using Masa.Tsc.Web.Admin.Rcl.Extentions;
global using Masa.Utils.Configuration.Json;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Hosting.StaticWebAssets;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.RazorPages;
global using Microsoft.IdentityModel.Logging;
global using Microsoft.IdentityModel.Protocols.OpenIdConnect;
global using System.Diagnostics;
global using System.Security.Cryptography.X509Certificates;
global using Masa.Contrib.StackSdks.Tsc.OpenTelemetry.Blazor;
global using System.Reflection;