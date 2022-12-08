// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Tsc.Service.Admin.Domain.Aggregates;

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

    public List<Panel> Panels { get; set; }

    public Instrument(Guid Id) : base(Id) { }

    public Instrument() { }

    public void Update(UpdateDashboardDto pannel)
    {
        if (pannel.Name != Name)
            Name = pannel.Name;
        Layer = pannel.Layer.ToString();
        Model = pannel.Model.ToString();
        Lable = pannel.Type.ToString();
    }

    public Panel AddPanel(PanelDto model)
    {
        var panel = Panel.ConvertTo(model);
        if (Panels == null)
            Panels = new List<Panel> { panel };
        else
            Panels.Add(panel);
        return panel;
    }

    public Panel RemovePanel(Guid panelId)
    {
        if (Panels != null && Panels.Any(p => p.Id == panelId))
        {
            var find = Panels.First(p => p.Id == panelId);
            Panels.Remove(find);
            return find;
        }
        return default!;
    }

    public void UpdatePanelsShow(UpdatePanelShowDto[] data)
    {
        if (Panels == null || !Panels.Any() || data == null || !data.Any())
            return;

        foreach (var item in data)
        {
            var panel = Panels.FirstOrDefault(p => p.Id == item.Id);
            if (panel == null)
                continue;
            panel.UpdateShow(item);
        }
    }

    public void SetRoot(bool isRoot)
    {
        if (IsRoot == isRoot)
            return;
        IsRoot = !IsRoot;
    }
}