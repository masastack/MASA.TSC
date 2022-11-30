﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Caching;
global using Masa.BuildingBlocks.Configuration;
global using Masa.BuildingBlocks.Data;
global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Ddd.Domain.Entities;
global using Masa.BuildingBlocks.Ddd.Domain.Entities.Full;
global using Masa.BuildingBlocks.Ddd.Domain.Repositories;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;
global using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;
global using Masa.BuildingBlocks.StackSdks.Auth;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Consts;
global using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;
global using Masa.BuildingBlocks.StackSdks.Pm;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Log;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Model.Aggregate;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Service;
global using Masa.BuildingBlocks.StackSdks.Tsc.Contracts.Trace;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;
global using Masa.Contrib.Data.UoW.EFCore;
global using Masa.Contrib.Ddd.Domain.Repository.EFCore;
global using Masa.Contrib.Dispatcher.Events;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;
global using Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore;
global using Masa.Contrib.Service.Caller.Authentication.OpenIdConnect;
global using Masa.Contrib.StackSdks.Tsc.Elasticsearch.Constants;
global using Masa.Tsc.Contracts.Admin;
global using Masa.Tsc.Contracts.Admin.Enums;
global using Masa.Tsc.Contracts.Admin.Infrastructure.Const;
global using Masa.Tsc.Contracts.Admin.Instruments;
global using Masa.Tsc.Contracts.Admin.Topologies;
global using Masa.Tsc.Service.Admin.Application.Instruments.Commands;
global using Masa.Tsc.Service.Admin.Application.Instruments.Queries;
global using Masa.Tsc.Service.Admin.Application.Logs;
global using Masa.Tsc.Service.Admin.Application.Metrics;
global using Masa.Tsc.Service.Admin.Application.Projects;
global using Masa.Tsc.Service.Admin.Application.Setting;
global using Masa.Tsc.Service.Admin.Application.Teams;
global using Masa.Tsc.Service.Admin.Application.Topologies.Query;
global using Masa.Tsc.Service.Admin.Application.Traces;
global using Masa.Tsc.Service.Admin.Domain.Aggregates;
global using Masa.Tsc.Service.Admin.Domain.Repositories;
global using Masa.Tsc.Service.Admin.Domain.Topologies.Aggregates;
global using Masa.Tsc.Service.Admin.Domain.Topologies.Repositories;
global using Masa.Tsc.Service.Admin.Infrastructure.Caching;
global using Masa.Tsc.Service.Admin.Infrastructure.Const;
global using Masa.Tsc.Service.Infrastructure;
global using Masa.Tsc.Service.Infrastructure.Middleware;
global using Masa.Utils.Data.Elasticsearch;
global using Masa.Utils.Data.Prometheus;
global using Masa.Utils.Data.Prometheus.Enums;
global using Masa.Utils.Data.Prometheus.Model;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.OpenApi.Models;
global using System.Linq.Expressions;
global using System.Net.Http.Headers;
global using System.Reflection;
global using System.Runtime.ExceptionServices;