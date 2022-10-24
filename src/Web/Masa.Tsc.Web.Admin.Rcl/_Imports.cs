// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using BlazorComponent;
global using BlazorComponent.I18n;
global using Masa.Blazor;
global using Masa.BuildingBlocks.Authentication.Identity;
global using Masa.Contrib.StackSdks.Tsc;
global using Masa.Stack.Components.Models;
global using Masa.Tsc.ApiGateways.Caller;
global using Masa.Tsc.ApiGateways.Caller.Extensions;
global using Masa.Tsc.Contracts.Admin;
global using Masa.Tsc.Contracts.Admin.Charts;
global using Masa.Tsc.Contracts.Admin.Enums;
global using Masa.Tsc.Contracts.Admin.Infrastructure.Command;
global using Masa.Tsc.Contracts.Admin.Infrastructure.Const;
global using Masa.Tsc.Contracts.Admin.Instruments;
global using Masa.Tsc.Web.Admin.Rcl.Data;
global using Masa.Tsc.Web.Admin.Rcl.Data.EChart;
global using Masa.Tsc.Web.Admin.Rcl.Data.Instrument;
global using Masa.Tsc.Web.Admin.Rcl.Data.Team;
global using Masa.Tsc.Web.Admin.Rcl.Data.Trace;
global using Masa.Tsc.Web.Admin.Rcl.Shared;
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
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;