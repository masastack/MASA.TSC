﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Infrastructure.Repositories;

public class InstrumentRepository : Repository<TscDbContext, Instrument, Guid>, IInstrumentRepository
{
    private readonly TscDbContext _context;

    public InstrumentRepository(TscDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
        _context = context;
    }

    public async Task<Instrument> GetAsync(Guid id, Guid userId)
    {
        return (await _context.Set<Instrument>().FirstOrDefaultAsync(item => item.Id == id))!;
    }

    public async Task<List<Instrument>> GetListAsync(IEnumerable<Guid> ids, Guid userId)
    {
        return await _context.Set<Instrument>().Where(item => ids.Contains(item.Id)).ToListAsync();
    }

    public async Task<Instrument> GetDetailAsync(Guid Id, Guid userId)
    {
        var instrument = await _context.Set<Instrument>().AsNoTracking().FirstAsync(item => item.Id == Id);
        if (instrument == null)
            return default!;

        var panels = await _context.Set<Panel>().AsNoTracking().Where(item => item.InstrumentId == Id).ToListAsync();
        if (panels != null && panels.Any())
        {
            var panelIds = panels.Select(item => item.Id).ToList();
            var metrics = await _context.Set<PanelMetric>().AsNoTracking().Where(item => panelIds.Contains(item.PanelId)).ToListAsync();
            if (metrics != null && metrics.Any())
            {
                foreach (var panel in panels)
                {
                    var matchMetrics = metrics.Where(item => item.PanelId == panel.Id).ToList();
                    if (matchMetrics.Any())
                        panel.Metrics = matchMetrics;
                }
            }

            instrument.Panels = GetChildren(panels, Guid.Empty);
        }

        return instrument;
    }

    #region update

    public async Task<Instrument> UpdateDetailAsync(Instrument instrument)
    {
        var panels = GetAllPanels(instrument.Panels);
        var originalPanels = _context.Set<Panel>().AsNoTracking().Where(item => item.InstrumentId == instrument.Id).ToList();
        UpdateMetrics(panels, originalPanels);
        UpdatePanels(panels, originalPanels);
        _context.Update(instrument);
        await _context.SaveChangesAsync();
        return instrument;
    }

    private void UpdatePanels(List<Panel> updatePanels, List<Panel> originalPanels)
    {
        var current = _context.Set<Panel>();
        if (originalPanels == null || !originalPanels.Any())
        {
            if (updatePanels != null && updatePanels.Any())
                current.AddRange(updatePanels);
        }
        else
        {
            if (updatePanels == null || !updatePanels.Any())
                current.RemoveRange(originalPanels);
            else
            {
                var orignalIds = originalPanels.Select(item => item.Id).ToList();
                var updates = updatePanels.Where(item => orignalIds.Contains(item.Id)).ToList();
                List<Guid> updateIds = new();
                if (updates.Any())
                {
                    current.UpdateRange(updates);
                    updateIds = updates.Select(item => item.Id).ToList();
                }

                var adds = updatePanels.Where(item => !orignalIds.Contains(item.Id)).ToList();
                if (adds.Any())
                    current.AddRange(adds);

                if (updateIds.Any())
                    orignalIds.RemoveAll(id => updateIds.Contains(id));

                if (orignalIds.Any())
                    current.RemoveRange(originalPanels.Where(item => orignalIds.Contains(item.Id)).ToList());
            }
        }
    }

    private void UpdateMetrics(List<Panel> updatePanels, List<Panel> originalPanels)
    {
        var all = GetAllMetrics(updatePanels);

        var current = _context.Set<PanelMetric>();
        var panelIds = originalPanels.Select(item => item.Id).ToList();
        var originals = originalPanels == null || !originalPanels.Any() ? default! : current.AsNoTracking().Where(item => panelIds.Contains(item.PanelId)).ToList();
        if (originals == null || !originals.Any())
        {
            if (all != null && all.Any())
                current.AddRange(all);
        }
        else
        {
            if (all == null || !all.Any())
                current.RemoveRange(originals);
            else
            {
                var orignalIds = originals.Select(item => item.Id).ToList();
                var updates = all.Where(item => orignalIds.Contains(item.Id)).ToList();
                List<Guid> updateIds = new();
                if (updates.Any())
                {
                    current.UpdateRange(updates);
                    updateIds = updates.Select(item => item.Id).ToList();
                }

                var adds = all.Where(item => !orignalIds.Contains(item.Id)).ToList();
                if (adds.Any())
                    current.AddRange(adds);

                if (updateIds.Any())
                    orignalIds.RemoveAll(id => updateIds.Contains(id));

                if (orignalIds.Any())
                    current.RemoveRange(originals.Where(item => orignalIds.Contains(item.Id)).ToList());
            }
        }
    }

    private List<Panel> GetChildren(IEnumerable<Panel> panels, Guid parentId)
    {
        if (panels == null || !panels.Any())
            return default!;

        var list = panels.Where(item => item.ParentId == parentId).ToList();
        if (!list.Any())
            return default!;
        if (list.Any(item => item.Type == PanelTypes.TabItem))
            list = list.OrderBy(item => item.Index).ToList();
        foreach (var panel in list)
        {
            var children = GetChildren(panels, panel.Id);
            if (children?.Any() is true)
                panel.Panels = children;
        }
        return list;
    }

    private List<Panel> GetAllPanels(List<Panel> panels)
    {
        if (panels == null || !panels.Any())
            return default!;
        var result = new List<Panel>();
        foreach (var panel in panels)
        {
            var children = GetAllPanels(panel.Panels);
            if (children == null || !children.Any())
                continue;
            result.AddRange(children);
        }
        result.InsertRange(0, panels);
        return result;
    }

    private List<PanelMetric> GetAllMetrics(List<Panel> panels)
    {
        if (panels == null || !panels.Any())
            return default!;
        var result = new List<PanelMetric>();
        foreach (var panel in panels)
        {
            if (panel.Metrics != null && panel.Metrics.Any())
                result.AddRange(panel.Metrics);
        }
        return result;
    }

    #endregion

    public override async Task RemoveRangeAsync(IEnumerable<Instrument> entities, CancellationToken cancellationToken = default)
    {
        if (entities == null || !entities.Any())
            return;
        var instrumentIds = entities.Select(item => item.Id).ToList();
        var panels = await _context.Set<Panel>().Where(m => instrumentIds.Contains(m.InstrumentId)).ToListAsync();
        if (panels != null && panels.Any())
        {
            var panelIds = panels.Select(item => item.Id).ToList();
            var metrics = await _context.Set<PanelMetric>().Where(m => panelIds.Contains(m.PanelId)).ToListAsync();
            if (metrics != null && metrics.Any())
                _context.Set<PanelMetric>().RemoveRange(metrics);

            _context.Set<Panel>().RemoveRange(panels);
        }

        _context.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }
}