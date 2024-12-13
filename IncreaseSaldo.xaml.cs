using System.Runtime.Intrinsics.Arm;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Data.Sqlite;

namespace ZarzadzanieFinansami;

public partial class IncreaseSaldo
{
    protected string Pkwota = String.Empty;
    private bool _fFirstTimeImput = true;
    private bool _fErrorInTextImput0 = false;
    private bool _fErrorInTextImput1 = false;
    
    public IncreaseSaldo()
    {
        InitializeComponent();
        Dodaj_Button.IsEnabled = false;
    }
    private void DodajButton_Click(object sender, RoutedEventArgs e)
    {
        // Get the values from TextBoxes
        var nazwa = NazwaTextBox.Text;
        var kwotaText = KwotaTextBox.Text;
        var uwagi = UwagiTextBox.Text;
        var data = DateTime.Now.ToString("dd/MM/yyyy");
        DateTime? selectedDate = Datepicker.SelectedDate;
        if (selectedDate.HasValue)
        {
            data = selectedDate.Value.ToString("dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }
        DbUtility.SaveTransaction(nazwa, kwotaText, data, uwagi);
        Close();
    }
    private void AnulujButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    private void KwotaTextBox_OnGotFocus(object sender, RoutedEventArgs e)
    {
        TextBox textBox = (sender as TextBox)!;
        textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
    }
    private void KwotaTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (!StrUtillity.IsNumberFormatValid(e.Text))
        {
            e.Handled = true;
        }
        
        TextBox textBox = (sender as TextBox)!;
        
        if (_fFirstTimeImput)
        {
            textBox.Text = e.Text + ",00";
            _fFirstTimeImput = false;
            Pkwota = textBox.Text;
            e.Handled = true;
            textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectionStart = Pkwota.Length - 1 - StrUtillity.NumberOfDigitsAfterComa(Pkwota)));
        }
        else
        {
            Pkwota = textBox.Text;
        }

        if (e.Text.ToCharArray()[0] == '.' || e.Text.ToCharArray()[0] == ',')
        {
            if (!Pkwota.EndsWith(','))
            {
                Pkwota += ",";
                textBox.Text = Pkwota;
            }
            textBox.SelectionStart = textBox.Text.Length - StrUtillity.NumberOfDigitsAfterComa(textBox.Text);
            e.Handled = true;
        }
    }
    private void KwotaTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (sender as TextBox)!;
        Pkwota = textBox.Text;
        
        if (!StrUtillity.IsNumberFormatValid(textBox.Text) || Pkwota == "")
        {
            textBox.Text = "0,00";
            Pkwota = String.Empty;
            _fFirstTimeImput = true;
        }

        textBox.Text = textBox.Text.Trim();
        if (StrUtillity.NumberOfDigitsAfterComa(textBox.Text) > 2)
        {
            var temp = textBox.SelectionStart;
            textBox.Text = StrUtillity.CropString(textBox.Text, textBox.Text.Length - StrUtillity.NumberOfDigitsAfterComa(textBox.Text) + 2);
            textBox.SelectionStart = temp;
        }
    }
    private void NazwaTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (sender as TextBox)!;
        if (Regex.IsMatch(textBox.Text, @"^[A-Za-z0-9ąĄęĘóÓśŚłŁżŻźŹćĆńŃ ]{1,25}$") && textBox.Text != "")
        {
            ToolTipService.SetToolTip(textBox, null);
            textBox.Foreground = Brushes.Black;
            _fErrorInTextImput1 = false;
            ButtonToggle(_fErrorInTextImput0,_fErrorInTextImput1);
        }
        else
        {
            ToolTip tooltip = new ToolTip();
            tooltip.Content = "Proszę wprowadzić tekst, który zawiera tylko litery lub cyfry. Długość tekstu nie może przekraczać 25 znaków.";
            ToolTipService.SetToolTip(textBox, tooltip);
            textBox.Foreground = Brushes.Red;
            _fErrorInTextImput1 = true;
            ButtonToggle(_fErrorInTextImput0,_fErrorInTextImput1);
        }
    }
    private void UwagiTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (sender as TextBox)!;
        if (Regex.IsMatch(textBox.Text, @"^[A-Za-z0-9ąĄęĘóÓśŚłŁżŻźŹćĆńŃ ]{1,200}$") && textBox.Text != "")
        {
            ToolTipService.SetToolTip(textBox, null);
            textBox.Foreground = Brushes.Black;
            _fErrorInTextImput0 = false;
            ButtonToggle(_fErrorInTextImput0,_fErrorInTextImput1);
        }
        else
        {
            ToolTip tooltip = new ToolTip();
            tooltip.Content = "Proszę wprowadzić tekst, który zawiera tylko litery lub cyfry. Długość tekstu nie może przekraczać 200 znaków.";
            ToolTipService.SetToolTip(textBox, tooltip);
            textBox.Foreground = Brushes.Red;
            _fErrorInTextImput0 = true;
            ButtonToggle(_fErrorInTextImput0,_fErrorInTextImput1);
        }
    }
    private void ButtonToggle(bool a, bool b)
    {
        if (a == false && b == false && NazwaTextBox.Text != "" && UwagiTextBox.Text != "" && KwotaTextBox.Text != "0,00")
        {
            Dodaj_Button.IsEnabled = true;
        }
        else
        {
            Dodaj_Button.IsEnabled = false;
        }
    }
}
