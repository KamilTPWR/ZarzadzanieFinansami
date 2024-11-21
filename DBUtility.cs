using System.Windows;
using System.Windows.Threading;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;

namespace ZarządzanieFinansami;

public abstract class DbUtility
{
    public static List<Transaction> GetFromDatabase(string command = "SELECT * FROM ListaTranzakcji", string dataBaseName = $"FinanseDataBase.db")
    {
        List<Transaction> transactions = new List<Transaction>();
        SQLitePCL.Batteries.Init();
        
        string columnSection = command.Substring(7, command.IndexOf("FROM") - 8).Trim();

        List<string> columns;
        if (columnSection.Trim() == "*")
        {
            columns = new List<string> { "Nazwa", "Kwota", "Data", "Uwagi" };
        }
        else
        {
            columns = columnSection.Split(',').Select(c => c.Trim()).ToList();
        }

        string fullTableName = "";
        foreach (var column in columns)
        { 
           fullTableName += column + ",";
        }
        MessageBox.Show(fullTableName);
        
        using (var connection = new SqliteConnection($"Data Source={dataBaseName}"))
        {
            try
            {
                connection.Open();
                var _command = connection.CreateCommand();
                _command.CommandText = command;
                MessageBox.Show(command);

                using (var reader = _command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = (columns.Contains("Nazwa") && !reader.IsDBNull(columns.IndexOf("Nazwa")) ? 
                            reader.GetString(columns.IndexOf("Nazwa")) 
                            : null) ?? string.Empty;
                        
                        double amount = columns.Contains("Kwota") && !reader.IsDBNull(columns.IndexOf("Kwota"))? 
                            reader.GetDouble(columns.IndexOf("Kwota")) 
                            : 0;
                        
                        string date = (columns.Contains("Data") && !reader.IsDBNull(columns.IndexOf("Data"))? 
                            reader.GetString(columns.IndexOf("Data")) 
                            : null) ?? string.Empty;
                        
                        string remarks = (columns.Contains("Uwagi") && !reader.IsDBNull(columns.IndexOf("Uwagi"))?
                            reader.GetString(columns.IndexOf("Uwagi"))
                            : null) ?? string.Empty;

                        transactions.Add(new Transaction(name, amount, date, remarks));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                connection.Close();
            }
        }
        return transactions;
    }
}