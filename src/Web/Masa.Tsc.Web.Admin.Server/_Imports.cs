// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

global using Masa.BuildingBlocks.Configuration;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc.Options;
global using Masa.Contrib.Service.Caller.Authentication.OpenIdConnect;
global using Masa.Stack.Components;
global using Masa.Tsc.ApiGateways.Caller;
global using Masa.Tsc.Contracts.Admin.Converters;
global using Masa.Utils.Security.Authentication.OpenIdConnect;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Hosting.StaticWebAssets;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.RazorPages;
global using Microsoft.IdentityModel.Protocols.OpenIdConnect;
global using System.Diagnostics;
global using System.Security.Cryptography.X509Certificates;
global using Masa.Contrib.Configuration.ConfigurationApi.Dcc;
global using Masa.Tsc.Web.Admin.Rcl.Extentions;