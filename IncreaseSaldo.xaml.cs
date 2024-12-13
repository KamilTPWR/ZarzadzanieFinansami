using System.Runtime.Intrinsics.Arm;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Sqlite;

namespace ZarzadzanieFinansami;

public partial class IncreaseSaldo
{
    protected string Pkwota = String.Empty;
    
    private bool f_firstTimeImput = true;
    public IncreaseSaldo()
    {
        InitializeComponent();
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
        DbUtility.SaveTransaction(nazwa, kwotaText, uwagi, data);
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
        
        if (f_firstTimeImput)
        {
            textBox.Text = e.Text + ",00";
            f_firstTimeImput = false;
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
        
        if (!StrUtillity.IsNumberFormatValid(textBox.Text) || Pkwota == "" || Pkwota == null)
        {
            textBox.Text = "0,00";
            Pkwota = String.Empty;
            f_firstTimeImput = true;
        }

        textBox.Text = textBox.Text.Trim();
        if (StrUtillity.NumberOfDigitsAfterComa(textBox.Text) > 2)
        {
            var temp = textBox.SelectionStart;
            textBox.Text = StrUtillity.CropString(textBox.Text, textBox.Text.Length - StrUtillity.NumberOfDigitsAfterComa(textBox.Text) + 2);
            textBox.SelectionStart = temp;
        }
    }

    private void KwotaTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        
    }


}