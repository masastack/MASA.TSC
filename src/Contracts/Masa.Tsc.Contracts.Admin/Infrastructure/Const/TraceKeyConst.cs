// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Infrastructure.Const;

public sealed class TraceKeyConst
{
    public sealed class Resource
    {
        private Resource() { }

        public const string ServiceName = "service.name";
    }

    public sealed class Attributes
    {
        private Attributes() { }
        public const string Db = "db.system";
        public const string HttpKey = "http.method";
        public const string HttpStatusCode = "http.status_code";
        public const string Target = "http.target";
    }
}