using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using ReceiptApp.Models;
using ReceiptApp.ViewModels;
using SkiaSharp;

namespace ReceiptApp
{
    public partial class ReceiptsPage : ContentPage
    {
        private readonly ReceiptsViewModel _viewModel;

        public ReceiptsPage()
        {
            InitializeComponent();
            _viewModel = new ReceiptsViewModel();
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.LoadReceiptsCommand.Execute(null);
        }

        private void EditReceipt(object sender, EventArgs e)
        {
            var receipt = (ReceiptWrapper)((Button)sender).BindingContext;
            _viewModel.EditReceipt(receipt);
        }

        private async void SaveReceipt(object sender, EventArgs e)
        {
            var receipt = (ReceiptWrapper)((Button)sender).BindingContext;
            await _viewModel.SaveReceipt(receipt);
        }

        private void CancelEdit(object sender, EventArgs e)
        {
            var receipt = (ReceiptWrapper)((Button)sender).BindingContext;
            _viewModel.CancelEdit(receipt);
        }
        private async void DeleteReceipt(object sender, EventArgs e)
        {
            var receipt = (ReceiptWrapper)((Button)sender).BindingContext;
            await _viewModel.DeleteReceipt(receipt);
        }
    }
}