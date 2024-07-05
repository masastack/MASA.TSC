// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components.Apm;

partial class ApmMultiSearch
{
    [Parameter]
    public string Value { get; set; }

    [Parameter]
    public string Target { get; set; }

    [Parameter]
    public string Url { get; set; }

    [Parameter]
    public string RequestBody { get; set; }

    [Parameter]
    public string ResponseBody { get; set; }

    [Parameter]
    public string ExceptionType { get; set; }

    [Parameter]
    public string ExceptionMessage { get; set; }

    [Parameter]
    public string Body { get; set; }

    [Parameter]
    public Guid UserId { get; set; }

    [Parameter]
    public string HttpMethod { get; set; }

    [Parameter]
    public int StatusCode { get; set; }

    private void OnValueChange()
    { }
}