﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using BlazorComponent;
global using BlazorComponent.I18n;
global using Masa.Blazor;
global using Masa.BuildingBlocks.Authentication.Identity;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Log;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model.Aggregate;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Trace;
global using Masa.Contrib.Service.Caller.Authentication.OpenIdConnect;
global using Masa.Stack.Components;
global using Masa.Stack.Components.Models;
global using Masa.Tsc.ApiGateways.Caller;
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
global using Masa.Utils.Data.Prometheus.Enums;
global using Masa.Utils.Data.Prometheus.Model;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Rendering;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using System;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Nodes;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using Masa.Tsc.Contracts.Admin.Dashboards;
global using System.Linq.Expressions;
global using Masa.BuildingBlocks.StackSdks.Pm.Model;
global using Masa.BuildingBlocks.StackSdks.Pm;
global using System.Diagnostics.CodeAnalysis;
global using Microsoft.JSInterop;
global using Masa.Tsc.Web.Admin.Rcl.Components.Panel.Models;
global using Masa.Tsc.Web.Admin.Rcl.Components.Panel.Topology;