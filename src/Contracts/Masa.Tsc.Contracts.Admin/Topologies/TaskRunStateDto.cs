// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Contracts.Admin.Topologies;

public class TaskRunStateDto
{
    public string Name { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    /// <summary>
    /// 0 未启动 1执行中 2异常 4已完成
    /// </summary>
    public int Status { get; set; }
}
