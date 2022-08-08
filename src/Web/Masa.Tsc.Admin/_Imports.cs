// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using BlazorComponent;
global using BlazorComponent.I18n;
global using Masa.Blazor;
global using Masa.Contrib.BasicAbility.Tsc;
global using Masa.Stack.Components.Models;
global using Masa.Tsc.Admin.Rcl.Data.Trace;
global using Masa.Tsc.Admin.Rcl.Pages.Components;
global using Masa.Tsc.Admin.Rcl.Shared;
global using Masa.Tsc.Caller;
global using Masa.Tsc.Caller.Extensions;
global using Masa.Tsc.Contracts.Admin;
global using Masa.Tsc.Contracts.Admin.Chart;
global using Masa.Tsc.Contracts.Admin.Enums;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Rendering;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using OpenTelemetry.Logs;
global using OpenTelemetry.Metrics;
global using OpenTelemetry.Resources;
global using OpenTelemetry.Trace;
global using System;
global using System.Text.Json;
global using Masa.BuildingBlocks.Identity.IdentityModel;