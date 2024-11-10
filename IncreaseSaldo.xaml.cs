using System.Windows;

namespace ZarządzanieFinansami;

public partial class IncreaseSaldo : Window
{
    public IncreaseSaldo()
    {
        InitializeComponent();
    }
    private void DodajButton_Click(object sender, RoutedEventArgs e)
    {
        // Get the values from TextBoxes
        string nazwa = NazwaTextBox.Text;
        string kwotaText = KwotaTextBox.Text;
            
        // Try to parse the kwota input as a float
        if (double.TryParse(kwotaText, out double kwota))
        {
            Close();
            //MessageBox.Show($"Nazwa: {nazwa}\nKwota: {kwota}\nUwagi: {UwagiTextBox.Text}");
        }
        else
        {
            MessageBox.Show("Invalid input for Kwota. Please enter a numeric value.");
        }
    }
    private void AnulujButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
