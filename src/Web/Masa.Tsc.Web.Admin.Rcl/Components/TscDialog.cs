// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Web.Admin.Rcl.Components;

public class TscDialog : MDialog
{
    [Parameter]
    public EventCallback<object[]> OnParentCallBack { get; set; }

    [Parameter]
    public EventCallback<object[]> OnChildCallBack { get; set; }
}