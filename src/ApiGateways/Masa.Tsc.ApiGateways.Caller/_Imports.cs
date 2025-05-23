﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Service.Caller;
global using Masa.BuildingBlocks.StackSdks.Auth;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;
global using Masa.BuildingBlocks.StackSdks.Dcc;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Log;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model.Aggregate;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Trace;
global using Masa.Contrib.Service.Caller;
global using Masa.Tsc.ApiGateways.Caller;
global using Masa.Tsc.ApiGateways.Caller.Services;
global using Masa.Tsc.Contracts.Admin;
global using Masa.Tsc.Contracts.Admin.Apps;
global using Masa.Tsc.Contracts.Admin.Converters;
global using Masa.Tsc.Contracts.Admin.Dashboards;
global using Masa.Tsc.Contracts.Admin.Enums;
global using Masa.Tsc.Contracts.Admin.Instruments;
global using Masa.Tsc.Contracts.Admin.Logs;
global using Masa.Tsc.Contracts.Admin.Metrics;
global using Masa.Tsc.Contracts.Admin.Setting;
global using Masa.Tsc.Contracts.Admin.Topologies;
global using Masa.Tsc.Contracts.Admin.Traces;
global using Masa.Tsc.Contracts.Admin.User;
global using Masa.Tsc.Storage.Clickhouse.Apm.Models.Request;
global using Masa.Tsc.Storage.Clickhouse.Apm.Models.Response;
global using Masa.Tsc.Storage.Prometheus.Model;
global using Masa.Utils.Models;
global using Microsoft.Extensions.DependencyInjection;
global using System.Text;
global using System.Text.Json;
