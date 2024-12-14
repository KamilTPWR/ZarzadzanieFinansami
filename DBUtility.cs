using Microsoft.Data.Sqlite;
using System.IO;
using System.Windows;

namespace ZarzadzanieFinansami;

public abstract class DbUtility
{
    public static string DataBasePath = "";
    public static List<Transaction> GetFromDatabase()
    {
        try
        {
            string dataBaseName = DbUtility.ReturnDataBasePath();
            if (dataBaseName == "")
            {
                throw new Exception("Nie zostala wybrana baza danych");
            }
            string command = "SELECT * FROM ListaTranzakcji";
            //ReSharper disable once UseCollectionExpression, ponieważ po co komplikować proste rzeczy
            List<string> columns = new List<string> { "ID", "Nazwa", "Kwota", "Data", "Uwagi" };

            List<Transaction> transactions = new();
            SQLitePCL.Batteries.Init();

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
                            int id = IfNotNull<int>("ID", columns, reader);
                            string name = IfNotNull<string>("Nazwa", columns, reader);
                            double amount = IfNotNull<double>("Kwota", columns, reader);
                            string date = IfNotNull<string>("Data", columns, reader);
                            string remarks = IfNotNull<string>("Uwagi", columns, reader);
                            transactions.Add(new Transaction(id, name, amount, date, remarks));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Nie spodziewany bład GetFromDatabase", MessageBoxButton.OK, MessageBoxImage.Error);
                    connection.Close();
                }
            }

            return transactions;
        }
        catch (Exception ex)
        {
            return new List<Transaction>();
        }
    }

    public static void SaveTransaction(string nazwa, string kwotaText, string data, string uwagi)
    {
        try
        {
            string dataBaseName = DbUtility.ReturnDataBasePath();
            if (dataBaseName == "")
            {
                throw new Exception("Nie zostala wybrana baza danych");
            }
            if (double.TryParse(kwotaText, out var kwota))
            {
                SQLitePCL.Batteries.Init();

                try
                {

                    using (var connection = new SqliteConnection($"Data Source={dataBaseName}"))
                    {
                        connection.Open();
                        var command = connection.CreateCommand();
                        command.CommandText =
                            "INSERT INTO ListaTranzakcji(Nazwa, Kwota, Data, Uwagi) VALUES ($nazwa, $kwota, $data, $uwagi)";
                        command.Parameters.AddWithValue("$nazwa", nazwa);
                        command.Parameters.AddWithValue("$kwota", kwota);
                        command.Parameters.AddWithValue("$data", data);
                        command.Parameters.AddWithValue("$uwagi", uwagi);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Nie spodziewany bład SaveTransaction", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Invalid input for Kwota. Please enter a numeric value.");
            }
        }
        catch (Exception ex)
        {
        }
    }

    public static int GetNumberOfTransactions()
    {
        try
        {
            string dataBaseName = DbUtility.ReturnDataBasePath();
            if (dataBaseName == "")
            {
                throw new Exception("Nie zostala wybrana baza danych");
            }
            List<Transaction> transactions = GetFromDatabase();
            if (transactions == null)
            {
                throw new Exception("Nie zostala wybrana baza danych");
            }
            var i = transactions.Count;
            return i;
        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    public static void DeleteFromDatabase(int index, string tableName = $"ListaTranzakcji")
    {
        try
        {
            string dataBaseName = DbUtility.ReturnDataBasePath();
            if (dataBaseName == "")
            {
                throw new Exception("Nie zostala wybrana baza danych");
            }
            var command = $"DELETE FROM {tableName} WHERE ID = {index}";
            SQLitePCL.Batteries.Init();

            using (var connection = new SqliteConnection($"Data Source={dataBaseName}"))
            {
                try
                {
                    connection.Open();
                    var sqliteCommand = connection.CreateCommand();
                    sqliteCommand.CommandText = command;
                    sqliteCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Nie spodziewany bład", MessageBoxButton.OK, MessageBoxImage.Error);
                    connection.Close();
                }
            }
        }
        catch (Exception ex)
        {
        }
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

        if (typeof(T) == typeof(int))
        {
            var temp = columns.Contains(condition) && !reader.IsDBNull(columns.IndexOf(condition))
                ? reader.GetInt32(columns.IndexOf(condition))
                : 0;
            return temp;
        }

        throw new NotSupportedException($"The type {typeof(T).Name} is not supported.");
    }
    public static void CreateDatabase()
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            FileName = "database.db",
            Filter = "SQLite Database Files (*.db)|*.db",
            Title = "Save SQLite Database"
        };

        // Show save file dialog box
        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            DataBasePath = dialog.FileName;

            try
            {
                // Copy the base database file to the selected location
                File.Copy("FinanseDataBase.db", DataBasePath, overwrite: false);
            }
            catch (IOException copyError)
            {
                MessageBox.Show($"Error: {copyError.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    public static void OpenDatabase()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "SQLite Database Files (*.db)|*.db|All Files (*.*)|*.*",
            Title = "Select an SQLite Database File"
        };
        
        bool? result = dialog.ShowDialog();
        
        if (result == true)
        {
            DataBasePath = dialog.FileName;

            //Schemat wszystich tabelek
            string[] expectedSchemas = [
                "CREATE TABLE \"ListaTranzakcji\" (\r\n\t\"ID\"\tINTEGER NOT NULL,\r\n\t\"Nazwa\"\tTEXT NOT NULL,\r\n\t\"Kwota\"\tREAL NOT NULL,\r\n\t\"Data\"\tTEXT NOT NULL,\r\n\t\"Uwagi\"\tTEXT,\r\n\tPRIMARY KEY(\"ID\" AUTOINCREMENT)\r\n)",
                "CREATE TABLE \"Rachunki\" (\r\n\t\"ID\"\tINTEGER NOT NULL,\r\n\t\"Nazwa\"\tTEXT,\r\n\t\"Saldo\"\tREAL NOT NULL,\r\n\tPRIMARY KEY(\"ID\" AUTOINCREMENT)\r\n)",
                "CREATE TABLE \"KontoOszczednosciowe\" (\r\n\t\"ID\"\tINTEGER NOT NULL,\r\n\t\"IDkonta\"\tINTEGER NOT NULL,\r\n\tPRIMARY KEY(\"ID\" AUTOINCREMENT),\r\n\tCONSTRAINT \"KluczObcy\" FOREIGN KEY(\"IDkonta\") REFERENCES \"\"\r\n)",
                "CREATE TABLE main_ListaTranzakcji\r\n(\r\n    ID    INTEGER,\r\n    Nazwa TEXT,\r\n    Kwota REAL,\r\n    Data  TEXT,\r\n    Uwagi TEXT\r\n)"
            ];
            int temp = 0;
            try
            {
                using (var connection = new SqliteConnection($"Data Source={DataBasePath}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = "SELECT sql FROM sqlite_master WHERE type='table' AND name != 'sqlite_sequence'";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var actualSchema = reader.GetString(0);
                            if (actualSchema != String.Empty)
                            {
                                if (NormalizeSchema(actualSchema) == NormalizeSchema(expectedSchemas[temp]))
                                {
                                    temp++;
                                }
                                else
                                {
                                    throw new Exception("Table structures dont match");
                                }
                            }
                            else
                            {
                                throw new Exception("Table does not exist");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                DataBasePath = "";
            }
        }
    }
    public static void SaveDatabase()
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            FileName = "database.db",
            Filter = "SQLite Database Files (*.db)|*.db",
            Title = "Save SQLite Database"
        };

        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            string tempPath = dialog.FileName;
            try
            {
                File.Copy(DataBasePath, tempPath, overwrite: false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    private static string ReturnDataBasePath()
    {
        if (string.IsNullOrEmpty(DataBasePath))
        {
            return "";
        }
        if (!File.Exists(DataBasePath))
        {
            return "";
        }
        return DataBasePath;
    }
    private static string NormalizeSchema(string schema)
    {
        return schema
        .Replace("\r", "")
        .Replace("\n", "")
        .Replace("\t", "")
        .Replace("\"", "'")
        .Trim()
        .ToLowerInvariant();
    }
}