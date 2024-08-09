namespace ReceiptApp.Models;
public record TotalSumByMonthAndYear
{
    public int Year { get; set; }
    public int Month { get; set; }
    public float TotalSum { get; set; }
}