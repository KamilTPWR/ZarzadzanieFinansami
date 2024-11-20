using System.Windows;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;

namespace ZarządzanieFinansami;

public class DbUtility
{
    private List<Transaction> GetFromDatabase(string dataBaseName, string tableName = "ListaTranzakcji", string command = "SELECT * FROM ")
    {
        List<Transaction> transactions = new List<Transaction>();
        SQLitePCL.Batteries.Init();
        
        string columnSection = command.Substring(7, command.IndexOf("FROM") - 5).Trim();
        string fullTableName = "";

        List<string> columns;
        if (columnSection.Trim() == "*")
        {
            columns = new List<string> { "Name", "Amount", "Date", "Remarks" };
        }
        else
        {
            columns = columnSection.Split(',').Select(c => c.Trim()).ToList();
        }

       
        foreach (var column in columns)
        { 
           fullTableName += column + ",";
        }
        MessageBox.Show(fullTableName);
        
        using (var connection = new SqliteConnection($"Data Source={dataBaseName}"))
        {
            connection.Open();
            var _command = connection.CreateCommand();
            _command.CommandText = command;

            using (var reader = _command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string name = columns.Contains("Name") ? reader.GetString(columns.IndexOf("Name")) : null;
                    double amount = columns.Contains("Amount") ? reader.GetDouble(columns.IndexOf("Amount")) : 0;
                    string date = columns.Contains("Date") ? reader.GetString(columns.IndexOf("Date")) : null;
                    string remarks = columns.Contains("Remarks") ? reader.GetString(columns.IndexOf("Remarks")) : null;

                    transactions.Add(new Transaction(name, amount, date, remarks));
                }
            }
        }
        return transactions;
    }
}