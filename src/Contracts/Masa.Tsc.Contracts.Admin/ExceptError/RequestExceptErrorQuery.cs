namespace Masa.Tsc.Contracts.Admin;

public class RequestExceptErrorQuery : RequestPageBase
{
    public string SortField { get; set; }

    public bool SortDesc { get; set; }

    public string Environment { get; set; }

    public string Project { get; set; }

    public string Service { get; set; }

    public string Type { get; set; }

    public string Message { get; set; }
}