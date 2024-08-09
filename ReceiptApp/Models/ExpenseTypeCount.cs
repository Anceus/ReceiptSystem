namespace ReceiptApp.Models;
public record ExpenseTypeCount
{
    public ExpenseType ExpenseType { get; set; }
    public int Count { get; set; }
}