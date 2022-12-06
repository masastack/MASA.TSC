// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Application.Instruments;

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

    #region add

    [EventHandler]
    public async Task AddInstrumentAsync(AddInstrumentCommand command)
    {
        var model = new Instrument
        {
            Name = command.Data.Name,
            Layer = command.Data.Layer.ToString(),
            IsRoot = command.Data.IsRoot,
            DirectoryId = command.Data.Folder,
            Model = command.Data.Model.ToString(),
            Sort = command.Data.Order,
            Lable = command.Data.Type.ToString()
        };
        await _instrumentRepository.AddAsync(model);
    }

    /// <summary>
    /// add pannel
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [EventHandler(0)]
    public async Task AddPanelAsync(AddPanelCommand command)
    {
        var instrument = await _instrumentRepository.GetIncludePanelsAsync(command.Data.InstrumentId, command.UserId);
        if (instrument == null)
            throw new UserFriendlyException("数据不存在");
        instrument.AddPanel(command.Data);

        await _instrumentRepository.UpdateAsync(instrument);
    }

    #endregion

    #region update

    [EventHandler]
    public async Task UpdateInstrumentAsync(UpdateInstrumentCommand command)
    {
        var entry = await _instrumentRepository.GetAsync(command.Data.Id, command.UserId);
        if (entry == null)
            throw new UserFriendlyException($"instrument {command.Data.Id} is not exists");
        entry.Update(command.Data);
        await _instrumentRepository.UpdateAsync(entry);
    }

    [EventHandler]
    public async Task SetRootAsync(SetRootCommand command)
    {
        var instrument = await _instrumentRepository.GetAsync(command.Id, command.UserId);
        if (instrument == null)
            throw new UserFriendlyException("数据不存在");

        instrument.SetRoot(command.IsRoot);
        await _instrumentRepository.UpdateAsync(instrument);
    }

    [EventHandler]
    public async Task UpdatePanelsShowAsync(UpdatePanelsShowCommand command)
    {
        var instrument = await _instrumentRepository.GetIncludePanelsAsync(command.Data.Id, command.UserId);
        if (instrument == null)
            throw new UserFriendlyException($"instrument {command.Data.Id} is not exists");

        instrument.UpdatePanelsShow(command.Data.Panels);
        await _instrumentRepository.UpdateAsync(instrument);
    }

    [EventHandler(0)]
    public async Task UpdatePanelAsync(UpdatePanelCommand command)
    {
        var instrument = await _instrumentRepository.GetIncludePanelsAsync(command.Data.InstrumentId, command.UserId);
        if (instrument == null)
            throw new UserFriendlyException($"instrument {command.Data.InstrumentId} is not exists");

        var panel = instrument.Panels.FirstOrDefault(p => p.Id == command.Data.Id);
        if (panel == null)
            throw new UserFriendlyException($"panel {command.Data.Id} is not exists");
        panel.Update(command.Data);

        await _instrumentRepository.UpdateAsync(instrument);
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
    #endregion

    #region  remove

    [EventHandler]
    public async Task RemoveInstrumentAsync(RemoveInstrumentCommand command)
    {
        var list = await _instrumentRepository.GetListAsync(command.InstrumentIds, command.UserId);
        if (list == null || !list.Any())
            throw new UserFriendlyException("数据不存在");
        var instrumentIds = list.Select(m => m.Id).ToArray();
        await RemovePanelsByInstrumentAsync(instrumentIds);
        await _instrumentRepository.RemoveRangeAsync(list);
    }

    [EventHandler]
    public async Task RemovePanelAsync(RemovePanelCommand command)
    {
        var instrument = await _instrumentRepository.GetIncludePanelsAsync(command.PannelId, command.UserId);
        if (instrument == null)
            throw new UserFriendlyException("数据不存在");
        var panel = instrument.RemovePanel(command.PannelId);
        if (panel != null)
        {
            await RemovePanelsByIdAsync(command.PannelId);
            await _panelRepository.RemoveAsync(panel);
        }
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

    #endregion    
}