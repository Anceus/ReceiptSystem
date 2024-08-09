using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReceiptApp.Models;

namespace ReceiptApp.ViewModels
{
    public partial class ReceiptsViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5252") };

        [ObservableProperty]
        private bool _canGoToPreviousPage;

        [ObservableProperty]
        private bool _canGoToNextPage = true;

        [ObservableProperty]
         private ObservableCollection<ReceiptWrapper> _receipts = new();

        [ObservableProperty]
        private int _currentPage = 1;

        [ObservableProperty]
        private int _pageSize = 10;

        [ObservableProperty]
        private int _totalPages;

        [ObservableProperty]
        private float? _minSum;

        [ObservableProperty]
        private float? _maxSum;

        [ObservableProperty]
        private DateTime? _startDate;

        [ObservableProperty]
        private DateTime? _endDate;

        public List<ExpenseType> ExpenseTypes { get; } = Enum.GetValues(typeof(ExpenseType)).Cast<ExpenseType>().ToList();

        [ObservableProperty]
        private string _selectedExpenseType = "All";

        public List<string> OrderByOptions { get; } = new List<string> { "sum", "date", "expensetype" };
     
        [ObservableProperty]
        private string _orderBy = "date";


        [RelayCommand]
        public async Task LoadReceipts()
        {
            var query = $"receipts?page={CurrentPage}&pageSize={PageSize}";
            if (_minSum.HasValue) query += $"&minSum={_minSum}";
            if (_maxSum.HasValue) query += $"&maxSum={_maxSum}";
            if (_startDate.HasValue) query += $"&startDate={_startDate:yyyy-MM-dd}";
            if (_endDate.HasValue) query += $"&endDate={_endDate:yyyy-MM-dd}";
            if (_selectedExpenseType != "All") query += $"&expenseType={_selectedExpenseType}";
            query += $"&orderBy={_orderBy}";

            var response = await _httpClient.GetAsync(query);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ReceiptResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                Receipts = new ObservableCollection<ReceiptWrapper>(result.Receipts.Select(r => new ReceiptWrapper(r)));

                OnPropertyChanged(nameof(Receipts));

                TotalPages = result.TotalPages;
                CanGoToNextPage = CurrentPage < TotalPages;
                CanGoToPreviousPage = CurrentPage > 1;

                OnPropertyChanged(nameof(CanGoToPreviousPage));
                OnPropertyChanged(nameof(CanGoToNextPage));
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(TotalPages));
                
            }
        }

        [RelayCommand]
        public void ClearFilters()
        {
            MinSum = null;
            MaxSum = null;
            StartDate = null;
            EndDate = null;
            SelectedExpenseType = "All";
            OrderBy = "date";
        }

        [RelayCommand]
        public async Task PreviousPage()
        {
            if (CanGoToPreviousPage)
            {
                CurrentPage--;
                await LoadReceipts();
            }
        }

        [RelayCommand]
        public async Task NextPage()
        {
            if (CanGoToNextPage)
            {
                CurrentPage++;
                await LoadReceipts();
            }
        }

        [RelayCommand]
        public async Task DeleteReceipt(ReceiptWrapper receiptWrapper)
        {
            var result = await Application.Current.MainPage.DisplayAlert("Confirm Delete", "Are you sure you want to delete this receipt?", "Yes", "No");
            if (result)
            {
                var response = await _httpClient.DeleteAsync($"receipts/{receiptWrapper.Id}");
                if (response.IsSuccessStatusCode)
                {
                    Receipts.Remove(receiptWrapper);
                }
            }
        }

        [RelayCommand]
        public void EditReceipt(ReceiptWrapper receiptWrapper)
        {
            receiptWrapper.IsEditing = true;
            receiptWrapper.SaveOriginalValues();
        }

        [RelayCommand]
        public async Task SaveReceipt(ReceiptWrapper receiptWrapper)
        {
            var updatedReceipt = new Receipt(
                receiptWrapper.Id,
                receiptWrapper.Sum,
                receiptWrapper.Date,
                receiptWrapper.Description,
                receiptWrapper.ExpenseType,
                receiptWrapper.Location
            );

            var response = await _httpClient.PutAsJsonAsync($"receipts/{receiptWrapper.Id}", updatedReceipt);
            if (response.IsSuccessStatusCode)
            {
                receiptWrapper.IsEditing = false;
                receiptWrapper.ClearOriginalValues();
            }
        }

        [RelayCommand]
        public void CancelEdit(ReceiptWrapper receiptWrapper)
        {
            receiptWrapper.RestoreOriginalValues();
            receiptWrapper.IsEditing = false;
        }
    }
}