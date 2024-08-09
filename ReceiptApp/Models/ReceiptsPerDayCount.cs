namespace ReceiptApp.Models;
public record ReceiptsPerDayCount
{
    public DayOfWeek DayOfWeek { get; set; }
    public int Count { get; set; }
}