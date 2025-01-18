using System.Runtime.Intrinsics.Arm;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Data.Sqlite;
using ZarzadzanieFinansami;

namespace ZarzadzanieFinansami;

public partial class IncreaseSaldo
{
    private string _kwota = String.Empty;
    
    private bool _fFirstTimeImput = true;
    private bool _fErrorInTextImput0 = false;
    private bool _fErrorInTextImput1 = false;
    
    private List<string> _categories = new();

    public IncreaseSaldo()
    {
        InitializeComponent();
        AddButton.IsEnabled = false;
        UpdateCategories();
    }
    public void UpdateCategories()
    {
        List<Category> temp = DbUtility.GetCategoriesFromDatabase();
        _categories.Clear();
        Cats.ItemsSource = _categories;
        foreach (Category category in temp)
        {
            _categories.Add(category.ID + ". " + category.Name);
        }
        Cats.ItemsSource = _categories;
    }
    
/***********************************************************************************************************************/
/*                                                Buttons Event Handlers                                               */
/***********************************************************************************************************************/    
    private void DodajButton_Click(object sender, RoutedEventArgs e)
    {
        var nazwa = NazwaTextBox.Text;
        var kwotaText = KwotaTextBox.Text;
        var uwagi = UwagiTextBox.Text;
        var data = DateTime.Now.ToString(Constants.DATEFORMAT);
        var temp = Cats.SelectedValue;
        
        if (temp != null)
        {
            Int32 kategoria = Convert.ToInt32(temp.ToString()!.Split(".")[0]);
            DateTime? selectedDate = Datepicker.SelectedDate;
            
            if (selectedDate.HasValue)
            {
                data = selectedDate.Value.ToString(Constants.DATEFORMAT, System.Globalization.CultureInfo.InvariantCulture);
            }

            DbUtility.SaveTransaction(nazwa, kwotaText, data, uwagi, kategoria);
            Close();
        }
        else
        {
            MessageBox.Show("Nie wybrano Kategorii.", "Nie wybrano kategorii.", MessageBoxButton.OK,
                MessageBoxImage.Error);
            e.Handled = true;
        }
    }
    private void AnulujButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
    private void Dodaj_Kategorie_Button_Click(object sender, RoutedEventArgs e)
    {
        CategoryAdd window = new CategoryAdd();
        window.ShowDialog();
        UpdateCategories();
        Cats.Dispatcher.BeginInvoke(new Action(() => Cats.ItemsSource = _categories));
        Cats.Dispatcher.Invoke(delegate { Cats.Items.Refresh(); });
    }
    
/***********************************************************************************************************************/
/*                                                   TextBox Logic                                                     */
/***********************************************************************************************************************/        
  
    //Title
    private void TitleTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (sender as TextBox)!;

        if (IsMatchTitleRegex(textBox))
        {
            HandleValidInput(textBox);
        }
        else
        {
            HandleInvalidInput(textBox);
        }

        ButtonToggle(_fErrorInTextImput0 && !_fErrorInTextImput1);;
    }
    private void TitleTextBox_OnGotFocus(object sender, RoutedEventArgs e)
    {
        TextBox textBox = (sender as TextBox)!;
        textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()));
    }
    
    //Price
    private void KwotaTextBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        DataObject.AddPastingHandler(KwotaTextBox, OnPaste);
    }
    private void KwotaTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        TextBox textBox = (sender as TextBox)!;
        
        if (!StrUtility.IsNumberFormatValid(e.Text)) e.Handled = true;
        
        ButtonToggle(_fErrorInTextImput0 && _fErrorInTextImput1);
        
        //A bit of Magic
        //Made by trial and error
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
    private void KwotaTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (sender as TextBox)!;
        _kwota = textBox.Text;
        
        HandleInvalidFormat(textBox);
        
        textBox.Text = textBox.Text.Trim();
        
        HandleToManySpacesAfterComa(textBox);
    }
    
    //Note
    private void NoteTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        TextBox textBox = (sender as TextBox)!;
        if (IsMatchNoteRegex(textBox))
        {
            ToolTipService.SetToolTip(textBox, null);
            textBox.Foreground = Brushes.Black;
            
            _fErrorInTextImput0 = false;
            
            ButtonToggle(_fErrorInTextImput0 && _fErrorInTextImput1);
        }
        else
        {
            var tooltip = ShowNoteTooltip();
            ToolTipService.SetToolTip(textBox, tooltip);
            textBox.Foreground = Brushes.Red;
            
            _fErrorInTextImput0 = true;
            
            ButtonToggle(_fErrorInTextImput0 && _fErrorInTextImput1);
        }
    }
    private void OnPaste(object sender, DataObjectPastingEventArgs eventArgs)
    {
        try
        {
            if (!IsClipboardText(eventArgs))
            {
                eventArgs.CancelCommand();
                return;
            }

            string clipboardText = GetClipboardText(eventArgs);
            
            if (IsClipboardTextValid(clipboardText))
            {
                eventArgs.CancelCommand();
                return;
            }

            ProcessClipboardText(sender, clipboardText);
            eventArgs.CancelCommand();
        }
        catch (Exception exception)
        {
            HandlePasteException(exception, eventArgs);
        }
    }
    
    private bool IsClipboardText(DataObjectPastingEventArgs e)
    {
        return e.DataObject.GetDataPresent(DataFormats.Text);
    }

    private string GetClipboardText(DataObjectPastingEventArgs e)
    {
        return e.DataObject.GetData(DataFormats.Text) as string ?? throw new NullReferenceException();
    }

    private static bool IsClipboardTextValid(string text)
    {
        var isNullOrWhiteSpace = string.IsNullOrWhiteSpace(text) && StrUtility.IsNumberFormatValid(text);
        return !isNullOrWhiteSpace;
    }

    private void ProcessClipboardText(object sender, string clipboardText)
    {
        _kwota = clipboardText;
        _fFirstTimeImput = false;

        if (sender is TextBox textBox)
        {
            UpdateTextBoxWithClipboardText(textBox, clipboardText);
        }
    }

    private void UpdateTextBoxWithClipboardText(TextBox textBox, string clipboardText)
    {
        int selectionStart = textBox.SelectionStart;
        int selectionLength = textBox.SelectionLength;

        textBox.Text = textBox.Text.Remove(selectionStart, selectionLength);
        textBox.Text = textBox.Text.Insert(selectionStart, clipboardText);
        textBox.SelectionStart = selectionStart + clipboardText.Length;
    }

    private void HandlePasteException(Exception exception, DataObjectPastingEventArgs e)
    {
        Console.WriteLine(exception);
        e.CancelCommand();
    }
    
    
    //ex
    private void HandleValidInput(TextBox textBox)
    {
        ToolTipService.SetToolTip(textBox, null);
        textBox.Foreground = Brushes.Black;
        _fErrorInTextImput1 = false;
    }
    private void HandleInvalidInput(TextBox textBox)
    {
        var tooltip = ShowTitleTooltip();
        ToolTipService.SetToolTip(textBox, tooltip);
        textBox.Foreground = Brushes.Red;
        _fErrorInTextImput1 = true;
    }
    private static ToolTip ShowTitleTooltip()
    {
        ToolTip tooltip = new ToolTip();
        tooltip.Content = "Proszę wprowadzić tekst, który zawiera tylko litery lub cyfry. Długość tekstu nie może przekraczać 30 znaków.";
        return tooltip;
    }
    private static bool IsMatchTitleRegex(TextBox textBox)
    {
        return Regex.IsMatch(textBox.Text, @"^[A-Za-z0-9ąĄęĘóÓśŚłŁżŻźŹćĆńŃ ]{1,30}$") && textBox.Text != "";
    }
    private static void HandleToManySpacesAfterComa(TextBox textBox)
    {
        if (StrUtility.NumberOfDigitsAfterComa(textBox.Text) > 2)
        {
            var temp = textBox.SelectionStart;
            textBox.Text = StrUtility.CropString(textBox.Text, textBox.Text.Length - StrUtility.NumberOfDigitsAfterComa(textBox.Text) + 2);
            textBox.SelectionStart = temp;
        }
    }
    private void HandleInvalidFormat(TextBox textBox)
    {
        if (!StrUtility.IsNumberFormatValid(textBox.Text) || _kwota == "")
        {
            textBox.Text = "0,00";
            _kwota = String.Empty;
            _fFirstTimeImput = true;
        }
    } 
    private static ToolTip ShowNoteTooltip()
    {
        ToolTip tooltip = new ToolTip();
        tooltip.Content = "Proszę wprowadzić tekst, który zawiera tylko litery lub cyfry. Długość tekstu nie może przekraczać 220 znaków.";
        return tooltip;
    }
    private static bool IsMatchNoteRegex(TextBox textBox)
    {
        return Regex.IsMatch(textBox.Text, @"^[A-Za-z0-9ąĄęĘóÓśŚłŁżŻźŹćĆńŃ ]{1,220}$") && textBox.Text != "";
    }
    private void ButtonToggle(bool a)
    {
        if (a == false && NazwaTextBox.Text != "" && KwotaTextBox.Text != "0,00")
        {
            AddButton.IsEnabled = true;
        }
        else
        {
            AddButton.IsEnabled = false;
        }
    }
}
