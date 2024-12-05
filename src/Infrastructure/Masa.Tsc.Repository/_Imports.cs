// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Data.UoW;
global using Masa.BuildingBlocks.Ddd.Domain.Repositories;
global using Masa.Contrib.Ddd.Domain.Repository.EFCore;
global using Masa.Tsc.Domain;
global using Masa.Tsc.Domain.Instruments.Aggregates;
global using Masa.Tsc.EFCore;
global using Microsoft.EntityFrameworkCore;
global using ClickHouse.Ado;
global using Microsoft.Extensions.Logging;
global using Masa.Tsc.Storage.Clickhouse;
global using System.Data;
global using System.Text;
global using Masa.Tsc.Contracts.Admin.Enums;
global using Masa.Tsc.Repository;
