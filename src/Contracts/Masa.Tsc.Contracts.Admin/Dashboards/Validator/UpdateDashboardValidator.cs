// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Dashboards.Validator;

public class UpdateDashboardValidator : AbstractValidator<UpdateDashboardDto>
{
    public UpdateDashboardValidator()
    {
        RuleFor(dashboard => dashboard.Name).Required().ChineseLetterNumber();
        RuleFor(dashboard => dashboard.Folder).Required();
    }
}

