using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microcharts;
using ReceiptApp.Models;
using SkiaSharp;

namespace ReceiptApp.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5252") };

        [ObservableProperty]
        private Chart _expenseTypeChart;

        [ObservableProperty]
        private Chart _monthlyTotalSumChart;

        [ObservableProperty]
        private Chart _averageSumByExpenseTypeChart;

        [ObservableProperty]
        private Chart _receiptsPerDayOfWeekChart;

        [ObservableProperty]
        private Chart _topLocationsChart;

        [RelayCommand]
        public async Task LoadStatistics()
        {
            await LoadExpenseTypeStats();
            await LoadAverageSumByExpenseTypeStats();
            await LoadMonthlyTotalSumStats();
            await LoadReceiptsPerDayOfWeekStats();
            await LoadTopLocationsStats();
        }

        private async Task LoadExpenseTypeStats()
        {
            var response = await _httpClient.GetAsync("stats/count-by-expense-type");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<ExpenseTypeCount>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ExpenseTypeChart = new BarChart
                {
                    Entries = result.Select(r => new ChartEntry((float)r.Count)
                    {
                        Label = r.ExpenseType.ToString(),
                        ValueLabel = r.Count.ToString(),
                        Color = GetColorForExpenseType(r.ExpenseType)
                    }).ToArray(),
                    LabelTextSize = 40,
                    ValueLabelTextSize = 40,
                    LabelOrientation = Orientation.Horizontal,
                    ValueLabelOrientation = Orientation.Horizontal,
                    Margin = 30,
                    
                
                };
            }
        }

        private async Task LoadMonthlyTotalSumStats()
        {
            var response = await _httpClient.GetAsync("stats/total-sum-by-month");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<TotalSumByMonthAndYear>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                MonthlyTotalSumChart = new LineChart
                {
                    Entries = result.Select(r => new ChartEntry(r.TotalSum)
                    {
                        Label = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(r.Month),
                        ValueLabel = r.TotalSum.ToString(),
                    }).ToArray(),
                    LabelTextSize = 40,
                    ValueLabelTextSize = 40,
                    LabelOrientation = Orientation.Horizontal,
                    ValueLabelOrientation = Orientation.Horizontal,
                };
            }
        }

        private async Task LoadAverageSumByExpenseTypeStats()
        {
            var response = await _httpClient.GetAsync("stats/average-sum-by-expense-type");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<ExpenseTypeAverageSum>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                AverageSumByExpenseTypeChart = new BarChart
                {
                    Entries = result.Select(r => new ChartEntry(r.AverageSum)
                    {
                        Label = r.ExpenseType.ToString(),
                        ValueLabel = r.AverageSum.ToString(),
                        Color = GetColorForExpenseType(r.ExpenseType)
                    }).ToArray(),
                    LabelTextSize = 40,
                    ValueLabelTextSize = 40,
                    LabelOrientation = Orientation.Horizontal,
                    ValueLabelOrientation = Orientation.Horizontal,
                };
            }
        }

        private async Task LoadReceiptsPerDayOfWeekStats()
        {
            var response = await _httpClient.GetAsync("stats/receipts-per-day-of-week");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<ReceiptsPerDayCount>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ReceiptsPerDayOfWeekChart = new DonutChart
                {
                    Entries = result.Select(r => new ChartEntry((float)r.Count)
                    {
                        Label = r.DayOfWeek.ToString(),
                        ValueLabel = r.Count.ToString(),
                        Color = GetColorForDayOfWeek(r.DayOfWeek)
                    }).ToArray(),
                    LabelTextSize = 40,
                };
            }
        }

        private async Task LoadTopLocationsStats()
        {
            var response = await _httpClient.GetAsync("stats/top-locations");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<List<ReceiptsPerLocationCount>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                TopLocationsChart = new BarChart
                {
                    Entries = result.Select(r => new ChartEntry(r.Count)
                    {
                        Label = r.Location,
                        ValueLabel = r.Count.ToString(),
                        Color = GetRandomColor()
                    }).ToArray(),
                    LabelTextSize = 40,
                    ValueLabelTextSize = 40,
                    LabelOrientation = Orientation.Horizontal,
                    ValueLabelOrientation = Orientation.Horizontal,
                };
            }
        }
        
        private SKColor GetColorForExpenseType(ExpenseType expenseType)
        {
            switch (expenseType)
            {
                case ExpenseType.Food:
                    return SKColor.Parse("#FFF0F5");
                case ExpenseType.Transportation:
                    return SKColor.Parse("#E0FFFF");
                case ExpenseType.Entertainment:
                    return SKColor.Parse("#F0FFF0");
                case ExpenseType.Utilities:
                    return SKColor.Parse("#FFF8DC");
                case ExpenseType.Other:
                    return SKColor.Parse("#F5F5F5");
                default:
                    return SKColor.Parse("#FFFFFF");
            }
        }

        private SKColor GetColorForDayOfWeek(DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    return SKColor.Parse("#FFF0F5");
                case DayOfWeek.Tuesday:
                    return SKColor.Parse("#E0FFFF");
                case DayOfWeek.Wednesday:
                    return SKColor.Parse("#F0FFF0");
                case DayOfWeek.Thursday:
                    return SKColor.Parse("#FFF8DC");
                case DayOfWeek.Friday:
                    return SKColor.Parse("#F5F5F5");
                case DayOfWeek.Saturday:
                    return SKColor.Parse("#ADD8E6");
                case DayOfWeek.Sunday:
                    return SKColor.Parse("#FFB6C1");
                default:
                    return SKColor.Parse("#FFFFFF");
            }
        }

        private SKColor GetRandomColor()
        {
            var random = new Random();
            return SKColor.Parse($"#{random.Next(0x1000000):X6}");
        }
    }
}