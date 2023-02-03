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

    public Instrument() { }

    public Instrument(Guid Id) : base(Id) { }

    public void Update(UpdateDashboardDto dashbord)
    {
        if (dashbord.Name != Name)
            Name = dashbord.Name;
        Layer = dashbord.Layer.ToString();
        Model = dashbord.Model.ToString();
        Lable = dashbord.Type.ToString();
        Sort = dashbord.Order;
        DirectoryId = dashbord.Folder;
        IsRoot = dashbord.IsRoot;
    }

    public void UpdatePanels(UpsertPanelDto[] data)
    {
        if ((data == null || !data.Any()) && Panels != null && this.Panels.Any())
        {
            Panels.Clear();
            return;
        }

        if (Panels == null)
            Panels = new();
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