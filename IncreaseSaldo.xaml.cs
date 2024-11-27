using System.Windows;
using Microsoft.Data.Sqlite;

namespace ZarzadzanieFinansami;

public partial class IncreaseSaldo
{
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

        // Try to parse the kwota input as a float
        if (double.TryParse(kwotaText, out var kwota))
        {
            Close();
            MessageBox.Show($"Nazwa: {nazwa}\nKwota: {kwota}\nUwagi: {uwagi}");
            SQLitePCL.Batteries.Init();

            var data = DateTime.Now.ToString("dd/MM/yyyy");

            using (var connection = new SqliteConnection("Data Source=FinanseDataBase.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText =
                    @"INSERT INTO ListaTranzakcji(Nazwa,Kwota,Data,Uwagi) VALUES ($nazwa,$kwota,$data,$uwagi)";
                command.Parameters.AddWithValue("$nazwa", nazwa);
                command.Parameters.AddWithValue("$kwota", kwota);
                command.Parameters.AddWithValue("$data", data);
                command.Parameters.AddWithValue("$uwagi", uwagi);
                command.ExecuteNonQuery();
            }
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