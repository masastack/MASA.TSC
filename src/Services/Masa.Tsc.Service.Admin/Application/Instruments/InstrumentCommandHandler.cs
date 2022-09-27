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
        //private readonly IDomainEventBus _domainEventBus;

        public InstrumentCommandHandler(IDirectoryRepository directoryRepository,
        IInstrumentRepository instrumentRepository,
        IPanelRepository panelRepository,
        IMetricReposity metricReposity
        //IDomainEventBus domainEventBus
            )
        {
            _directoryRepository = directoryRepository;
            _instrumentRepository = instrumentRepository;
            _panelRepository = panelRepository;
            _metricReposity = metricReposity;
            //_domainEventBus = domainEventBus;
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

        [EventHandler]
        public async Task AddPanelAsync(AddPanelCommand command)
        {
            var entity = new Panel(command.Data.Id)
            {
                Type = command.Data.Type,
                Title = command.Data.Title,
                Description = command.Data.Description,
                Sort = command.Data.Sort,
                InstrumentId = command.Data.InstrumentId,
                ParentId = command.Data.ParentId
            };
            await _panelRepository.AddAsync(entity);
            //if (command.Data.Metrics != null && command.Data.Metrics.Any())
            //{
            //    var list = command.Data.Metrics.Select(x => new PanelMetric
            //    {
            //        DisplayName = x.Name,
            //        Name = x.Name,
            //        PanelId = entity.Id,
            //        Sort = x.Sort,
            //        Unit = x.Unit,
            //        Value = x.Value
            //    });

            //    await _metricReposity.AddRangeAsync(list);
            //}
        }

        [EventHandler]
        public async Task UpdatePanelAsync(UpdatePanelCommand command)
        {
            var panel = await _panelRepository.FindAsync(item => item.Id == command.Data.Id);
            if (panel == null)
                throw new UserFriendlyException("数据不存在");
            panel.Update(command.Data);
            await _panelRepository.UpdateAsync(panel);
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