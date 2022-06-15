// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Projects.Events
{
    public class QueryHandler
    {
        private readonly IPmClient _pmClient;
        private readonly IAuthClient _authClient;

        public QueryHandler(IPmClient pmClient, IAuthClient authClient)
        {
            _pmClient = pmClient;
            _authClient = authClient;
        }

        [EventHandler]
        public async Task GetProjectsAsync(ProjectsQuery query)
        {
            var teams = Array.Empty<TeamDetailModel>();
            if (teams != null && teams.Any())
            {
                query.Result = await GetAllProjectsAsync(teams.Select(t => t.Id).ToList());
            }
        }

        private async Task<List<ProjectDto>> GetAllProjectsAsync(List<Guid> teamIds)
        {
            var result = new List<ProjectDto>();
            var list = new List<int>();
            foreach (var id in teamIds)
            {
                var projects = await _pmClient.ProjectService.GetListByTeamIdAsync(id);
                if (projects == null || !projects.Any())
                    continue;
                foreach (var project in projects)
                {
                    if (list.Contains(project.Id))
                        continue;

                    list.Add(project.Id);
                    result.Add(new ProjectDto
                    {
                        Id = project.Id.ToString(),
                        Identity = project.Identity,
                        Name = project.Name,
                        Description = project.Description,
                        LabelName = project.LabelName
                    });
                }
            }
            return result;
        }

        [EventHandler]
        public async Task GetAppsAsync(AppsQuery query)
        {
            if (int.TryParse(query.ProjectId, out int projectId) && projectId > 0)
            {
                var result = await _pmClient.AppService.GetListByProjectIdsAsync(new List<int> { projectId });
                if (result != null && result.Any())
                {
                    query.Result = result.Select(m => new AppDto
                    {
                        Id = m.Id.ToString(),
                        Identity = m.Identity,
                        Name = m.Name,
                        ServiceType = (ServiceTypes)((int)m.ServiceType)
                    }).ToList();
                }
            }
        }
    }
}
