// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller;

public class TscCaller
{
    internal TscCaller(IServiceProvider serviceProvider, ICaller caller, TokenProvider tokenProvider)
    {
        AppService = new AppService(caller, tokenProvider);
        ProjectService = new ProjectService(caller, tokenProvider, serviceProvider.GetRequiredService<IDccClient>());
        TeamService = new TeamService(caller, tokenProvider);
        LogService = new LogService(caller, tokenProvider);
        TraceService = new TraceService(caller, tokenProvider);
        DirectoryService = new DirectoryService(caller, tokenProvider);
        InstrumentService = new InstrumentService(caller, tokenProvider);
        PanelService = new PanelService(caller, tokenProvider);
        MetricService = new MetricService(caller, tokenProvider);
        TopologyService = new TopologyService(caller, tokenProvider);
    }

    public AppService AppService { get; private init; }

    public ProjectService ProjectService { get; private init; }

    public TeamService TeamService { get; private init; }

    public LogService LogService { get; private init; }

    public TraceService TraceService { get; private init; }

    public DirectoryService DirectoryService { get; private init; }

    public InstrumentService InstrumentService { get; private init; }

    public PanelService PanelService { get; private init; }

    public MetricService MetricService { get; private init; }

    public TopologyService TopologyService { get; private init; }
}
