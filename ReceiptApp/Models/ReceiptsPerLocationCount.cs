namespace ReceiptApp.Models;
public record ReceiptsPerLocationCount
{
    public string Location { get; set; }
    public int Count { get; set; }
}