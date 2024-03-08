﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller;

public class TscCaller
{
    internal TscCaller(IServiceProvider serviceProvider, ICaller caller)
    {
        AppService = new AppService(caller);
        SettingService = new SettingService(caller);
        ProjectService = new ProjectService(caller, serviceProvider.GetRequiredService<IDccClient>());
        TeamService = new TeamService(caller);
        LogService = new LogService(caller);
        TraceService = new TraceService(caller);
        DirectoryService = new DirectoryService(caller);
        InstrumentService = new InstrumentService(caller);
        MetricService = new MetricService(caller);
        TopologyService = new TopologyService(caller);
        ApmService=new ApmService(caller);
    }

    public AppService AppService { get; private init; }

    public ApmService ApmService { get; private init; }

    public ProjectService ProjectService { get; private init; }

    public TeamService TeamService { get; private init; }

    public LogService LogService { get; private init; }

    public TraceService TraceService { get; private init; }

    public DirectoryService DirectoryService { get; private init; }

    public InstrumentService InstrumentService { get; private init; }

    public MetricService MetricService { get; private init; }

    public TopologyService TopologyService { get; private init; }

    public SettingService SettingService { get; private init; }
}
