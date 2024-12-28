using Microsoft.Data.Sqlite;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace ZarzadzanieFinansami;

public abstract class DbUtility
{
    private static string _dataBasePath = String.Empty;

    public static List<Transaction> GetTransactionsFromDatabase(out bool success)
    {
        string command = 
            "SELECT ListaTranzakcji.ID, ListaTranzakcji.Nazwa, Kwota, Data, Uwagi, Kategorie.Nazwa FROM ListaTranzakcji JOIN Kategorie on ListaTranzakcji.KategoriaID = Kategorie.ID";
        List<string> columns = Constants.DEFAULTCOLUMNS;
        List<Transaction> transactions = new();
        success = false;

        try
        {
            string dataBaseName = ReturnDataBasePath();
            EnsureNotEmpty(dataBaseName);
            SQLitePCL.Batteries.Init();
            using (var connection = new SqliteConnection($"Data Source={dataBaseName}"))
            using (var reader = SqliteExecuteCommand(connection, command).ExecuteReader())
            {
                while (reader.Read())
                {
                    try
                    {
                        AddTransactionsFromColumns(columns, reader, transactions);
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Nie spodziewany bład GetFromDatabase", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }
        catch (Exception)
        {
            return new List<Transaction>();
        }
        return transactions;
    }
    
    private static void AddTransactionsFromColumns(List<string> columns, SqliteDataReader reader, List<Transaction> transactions)
    {
        int id = TryGetValue<int>("ListaTranzakcji.ID", columns, reader);
        string name = TryGetValue<string>("ListaTranzakcji.Nazwa", columns, reader);
        double amount = TryGetValue<double>("Kwota", columns, reader);
        string date = TryGetValue<string>("Data", columns, reader);
        string remarks = TryGetValue<string>("Uwagi", columns, reader);
        string category = TryGetValue<string>("Kategorie.Nazwa", columns, reader);
        transactions.Add(new Transaction(id, name, amount, date, remarks, category));
    }

    public static List<Category> GetCategoriesFromDatabase()
    {
        string command = 
            "SELECT * FROM Kategorie";
        var columns = new List<string> { "ID", "Nazwa" };
        List<Category> categories = new();
        try
        {
            string dataBaseName = ReturnDataBasePath();
            EnsureNotEmpty(dataBaseName);
            SQLitePCL.Batteries.Init();
            { 
                using (var connection = new SqliteConnection($"Data Source={dataBaseName}"))
                using (var reader = SqliteExecuteCommand(connection, command).ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {
                            AddCategorisFromColumns(columns, reader, categories);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Nie spodziewany bład GetCategoriesFromDatabase", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                            connection.Close();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            return new List<Category>();
        }
        return categories;
    }

    private static void AddCategorisFromColumns(List<string> columns, SqliteDataReader reader, List<Category> categories)
    {
        int id = TryGetValue<int>("ID", columns, reader);
        string name = TryGetValue<string>("Nazwa", columns, reader);
        categories.Add(new Category(id, name));
    }

    public static void SaveTransaction(string nazwa, string kwotaText, string data, string uwagi, int idkategorii)
    {
        string commandText = 
            "INSERT INTO ListaTranzakcji(Nazwa, Kwota, Data, Uwagi, KategoriaID) VALUES ($nazwa, $kwota, $data, $uwagi, $idkat)";
        string dataBaseName = ReturnDataBasePath();
        if (IsDataBaseNull(dataBaseName)) return;
        if (!double.TryParse(kwotaText, out var kwota))
        {
            MessageBox.Show("Błąd", "Nie udało się zapisać w bazie danych", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        else
        {
            SQLitePCL.Batteries.Init();
            using (var connection = new SqliteConnection($"Data Source={dataBaseName}"))
            {
                try
                {
                    connection.Open();
                    ExecuteSaveTransactionSql(connection, commandText, nazwa, kwota, data, uwagi, idkategorii);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Nie spodziewany bład SaveTransaction", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }
    }

    private static void ExecuteSaveTransactionSql(SqliteConnection connection, string commandText, string nazwa, double kwota, string data, string uwagi, int idkategorii)
    {
        var command = connection.CreateCommand();
        command.CommandText = commandText;
        command.Parameters.AddWithValue("$nazwa", nazwa);
        command.Parameters.AddWithValue("$kwota", kwota);
        command.Parameters.AddWithValue("$data", data);
        command.Parameters.AddWithValue("$uwagi", uwagi);
        command.Parameters.AddWithValue("$idkat", idkategorii);
        command.ExecuteNonQuery();
    }

    public static void SaveCategory(string nazwa)
    {
        string commandText =
            "INSERT INTO Kategorie(Nazwa) VALUES ($nazwa)";
        string dataBaseName = ReturnDataBasePath();
        if (IsDataBaseNull(dataBaseName)) return;
        try
        {
            SQLitePCL.Batteries.Init();
            using (var connection = new SqliteConnection($"Data Source={dataBaseName}"))
            {
                connection.Open();
                ExecuteSaveCategorySql(nazwa, connection, commandText);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Nie spodziewany bład SaveCategory", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private static void ExecuteSaveCategorySql(string nazwa, SqliteConnection connection, string commandText)
    {
        var command = connection.CreateCommand();
        command.CommandText = commandText;
        command.Parameters.AddWithValue("$nazwa", nazwa);
        command.ExecuteNonQuery();
    }

    public static int GetNumberOfTransactions()
    {
        try
        {
            string dataBaseName = ReturnDataBasePath();
            EnsureNotEmpty(dataBaseName);
            List<Transaction> transactions = GetTransactionsFromDatabase(out var success);
            return success ? transactions.Count : 0;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public static void DeleteFromDatabase(int index, string tableName = $"ListaTranzakcji")
    {
        string command = 
            $"DELETE FROM {tableName} WHERE ID = {index}";
        string dataBaseName = ReturnDataBasePath();
        if (IsDataBaseNull(dataBaseName)) return;
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

    /***********************************************************************************************************************/
    /*                                                Dialog Winodws                                                       */
    /***********************************************************************************************************************/

    public static void CreateDatabase()
    {
        var dialog = CreateSaveFileDialog();
        var result = dialog.ShowDialog();
        if (result != true) return;
        _dataBasePath = dialog.FileName;
        try
        {
            File.Copy("FinanseDataBase.db", _dataBasePath, overwrite: false);
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

    public static void OpenDatabase()
    {
        var dialog = OpenFileDialog();
        var result = dialog.ShowDialog();
        if (result != true) return;
        _dataBasePath = dialog.FileName;
        try
        {
            using (var connection = new SqliteConnection($"Data Source={_dataBasePath}"))
            {
                int temp = 0;
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT sql FROM sqlite_master WHERE type='table' AND name != 'sqlite_sequence'";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var actualSchema = reader.GetString(0);
                        if (actualSchema == String.Empty)
                        {
                            throw new Exception("Table does not exist");
                        }

                        if (NormalizeSchema(actualSchema) == NormalizeSchema(Constants.EXPECTEDSCHAMES[temp]))
                        {
                            temp++;
                        }
                        else
                        {
                            throw new Exception("Table structures dont match");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            _dataBasePath = String.Empty;
        }
    }

    public static void SaveDatabase()
    {
        var dialog = SaveFileDialog();

        bool? result = dialog.ShowDialog();

        if (result == true)
        {
            string tempPath = dialog.FileName;
            try
            {
                File.Copy(_dataBasePath, tempPath, overwrite: false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private static SaveFileDialog CreateSaveFileDialog()
    {
        var dialog = new SaveFileDialog
        {
            FileName = "database.db",
            Filter = "SQLite Database Files (*.db)|*.db",
            Title = "Zapisz Plik"
        };
        return dialog;
    }

    private static OpenFileDialog OpenFileDialog()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "SQLite Database Files (*.db)|*.db|All Files (*.*)|*.*",
            Title = "Select an SQLite Database File"
        };
        return dialog;
    }

    private static SaveFileDialog SaveFileDialog()
    {
        var dialog = new SaveFileDialog
        {
            FileName = "database.db",
            Filter = "SQLite Database Files (*.db)|*.db",
            Title = "Save SQLite Database"
        };
        return dialog;
    }

    /***********************************************************************************************************************/
    /*                                                Private Methods                                                      */
    /***********************************************************************************************************************/

    private static string ReturnDataBasePath()
    {
        if (string.IsNullOrEmpty(_dataBasePath))
        {
            return String.Empty;
        }
        if (!File.Exists(_dataBasePath))
        {
            return String.Empty;
        }
        return _dataBasePath;
    }
    
    private static void EnsureNotEmpty(string dataBaseName)
    {
        if (dataBaseName == String.Empty)
        {
            throw new Exception("Nie zostala wybrana baza danych");
        }
    }
    
    private static bool IsDataBaseNull(string dataBaseName)
    {
        try
        {
            EnsureNotEmpty(dataBaseName);
        }
        catch (Exception)
        {
            return true;
        }

        return false;
    }
    
    private static SqliteCommand SqliteExecuteCommand(SqliteConnection connection, string command)
    {
        connection.Open();
        var sqliteCommand = connection.CreateCommand();
        sqliteCommand.CommandText = command;
        return sqliteCommand;
    }

    private static dynamic TryGetValue<T>(string condition, List<string> columns, SqliteDataReader? reader)
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