namespace ReceiptApp.Models;
public record ReceiptResponse
{
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public List<Receipt> Receipts { get; set; }
}