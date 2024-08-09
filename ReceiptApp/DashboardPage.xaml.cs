using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using ReceiptApp.ViewModels;

namespace ReceiptApp
{
    public partial class DashboardPage : ContentPage
    {
        private readonly DashboardViewModel _viewModel;

        public DashboardPage()
        {
            InitializeComponent();
            _viewModel = new DashboardViewModel();
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadStatistics();
        }
    }
}