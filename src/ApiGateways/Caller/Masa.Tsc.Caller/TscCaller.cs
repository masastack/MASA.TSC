// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Caller;

public class TscCaller
{
    public TscCaller(ICallerProvider caller)
    {
        AppService = new AppService(caller);
        ProjectService = new ProjectService(caller);
        TeamService = new TeamService(caller);
        LogService = new LogService(caller);
        TraceService = new TraceService(caller);
        SettingService = new SettingService(caller);
        DirectoryService = new DirectoryService(caller);
    }

    public AppService AppService { get; private init; }

    public ProjectService ProjectService { get; private init; }

    public TeamService TeamService { get; private init; }

    public LogService LogService { get; private init; }

    public TraceService TraceService { get; private init; }

    public SettingService SettingService { get; private init; }

    public DirectoryService DirectoryService { get; private init; }
}
