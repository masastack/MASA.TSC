﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using BlazorComponent;
global using BlazorComponent.I18n;
global using Force.DeepCloner;
global using Masa.Blazor;
global using Masa.BuildingBlocks.Authentication.Identity;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts;
global using Masa.BuildingBlocks.StackSdks.Config;
global using Masa.BuildingBlocks.StackSdks.Pm;
global using Masa.BuildingBlocks.StackSdks.Pm.Model;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Log;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model.Aggregate;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Trace;
global using Masa.BuildingBlocks.Storage.ObjectStorage;
global using Masa.Contrib.StackSdks.Caller;
global using Masa.Contrib.StackSdks.Tsc.Apm.Clickhouse.Models;
global using Masa.Contrib.StackSdks.Tsc.Apm.Clickhouse.Models.Request;
global using Masa.Contrib.StackSdks.Tsc.Apm.Clickhouse.Models.Response;
global using Masa.Stack.Components;
global using Masa.Stack.Components.Configs;
global using Masa.Stack.Components.Extensions;
global using Masa.Stack.Components.Models;
global using Masa.Stack.Components.UserCenters.Models;
global using Masa.Tsc.ApiGateways.Caller;
global using Masa.Tsc.Contracts.Admin;
global using Masa.Tsc.Contracts.Admin.Dashboards;
global using Masa.Tsc.Contracts.Admin.Enums;
global using Masa.Tsc.Contracts.Admin.Infrastructure.Const;
global using Masa.Tsc.Contracts.Admin.Infrastructure.Utils;
global using Masa.Tsc.Contracts.Admin.Logs;
global using Masa.Tsc.Contracts.Admin.Metrics;
global using Masa.Tsc.Contracts.Admin.Setting;
global using Masa.Tsc.Web.Admin.Rcl.Components.Antvg6;
global using Masa.Tsc.Web.Admin.Rcl.Components.Apps;
global using Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Models;
global using Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Panel.Chart;
global using Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Panel.Chart.Models;
global using Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Panel.Log.Models;
global using Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Panel.Tabs.Models;
global using Masa.Tsc.Web.Admin.Rcl.Components.Dashboards.Configurations.Panel.Trace;
global using Masa.Tsc.Web.Admin.Rcl.Components.Gridstack.Models;
global using Masa.Tsc.Web.Admin.Rcl.Components.Project.Charts;
global using Masa.Tsc.Web.Admin.Rcl.Data.Apm;
global using Masa.Tsc.Web.Admin.Rcl.Data.Team;
global using Masa.Tsc.Web.Admin.Rcl.Extentions;
global using Masa.Tsc.Web.Admin.Rcl.Pages.Dashboards.Models;
global using Masa.Tsc.Web.Admin.Rcl.Shared;
global using Masa.Tsc.Web.Admin.Rcl.Shared.Apm;
global using Masa.Utils.Data.Prometheus.Enums;
global using Masa.Utils.Data.Prometheus.Model;
global using Masa.Utils.Models;
global using Masa.Utils.Security.Cryptography;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Rendering;
global using Microsoft.AspNetCore.Components.Web;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.JSInterop;
global using System;
global using System.ComponentModel;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq.Expressions;
global using System.Runtime.CompilerServices;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Nodes;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using System.Web;
