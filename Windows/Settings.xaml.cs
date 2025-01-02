using System.Windows;
using ZarzadzanieFinansami;

namespace ZarządzanieFinansami.Windows;

public partial class Settings : Window
{
    public Settings()
    {
        var selectedCurrency = Currency.EUR;
        InitializeComponent();
        CurrencyPicker.SelectedIndex = (int)selectedCurrency;
    }
    
}