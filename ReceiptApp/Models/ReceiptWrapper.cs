using CommunityToolkit.Mvvm.ComponentModel;

namespace ReceiptApp.Models;

public partial class ReceiptWrapper : ObservableObject
    {
        private Receipt _originalReceipt;
        private Receipt _editingReceipt;

        public ReceiptWrapper(Receipt receipt)
        {
            _originalReceipt = receipt;
            _editingReceipt = new Receipt(
                receipt.Id,
                receipt.Sum,
                receipt.Date,
                receipt.Description,
                receipt.ExpenseType,
                receipt.Location
            );
        }

        public Guid Id => _editingReceipt.Id;
        public float Sum
        {
            get => _editingReceipt.Sum;
            set
            {
                if (_editingReceipt.Sum != value)
                {
                    _editingReceipt = _editingReceipt with { Sum = value };
                    OnPropertyChanged(nameof(Sum));
                }
            }
        }
        public DateTime Date
        {
            get => _editingReceipt.Date;
            set
            {
                if (_editingReceipt.Date != value)
                {
                    _editingReceipt = _editingReceipt with { Date = value };
                    OnPropertyChanged(nameof(Date));
                }
            }
        }
        public string Description
        {
            get => _editingReceipt.Description;
            set
            {
                if (_editingReceipt.Description != value)
                {
                    _editingReceipt = _editingReceipt with { Description = value };
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
        
        public ExpenseType ExpenseType
        {
            get => _editingReceipt.ExpenseType;
            set
            {
                if (_editingReceipt.ExpenseType != value)
                {
                    _editingReceipt = _editingReceipt with { ExpenseType = value };
                    OnPropertyChanged(nameof(ExpenseType));
                }
            }
        }
        public string Location
        {
            get => _editingReceipt.Location;
            set
            {
                if (_editingReceipt.Location != value)
                {
                    _editingReceipt = _editingReceipt with { Location = value };
                    OnPropertyChanged(nameof(Location));
                }
            }
        }
        

        [ObservableProperty]
        private bool _isEditing;

        public void SaveOriginalValues()
        {
            _originalReceipt = _editingReceipt;
        }

        public void RestoreOriginalValues()
        {
            _editingReceipt = _originalReceipt;
            OnPropertyChanged(nameof(Sum));
            OnPropertyChanged(nameof(Date));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(ExpenseType));
            OnPropertyChanged(nameof(Location));
        }

        public void ClearOriginalValues()
        {
            _originalReceipt = null;
        }
    }