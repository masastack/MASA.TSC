// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.BasicAbility.Auth;
global using Masa.BuildingBlocks.BasicAbility.Auth.Enum;
global using Masa.BuildingBlocks.BasicAbility.Auth.Model;
global using Masa.BuildingBlocks.BasicAbility.Pm;
global using Masa.BuildingBlocks.BasicAbility.Pm.Enum;
global using Masa.BuildingBlocks.Ddd.Domain.Entities;
global using Masa.BuildingBlocks.Dispatcher.Events;
global using Masa.Contrib.Data.UoW.EF;
global using Masa.Contrib.Ddd.Domain;
global using Masa.Contrib.Ddd.Domain.Repository.EF;
global using Masa.Contrib.Dispatcher.Events;
global using Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;
global using Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF;
global using Masa.Contrib.ReadWriteSpliting.Cqrs.Queries;
global using Masa.Contrib.Service.MinimalAPIs;
global using Masa.Tsc.Contracts.Admin;
global using Masa.Tsc.Contracts.Admin.Enums;
global using Masa.Tsc.Service.Admin.Application;
global using Masa.Tsc.Service.Admin.Domain.Projects.Events.Query;
global using Masa.Tsc.Service.Admin.Domain.Teams.Aggregates;
global using Masa.Tsc.Service.Admin.Domain.Teams.Events.Query;
global using Masa.Tsc.Service.Infrastructure;
global using Masa.Tsc.Service.Infrastructure.Middleware;
global using Masa.Utils.Data.EntityFrameworkCore;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.OpenApi.Models;
