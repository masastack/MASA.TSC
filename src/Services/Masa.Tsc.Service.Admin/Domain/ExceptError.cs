﻿namespace Masa.Tsc.Service.Admin.Domain;

internal class ExceptError : FullAggregateRoot<string, Guid>
{
    public ExceptError()
    {
        Id = Guid.NewGuid().ToString("N").ToLower();
    }

    public ExceptError(string id)
    {
        if (Guid.TryParse(id, out var guid) && guid != Guid.Empty)
            Id = guid.ToString("N").ToLower();
    }    

    public string Environment { get; set; }

    public string Project { get; set; }

    public string Service { get; set; }

    public string Type { get; set; }

    public string Message { get; set; }

    public string Comment { get; set; }
}
