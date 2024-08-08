using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microcharts;
using SkiaSharp;
using Microsoft.Maui.Controls.Xaml;

namespace ReceiptApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new MainViewModel();
    }


}

public partial class MainViewModel : ObservableObject
{
    private List<ExpenseType> _expenseTypes;
    public List<ExpenseType> ExpenseTypes
    {
        get { return _expenseTypes; }
        set { SetProperty(ref _expenseTypes, value); }
    }

    private List<string> _orderByOptions;
    public List<string> OrderByOptions
    {
        get { return _orderByOptions; }
        set { SetProperty(ref _orderByOptions, value); }
    }
    
    private bool _canGoToPreviousPage;
    public bool CanGoToPreviousPage
    {
        get { return _canGoToPreviousPage; }
        set { SetProperty(ref _canGoToPreviousPage, value); }
    }

    private bool _canGoToNextPage;
    public bool CanGoToNextPage
    {
        get { return _canGoToNextPage; }
        set { SetProperty(ref _canGoToNextPage, value); }
    }


    private readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5252") };

    [ObservableProperty]
    private ObservableCollection<Receipt> receipts = new();

    [ObservableProperty]
    private int currentPage = 1;

    [ObservableProperty]
    private int pageSize = 10;

    [ObservableProperty]
    private int totalPages;

    [ObservableProperty]
    private float? minSum;

    [ObservableProperty]
    private float? maxSum;

    [ObservableProperty]
    private DateTime? startDate;

    [ObservableProperty]
    private DateTime? endDate;

    [ObservableProperty]
    private ExpenseType? selectedExpenseType;

    [ObservableProperty]
    private string orderBy = "date";

    [ObservableProperty]
    private Receipt editingReceipt;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    private Chart expenseTypeChart;

    [ObservableProperty]
    private Chart monthlyTotalSumChart;

    [ObservableProperty]
    private Chart averageSumByExpenseTypeChart;

    [ObservableProperty]
    private Chart receiptsPerDayOfWeekChart;

    [ObservableProperty]
    private Chart topLocationsChart;

    public MainViewModel()
    {
        LoadReceiptsCommand.Execute(null);
        LoadStatisticsCommand.Execute(null);

        ExpenseTypes = Enum.GetValues(typeof(ExpenseType)).Cast<ExpenseType>().ToList();
        OrderByOptions = new List<string> { "date", "sum", "expensetype" };     
    }

    [RelayCommand]
    private async Task LoadReceipts()
    {
        var query = $"receipts?page={CurrentPage}&pageSize={PageSize}";
        if (MinSum.HasValue) query += $"&minSum={MinSum}";
        if (MaxSum.HasValue) query += $"&maxSum={MaxSum}";
        if (StartDate.HasValue) query += $"&startDate={StartDate:yyyy-MM-dd}";
        if (EndDate.HasValue) query += $"&endDate={EndDate:yyyy-MM-dd}";
        if (SelectedExpenseType.HasValue) query += $"&expenseType={SelectedExpenseType}";
        query += $"&orderBy={OrderBy}";

        var response = await _httpClient.GetAsync(query);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ReceiptResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Receipts = new ObservableCollection<Receipt>(result.Receipts);
            TotalPages = result.TotalPages;
            CanGoToPreviousPage = CurrentPage > 1;
            CanGoToNextPage = CurrentPage < TotalPages;     
        }
    }

    [RelayCommand]
    private async Task PreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await LoadReceipts();
        }
    }

    [RelayCommand]
    private async Task NextPage()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            await LoadReceipts();
        }
    }

    [RelayCommand]
    private async Task DeleteReceipt(Receipt receipt)
    {
        var result = await Application.Current.MainPage.DisplayAlert("Confirm Delete", "Are you sure you want to delete this receipt?", "Yes", "No");
        if (result)
        {
            var response = await _httpClient.DeleteAsync($"receipts/{receipt.Id}");
            if (response.IsSuccessStatusCode)
            {
                Receipts.Remove(receipt);
            }
        }
    }

    [RelayCommand]
    private void EditReceipt(Receipt receipt)
    {
        EditingReceipt = receipt;
        IsEditing = true;
    }

    [RelayCommand]
    private async Task SaveReceipt()
    {
        var response = await _httpClient.PutAsJsonAsync($"receipts/{EditingReceipt.Id}", EditingReceipt);
        if (response.IsSuccessStatusCode)
        {
            var index = Receipts.IndexOf(Receipts.First(r => r.Id == EditingReceipt.Id));
            Receipts[index] = EditingReceipt;
        }
        IsEditing = false;
        EditingReceipt = null;
    }

    [RelayCommand]
    private void CancelEdit()
    {
        IsEditing = false;
        EditingReceipt = null;
    }

    [RelayCommand]
    private async Task LoadStatistics()
    {
        await LoadExpenseTypeStats();
        //await LoadMonthlyTotalSumStats();
        //await LoadAverageSumByExpenseTypeStats();
        //await LoadReceiptsPerDayOfWeekStats();
        //await LoadTopLocationsStats();
    }
    

    private async Task LoadExpenseTypeStats()
    {
        var response = await _httpClient.GetAsync("stats/count-by-expense-type");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<ExpenseTypeCount>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            ExpenseTypeChart = new PieChart
            {
                Entries = result.Select(r => new ChartEntry(r.Count)
                {
                    Label = r.ExpenseType.ToString(),
                    Color = SKColor.Parse(GetRandomColor())
                }).ToArray()
            };
        }
    }

    //TODO: Implement methods 4 other statistical charts

    private string GetRandomColor()
    {
        var random = new Random();
        return $"#{random.Next(0x1000000):X6}"; }
}

public record Receipt(
    Guid Id,
    float Sum,
    DateTime Date,
    string Description,
    ExpenseType ExpenseType,
    string Location
);

public enum ExpenseType {
    Food, Transportation,
    Entertainment,
    Utilities,
    Other
}

public class ReceiptResponse
{
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public List<Receipt> Receipts { get; set; }
}

public class ExpenseTypeCount
{
    public ExpenseType ExpenseType { get; set; }
    public int Count { get; set; }
}
