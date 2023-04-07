// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Services
{
    public class Test: ServiceBase
    {
        [Route("/blazor")]
        public async Task<string> PostMsgAsync([FromBody] TTT data)
        {
            return "200";
        }

        public class TTT
        {
            public string Text { get; set; }
        }
    }
}
