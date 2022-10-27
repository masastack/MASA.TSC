// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Ddd.Domain.Events;
using Masa.Tsc.Service.Admin.Domain.Instruments.Events;

namespace Masa.Tsc.Service.Admin.Application.Instruments
{
    public class InstrumentCommandHandler
    {
        private readonly IDirectoryRepository _directoryRepository;
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly IPanelRepository _panelRepository;
        private readonly IMetricReposity _metricReposity;

        public InstrumentCommandHandler(IDirectoryRepository directoryRepository,
        IInstrumentRepository instrumentRepository,
        IPanelRepository panelRepository,
        IMetricReposity metricReposity
            )
        {
            _directoryRepository = directoryRepository;
            _instrumentRepository = instrumentRepository;
            _panelRepository = panelRepository;
            _metricReposity = metricReposity;
        }

        [EventHandler]
        public async Task AddInstrumentAsync(AddInstrumentCommand command)
        {
            var model = new Instrument(command.Data.Id)
            {
                Name = command.Data.Name,
                IsGlobal = command.Data.IsGlobal,
                Layer = command.Data.Layer,
                IsRoot = command.Data.IsRoot,
                DirectoryId = command.Data.DirectoryId,
                Model = command.Data.Model,
                Sort = command.Data.Sort
            };
            await _instrumentRepository.AddAsync(model);
        }

        [EventHandler]
        public async Task UpdateInstrumentAsync(UpdateInstrumentCommand command)
        {
            var entry = await _instrumentRepository.FindAsync(m => m.Id == command.Data.Id);
            if (entry == null)
                throw new UserFriendlyException("数据不存在");
            entry.Update(command.Data);
            await _instrumentRepository.UpdateAsync(entry);
        }

        [EventHandler]
        public async Task RemoveInstrumentAsync(RemoveInstrumentCommand command)
        {
            var list = await _instrumentRepository.GetListAsync(m => m.Creator == command.UserId && command.InstrumentIds.Contains(m.Id));
            if (list == null || !list.Any())
                throw new UserFriendlyException("数据不存在");

            var instrumentIds = list.Select(m => m.Id).ToArray();
            await RemovePanelsByInstrumentAsync(instrumentIds);
            await _instrumentRepository.RemoveRangeAsync(list);
        }

        [EventHandler(0)]
        public async Task AddPanelAsync(AddPanelCommand command)
        {
            var entity = new Panel(command.Data.Id)
            {
                Type = command.Data.Type,
                Title = command.Data.Title ?? string.Empty,
                Description = command.Data.Description ?? string.Empty,
                Sort = command.Data.Sort,
                InstrumentId = command.Data.InstrumentId,
                ParentId = command.Data.ParentId
            };
            if (command.Data.Type == PanelTypes.Chart)
            {
                entity.ChartType = ((EChartPanelDto)command.Data).ChartType;
            }
            await _panelRepository.AddAsync(entity);            
        }

        [EventHandler(1)]
        public async Task AddPanelMetricAsync(AddPanelCommand command)
        {
            if (command.Data is EChartPanelDto chartDto && chartDto.Metrics != null && chartDto.Metrics.Any())
            {
                var list = chartDto.Metrics.Select(x => new PanelMetric(x.Id)
                {
                    Name = x.Name,
                    Caculate = x.Caculate,
                    PanelId = command.Data.Id,
                    Sort = command.Data.Sort,
                    Unit = x.Unit
                });

                await _metricReposity.AddRangeAsync(list);
            }
        }


        [EventHandler(0)]
        public async Task UpdatePanelAsync(UpdatePanelCommand command)
        {
            var panel = await _panelRepository.FindAsync(item => item.Id == command.Data.Id);
            if (panel == null)
                throw new UserFriendlyException("数据不存在");
            panel.Update(command.Data);
            await _panelRepository.UpdateAsync(panel);
        }

        [EventHandler(1)]
        public async Task UpdatePanelMetricAsync(UpdatePanelCommand command)
        {
            var panel = await _panelRepository.FindAsync(item => item.Id == command.Data.Id);
            if (panel != null && panel.Type == PanelTypes.Chart)
            {
                var echartPanel = ((EChartPanelDto)command.Data);
                var metrics = await _metricReposity.ToQueryable().Where(x => x.PanelId == panel.Id).OrderBy(x => x.Sort).ToListAsync();
                var adds = new List<PanelMetric>();
                foreach (var item in echartPanel.Metrics)
                {
                    var find = metrics.FirstOrDefault(x => x.Id == item.Id);
                    if (find == null)
                    {
                        adds.Add(new PanelMetric(item.Id)
                        {
                            Caculate = item.Caculate,
                            Name = item.Name,
                            PanelId = panel.Id,
                            Sort = item.Sort,
                            Unit = item.Unit,
                        });
                    }
                    else
                    {
                        find.Name = item.Name;
                        find.Unit = item.Unit;
                        find.Sort = item.Sort;
                        find.Caculate = item.Caculate;
                    }
                }
                var removes = metrics.Where(x => !echartPanel.Metrics.Any(t => t.Id == x.Id)).ToList();

                while (adds.Any() && removes.Any())
                {
                    var add = adds[0];
                    var remove = removes[0];
                    remove.Name = add.Name;
                    remove.Unit = add.Unit;
                    remove.Sort = add.Sort;
                    remove.Caculate = add.Caculate;
                    adds.Remove(add);
                    removes.Remove(remove);
                }

                if (removes.Any())
                    metrics.RemoveAll(x => removes.Any(item => item.Id == x.Id));

                if (removes.Any())
                    await _metricReposity.RemoveRangeAsync(removes);
                if (adds.Any())
                    await _metricReposity.AddRangeAsync(adds);
                if (metrics.Any())
                    await _metricReposity.UpdateRangeAsync(metrics);
            }
        }

        [EventHandler]
        public async Task UpdatePanelParentIdAsync(UpdatePanelParentCommond commond)
        {
            var panel = await _panelRepository.FindAsync(item => item.Id == commond.Id);
            if (panel == null)
                throw new UserFriendlyException("数据不存在");
            panel.UpdateParentId(commond.ParentId);
            await _panelRepository.UpdateAsync(panel);
        }

        [EventHandler]
        public async Task UpdatePanelWidthHeightAsync(UpdatePanelWidthHeightCommond commond)
        {
            var panel = await _panelRepository.FindAsync(item => item.Id == commond.Id);
            if (panel == null)
                throw new UserFriendlyException("数据不存在");
            panel.UpdateWidthHeight(commond.Width, commond.Height);
            await _panelRepository.UpdateAsync(panel);
        }

        [EventHandler]
        public async Task UpdatePanelSortAsync(UpdatePanelsSortCommand commond)
        {
            var list = await _panelRepository.GetListAsync(t => t.InstrumentId == commond.Id && t.ParentId == commond.ParentId);
            if (list == null)
                throw new UserFriendlyException("数据不存在");
            var index = 1;
            var updateList = new List<Panel>();
            foreach (var id in commond.PanelIds)
            {
                var panel = list.FirstOrDefault(x => x.Id == id);
                if (panel != null && panel.Sort - index != 0)
                {
                    panel.Sort = index;
                    updateList.Add(panel);
                }
                index++;
            }
            if (updateList.Any())
                await _panelRepository.UpdateRangeAsync(updateList);
        }

        [EventHandler]
        public async Task RemovePanelAsync(RemovePanelCommand command)
        {
            await RemovePanelsByIdAsync(command.PannelId);
        }

        private async Task RemovePanelsByInstrumentAsync(params Guid[] instrumentIds)
        {
            if (instrumentIds == null || !instrumentIds.Any())
                return;
            var panels = await _panelRepository.GetListAsync(item => instrumentIds.Contains(item.InstrumentId));
            if (panels == null || !panels.Any())
                return;

            var removePanels = new List<Panel>();
            foreach (var panel in panels)
            {
                var children = GetChildren(panels, panel.Id);
                if (children != null)
                    children.Add(panel);
                else
                    children = new List<Panel> { panel };
                removePanels.AddRange(children);
            }

            await _panelRepository.RemoveRangeAsync(removePanels);
            var panelIds = removePanels.Select(item => item.Id).ToArray();
            await RemoveMetricsAsync(panelIds);
        }

        private async Task RemovePanelsByIdAsync(params Guid[] panelIds)
        {
            if (panelIds == null || !panelIds.Any())
                return;
            var panels = await _panelRepository.GetListAsync(item => panelIds.Contains(item.Id));
            if (panels == null || !panels.Any())
                return;
            await RemovePanelsAysnc(panels);
        }

        private async Task RemovePanelsAysnc(IEnumerable<Panel> panels)
        {
            var removePanels = new List<Panel>();
            foreach (var panel in panels)
            {
                var children = GetChildren(panels, panel.Id);
                if (children != null)
                    children.Add(panel);
                else
                    children = new List<Panel> { panel };
                removePanels.AddRange(children);
            }

            await _panelRepository.RemoveRangeAsync(removePanels);
            var panelIds = removePanels.Select(item => item.Id).ToArray();
            await RemoveMetricsAsync(panelIds);
        }

        private async Task RemoveMetricsAsync(params Guid[] panelIds)
        {
            if (panelIds == null || !panelIds.Any())
                return;
            var metrics = await _metricReposity.GetListAsync(item => panelIds.Contains(item.PanelId));
            if (metrics != null && metrics.Any())
            {
                await _metricReposity.RemoveRangeAsync(metrics);
            }
        }

        private List<Panel> GetChildren(IEnumerable<Panel> panels, Guid parentId)
        {
            if (panels == null || !panels.Any())
                return default!;

            var list = panels.Where(item => item.ParentId == parentId).ToList();
            if (!list.Any())
                return default!;

            foreach (var panel in list)
            {
                var children = GetChildren(panels, panel.Id);
                if (children != null && children.Any())
                    list.Add(panel);
            }

            return list;
        }
    }
}