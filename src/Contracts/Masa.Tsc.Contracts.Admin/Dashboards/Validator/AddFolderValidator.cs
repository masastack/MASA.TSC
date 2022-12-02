// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Dashboards.Validator;

public class AddFolderValidator : AbstractValidator<AddFolderDto>
{
    public AddFolderValidator()
    {
        RuleFor(folder => folder.Name).Required();
    }
}

