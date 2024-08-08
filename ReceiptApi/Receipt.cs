record Receipt(
    Guid Id,
    float Sum,
    DateTime Date,
    string Description,
    ExpenseType ExpenseType,
    string Location
);

enum ExpenseType
{
    Food,
    Transportation,
    Entertainment,
    Utilities,
    Other
}