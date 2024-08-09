namespace ReceiptApp.Models;
public record Receipt(
    Guid Id,
    float Sum,
    DateTime Date,
    string Description,
    ExpenseType ExpenseType,
    string Location
);