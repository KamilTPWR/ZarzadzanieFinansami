using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZarzadzanieFinansami;

namespace ZarządzanieFinansami.Windows;

public partial class Settings : Window
{
    private bool _fFirstTimeImput;
    private string _kwota;

    public Settings()
    {
        SettingsUtility.LoadSettings(); 
        InitializeComponent();
        CurrencyPicker.SelectedIndex = (int)Core.GlobalCurrency;
        Box.Text = StrUtility.FormatValue(Core.GlobalSaldo);
    }
    
    private void Box_OnGotFocus(object sender, RoutedEventArgs e)
    {
        TextBox textBox = (sender as TextBox)!;
        textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
    }
    
    private void Box_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        TextBox textBox = (sender as TextBox)!;
        if (!StrUtility.IsNumberFormatValid(e.Text))
        {
            e.Handled = true;
        }
        
        if (_fFirstTimeImput)
        {
            textBox.Text = e.Text + ",00";
            _fFirstTimeImput = false;
            _kwota = textBox.Text;
            e.Handled = true;
            textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectionStart = _kwota.Length - 1 - StrUtility.NumberOfDigitsAfterComa(_kwota)));
        }
        else
        {
            _kwota = textBox.Text;
        }

        if (e.Text.ToCharArray()[0] == '.' || e.Text.ToCharArray()[0] == ',')
        {
            if (!_kwota.EndsWith(','))
            {
                _kwota += ",";
                textBox.Text = _kwota;
            }
            textBox.SelectionStart = textBox.Text.Length - StrUtility.NumberOfDigitsAfterComa(textBox.Text);
            e.Handled = true;
        }
    }
    
    private void Box_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (sender as TextBox)!;
        _kwota = textBox.Text;
        
        if (!StrUtility.IsNumberFormatValid(textBox.Text) || _kwota == "")
        {
            textBox.Text = "0,00";
            _kwota = String.Empty;
            _fFirstTimeImput = true;
        }

        textBox.Text = textBox.Text.Trim();
        if (StrUtility.NumberOfDigitsAfterComa(textBox.Text) > 2)
        {
            var temp = textBox.SelectionStart;
            textBox.Text = StrUtility.CropString(textBox.Text, textBox.Text.Length - StrUtility.NumberOfDigitsAfterComa(textBox.Text) + 2);
            textBox.SelectionStart = temp;
        }
    }
    
    private void Box_OnLoaded(object sender, RoutedEventArgs e)
    {
        DataObject.AddPastingHandler(Box, OnPaste);
    }
    
    private void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
        try
        {
            if (!e.DataObject.GetDataPresent(DataFormats.Text)) e.CancelCommand();
            else
            {
                string clipboardText = e.DataObject.GetData(DataFormats.Text) as string ?? throw new NullReferenceException();
                if (string.IsNullOrWhiteSpace(clipboardText) || !StrUtility.IsNumberFormatValid(clipboardText)) e.CancelCommand();
                else
                {
                    _kwota = clipboardText;
                    _fFirstTimeImput = false;
                    e.CancelCommand();

                    if (sender is TextBox textBox)
                    {
                        int selectionStart = textBox.SelectionStart;
                        int selectionLength = textBox.SelectionLength;
                        textBox.Text = textBox.Text.Remove(selectionStart, selectionLength);
                        textBox.Text = textBox.Text.Insert(selectionStart, clipboardText);
                        textBox.SelectionStart = selectionStart + clipboardText.Length;
                    }
                }
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            e.CancelCommand();
        }
    }
    
    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        var currencyIndex = CurrencyPicker.SelectedIndex;
        var dataRangeIndex = DataRangePicker.SelectedIndex;
        if (_kwota == String.Empty) _kwota = "0,00";
        
        var selectedCurrency = (Currency)currencyIndex;
        //string symbol = CurrencySymbol.Currency[selectedCurrency];
        //MessageBox.Show($"Selected Currency: {selectedCurrency}\nSymbol: {symbol}");
        
        var selectedDataRange = (DataRange)dataRangeIndex;
        //MessageBox.Show($"Selected Data Range: {selectedDataRange}");
        
        SettingsUtility.SaveSettings(_kwota, selectedCurrency, Core.NumberOfRows);
        SettingsUtility.LoadSettings();
        
        Close();
    }
}