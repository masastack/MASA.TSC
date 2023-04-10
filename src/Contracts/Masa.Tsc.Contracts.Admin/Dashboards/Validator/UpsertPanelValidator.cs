// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Dashboards.Validator;

public class UpsertPanelValidator : AbstractValidator<UpsertPanelDto>
{
    public UpsertPanelValidator()
    {
        RuleFor(folder => folder.Title).Required()
            .MaxLength(50);
    }
}
