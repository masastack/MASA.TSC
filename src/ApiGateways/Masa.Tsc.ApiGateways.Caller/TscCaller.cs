// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.ApiGateways.Caller;

public class TscCaller
{
    public TscCaller(ICaller caller, TokenProvider tokenProvider)
    {
        AppService = new AppService(caller, tokenProvider);
        ProjectService = new ProjectService(caller, tokenProvider);
        TeamService = new TeamService(caller, tokenProvider);
        LogService = new LogService(caller, tokenProvider);
        TraceService = new TraceService(caller, tokenProvider);
        SettingService = new SettingService(caller, tokenProvider);
        DirectoryService = new DirectoryService(caller, tokenProvider);
        InstrumentService = new InstrumentService(caller, tokenProvider);
        PanelService = new PanelService(caller, tokenProvider);
    }

    public AppService AppService { get; private init; }

    public ProjectService ProjectService { get; private init; }

    public TeamService TeamService { get; private init; }

    public LogService LogService { get; private init; }

    public TraceService TraceService { get; private init; }

    public SettingService SettingService { get; private init; }

    public DirectoryService DirectoryService { get; private init; }

    public InstrumentService InstrumentService { get; private init; }

    public PanelService PanelService { get; private init; }
}
