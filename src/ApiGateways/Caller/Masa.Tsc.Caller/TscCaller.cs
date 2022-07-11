// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Caller;

public class TscCaller
{
    private readonly ICallerProvider _caller;
    private readonly AppService _appService;
    private readonly ProjectService _projectService;
    private readonly TeamService _teamService;

    public TscCaller(ICallerProvider caller)
    {
        _caller = caller;
        _appService = new AppService(caller);
        _projectService = new ProjectService(caller);
        _teamService = new TeamService(caller);
    }

    public AppService AppService { get { return _appService; } }

    public ProjectService ProjectService { get { return _projectService; } }

    public TeamService TeamService { get { return _teamService; } }
}
