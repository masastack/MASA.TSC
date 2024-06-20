// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.User;

public class UserClaimPageDto
{
    public int Total { get; set; }

    public List<UserClaimDto> Items { get; }
}

public class UserClaimDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public UserClaimType UserClaimType { get; set; }
}

public enum UserClaimType
{
    Standard = 1,
    Customize
}
