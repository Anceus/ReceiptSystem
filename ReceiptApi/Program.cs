using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ReceiptApi", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

const string ReceiptsFileName = "receipts.json";

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

List<Receipt> LoadReceipts()
{
    if (!File.Exists(ReceiptsFileName))
    {
        return new List<Receipt>();
    }
    
    var json = File.ReadAllText(ReceiptsFileName);
    return JsonSerializer.Deserialize<List<Receipt>>(json) ?? new List<Receipt>();
}

void SaveReceipts(List<Receipt> receipts)
{
    var json = JsonSerializer.Serialize(receipts);
    File.WriteAllText(ReceiptsFileName, json);
}

// CRUD Operations

app.MapGet("/receipts", (
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? orderBy = null,
    [FromQuery] float? minSum = null,
    [FromQuery] float? maxSum = null,
    [FromQuery] DateTime? startDate = null,
    [FromQuery] DateTime? endDate = null,
    [FromQuery] ExpenseType? expenseType = null) =>
{
    var receipts = LoadReceipts();
    
    // Apply filters
    if (minSum.HasValue)
        receipts = receipts.Where(r => r.Sum >= minSum.Value).ToList();
    if (maxSum.HasValue)
        receipts = receipts.Where(r => r.Sum <= maxSum.Value).ToList();
    if (startDate.HasValue)
        receipts = receipts.Where(r => r.Date >= startDate.Value).ToList();
    if (endDate.HasValue)
        receipts = receipts.Where(r => r.Date <= endDate.Value).ToList();
    if (expenseType.HasValue)
        receipts = receipts.Where(r => r.ExpenseType == expenseType.Value).ToList();

    // Apply ordering
    if (!string.IsNullOrEmpty(orderBy))
    {
        receipts = orderBy.ToLower() switch
        {
            "sum" => receipts.OrderBy(r => r.Sum).ToList(),
            "date" => receipts.OrderBy(r => r.Date).ToList(),
            "expensetype" => receipts.OrderBy(r => r.ExpenseType).ToList(),
            _ => receipts
        };
    }

    // Apply paging
    var totalCount = receipts.Count;
    var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    receipts = receipts.Skip((page - 1) * pageSize).Take(pageSize).ToList();

    return new
    {
        TotalCount = totalCount,
        TotalPages = totalPages,
        CurrentPage = page,
        PageSize = pageSize,
        Receipts = receipts
    };
})
.WithName("GetReceipts")
.WithOpenApi();

app.MapGet("/receipts/{id}", (Guid id) =>
{
    var receipts = LoadReceipts();
    var receipt = receipts.FirstOrDefault(r => r.Id == id);
    return receipt is null ? Results.NotFound() : Results.Ok(receipt);
})
.WithName("GetReceiptById")
.WithOpenApi();

app.MapPost("/receipts", (Receipt receipt) =>
{
    var receipts = LoadReceipts();
    receipts.Add(receipt with { Id = Guid.NewGuid() });
    SaveReceipts(receipts);
    return Results.Created($"/receipts/{receipt.Id}", receipt);
})
.WithName("CreateReceipt")
.WithOpenApi();

app.MapPut("/receipts/{id}", (Guid id, Receipt updatedReceipt) =>
{
    var receipts = LoadReceipts();
    var index = receipts.FindIndex(r => r.Id == id);
    if (index == -1)
        return Results.NotFound();

    receipts[index] = updatedReceipt with { Id = id };
    SaveReceipts(receipts);
    return Results.NoContent();
})
.WithName("UpdateReceipt")
.WithOpenApi();

app.MapDelete("/receipts/{id}", (Guid id) =>
{
    var receipts = LoadReceipts();
    var receipt = receipts.FirstOrDefault(r => r.Id == id);
    if (receipt is null)
        return Results.NotFound();

    receipts.Remove(receipt);
    SaveReceipts(receipts);
    return Results.NoContent();
})
.WithName("DeleteReceipt")
.WithOpenApi();

// Statistical Endpoints

app.MapGet("/stats/count-by-expense-type", () =>
{
    var receipts = LoadReceipts();
    return receipts.GroupBy(r => r.ExpenseType)
                   .Select(g => new { ExpenseType = g.Key, Count = g.Count() });
})
.WithName("GetReceiptCountByExpenseType")
.WithOpenApi();

app.MapGet("/stats/total-sum-by-month", () =>
{
    var receipts = LoadReceipts();
    return receipts.GroupBy(r => new { r.Date.Year, r.Date.Month })
                   .Select(g => new { Year = g.Key.Year, Month = g.Key.Month, TotalSum = g.Sum(r => r.Sum) })
                   .OrderBy(r => r.Year).ThenBy(r => r.Month);
})
.WithName("GetTotalSumByMonth")
.WithOpenApi();

app.MapGet("/stats/average-sum-by-expense-type", () =>
{
    var receipts = LoadReceipts();
    return receipts.GroupBy(r => r.ExpenseType)
                   .Select(g => new { ExpenseType = g.Key, AverageSum = g.Average(r => r.Sum) });
})
.WithName("GetAverageSumByExpenseType")
.WithOpenApi();

app.MapGet("/stats/receipts-per-day-of-week", () =>
{
    var receipts = LoadReceipts();
    return receipts.GroupBy(r => r.Date.DayOfWeek)
                   .Select(g => new { DayOfWeek = g.Key, Count = g.Count() })
                   .OrderBy(r => r.DayOfWeek);
})
.WithName("GetReceiptsPerDayOfWeek")
.WithOpenApi();

app.MapGet("/stats/top-locations", () =>
{
    var receipts = LoadReceipts();
    return receipts.GroupBy(r => r.Location)
                   .Select(g => new { Location = g.Key, Count = g.Count() })
                   .OrderByDescending(r => r.Count)
                   .Take(5);
})
.WithName("GetTopLocations")
.WithOpenApi();

// Generate mock data
if (!File.Exists(ReceiptsFileName))
{
    var random = new Random();
    var mockReceipts = Enumerable.Range(1, 100).Select(_ => new Receipt(
        Id: Guid.NewGuid(),
        Sum: (float)Math.Round(random.NextDouble() * 1000, 2),
        Date: DateTime.Now.AddDays(-random.Next(365)),
        Description: $"Mock receipt {random.Next(1000)}",
        ExpenseType: (ExpenseType)random.Next(5),
        Location: $"Location {random.Next(10)}"
    )).ToList();
    SaveReceipts(mockReceipts);
}

app.Run();