﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Domain.Shared.Entities;

public class Instrument : FullAggregateRoot<Guid, Guid>
{
    public Directory Directory { get; set; }

    public string Name { get; set; }

    public string Layer { get; set; }

    public string Model { get; set; }

    public int Sort { get; set; }

    public bool IsRoot { get; set; }

    public string Lable { get; set; }

    public Guid DirectoryId { get; set; }

    public bool IsGlobal { get; set; }

    public bool EnableEdit { get; set; } = true;

    public List<Panel> Panels { get; set; }

    public Instrument() { }

    public Instrument(Guid id) : base(id) { }

    public void Update(UpdateDashboardDto dashboard)
    {
        if (dashboard.Name != Name)
            Name = dashboard.Name;
        Layer = dashboard.Layer;
        Model = dashboard.Model.ToString();
        Lable = dashboard.Type;
        Sort = dashboard.Order;
        DirectoryId = dashboard.Folder;
        IsRoot = dashboard.IsRoot;
    }

    public void UpdatePanels(UpsertPanelDto[] data)
    {
        if ((data == null || !data.Any()) && Panels != null && Panels.Any())
        {
            Panels.Clear();
            return;
        }

        Panels ??= new();
        var list = new List<Panel>();
        foreach (var item in data!)
        {
            var panel = Panels.FirstOrDefault(p => p.Id == item.Id);
            if (panel == null)
            {
                panel = new Panel(item, Id, Guid.Empty);
            }
            else
            {
                Panels.Remove(panel);
            }
            panel.Update(item);
            list.Add(panel);
        }
        Panels = list;
    }

    public void SetRoot(bool isRoot)
    {
        if (IsRoot == isRoot)
            return;
        IsRoot = !IsRoot;
    }
}