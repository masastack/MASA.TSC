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
    #endregion

    #region upsert

    public async Task UpsertInstrumentAsync(UpInsertCommand command)
    {
        var entry = await _instrumentRepository.GetDetailAsync(command.InstumentId, command.UserId);
        if (entry == null)
            throw new UserFriendlyException($"instrument {command.InstumentId} is not exists");
        entry.UpdatePanels(command.Data);
        await _instrumentRepository.UpdateAsync(entry);
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