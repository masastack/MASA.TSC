namespace Masa.Tsc.Web.Admin.Rcl.Components;

public record IntervalItem(string Text, TimeSpan TimeSpan)
{
    public static IntervalItem Off = new ("Off", TimeSpan.Zero);
    public static IntervalItem TenSecond = new ("10s", TimeSpan.FromSeconds(10));
    public static IntervalItem ThirtySecond = new ("30s", TimeSpan.FromSeconds(30));
    public static IntervalItem OneMinute = new IntervalItem("1m", TimeSpan.FromMinutes(1));
    public static IntervalItem FiveMinute = new IntervalItem("5m", TimeSpan.FromMinutes(5));
    public static IntervalItem FifteenMinute = new IntervalItem("15m", TimeSpan.FromMinutes(15));
    public static IntervalItem ThirtyMinute = new IntervalItem("30m", TimeSpan.FromMinutes(30));
    public static IntervalItem OneHour = new IntervalItem("1h", TimeSpan.FromHours(1));
    public static IntervalItem TwoHour = new IntervalItem("2h", TimeSpan.FromHours(2));
    public static IntervalItem OneDay = new IntervalItem("1d", TimeSpan.FromDays(1));
    
    public static List<IntervalItem> Items = new ()
    {
        Off, TenSecond, ThirtySecond, OneMinute, FiveMinute, FifteenMinute, ThirtyMinute, OneHour, TwoHour, OneDay
    };
}