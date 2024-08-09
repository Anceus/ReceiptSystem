using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using ReceiptApp.Models;

namespace ReceiptApp.Converters
{
    public class ExpenseTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ExpenseType expenseType)
            {
                return expenseType switch
                {
                    ExpenseType.Food => Colors.LightPink,
                    ExpenseType.Transportation => Colors.LightBlue,
                    ExpenseType.Entertainment => Colors.LightGreen,
                    ExpenseType.Utilities => Colors.LightYellow,
                    ExpenseType.Other => Colors.LightGray,
                    _ => Colors.White,
                };
            }
            return Colors.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}