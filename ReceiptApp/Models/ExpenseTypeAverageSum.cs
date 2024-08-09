namespace ReceiptApp.Models;
public record ExpenseTypeAverageSum
{
    public ExpenseType ExpenseType { get; set; }
    public float AverageSum { get; set; }
}