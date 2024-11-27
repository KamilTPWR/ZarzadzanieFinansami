using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Data.Sqlite;
using Microsoft.VisualBasic;

namespace ZarzadzanieFinansami;

public abstract class DbUtility
{
    public static List<Transaction> GetFromDatabase(string command = "SELECT * FROM ListaTranzakcji",
        string dataBaseName = $"FinanseDataBase.db")
    {
        List<Transaction> transactions = new();
        SQLitePCL.Batteries.Init();

        var columnSection = command.Substring(7, command.IndexOf("FROM") - 8).Trim();

        List<string> columns;
        if (columnSection.Trim() == "*")
            columns = new List<string> { "Nazwa", "Kwota", "Data", "Uwagi" };
        else
            columns = columnSection.Split(',').Select(c => c.Trim()).ToList();

        using (var connection = new SqliteConnection($"Data Source={dataBaseName}"))
        {
            try
            {
                connection.Open();
                var sqliteCommand = connection.CreateCommand();
                sqliteCommand.CommandText = command;

                using (var reader = sqliteCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = IfNotNull<string>("Nazwa", columns, reader);
                        double amount = IfNotNull<double>("Kwota", columns, reader);
                        string date = IfNotNull<string>("Data", columns, reader);
                        string remarks = IfNotNull<string>("Uwagi", columns, reader);
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

    public static int GetNumberOfTransactions(string command = "SELECT * FROM ListaTranzakcji",
        string dataBaseName = $"FinanseDataBase.db")
    {
        List<Transaction> transactions = GetFromDatabase(command, dataBaseName);
        var i = transactions.Count;
        return i;
    }

    private static dynamic IfNotNull<T>(string condition, List<string> columns, SqliteDataReader? reader)
    {
        if (reader == null) throw new NullReferenceException();
        if (typeof(T) == typeof(double))
        {
            var temp = columns.Contains(condition) && !reader.IsDBNull(columns.IndexOf(condition))
                ? reader.GetDouble(columns.IndexOf(condition))
                : 0;
            return temp;
        }

        if (typeof(T) == typeof(string))
        {
            var temp = (columns.Contains(condition) && !reader.IsDBNull(columns.IndexOf(condition))
                ? reader.GetString(columns.IndexOf(condition))
                : null) ?? string.Empty;
            return temp;
        }

        throw new NotSupportedException($"The type {typeof(T).Name} is not supported.");
    }
}