using Microsoft.Data.Sqlite;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace ZarzadzanieFinansami;

public abstract class DbUtility
{
    private static string _dataBasePath = String.Empty;

    public static List<Transaction> GetFromDatabase()
    {
        string dataBaseName = ReturnDataBasePath();
        try
        {
            EnsureNotEmpty(dataBaseName);
            string command = 
                "SELECT ListaTranzakcji.ID, ListaTranzakcji.Nazwa, Kwota, Data, Uwagi, Kategorie.Nazwa FROM ListaTranzakcji JOIN Kategorie on ListaTranzakcji.KategoriaID = Kategorie.ID";
            List<string> columns = Constants.DEFAULTCOLUMNS;

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
                            int id = TryGetValue<int>("ListaTranzakcji.ID", columns, reader);
                            string name = TryGetValue<string>("ListaTranzakcji.Nazwa", columns, reader);
                            double amount = TryGetValue<double>("Kwota", columns, reader);
                            string date = TryGetValue<string>("Data", columns, reader);
                            string remarks = TryGetValue<string>("Uwagi", columns, reader);
                            string category = TryGetValue<string>("Kategorie.Nazwa", columns, reader);
                            transactions.Add(new Transaction(id, name, amount, date, remarks, category));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Nie spodziewany bład GetFromDatabase", MessageBoxButton.OK,
                        MessageBoxImage.Error);
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
    public static List<Category> GetCategoriesFromDatabase()
    {
        try
        {
            string dataBaseName = DbUtility.ReturnDataBasePath();
            if (dataBaseName == "")
            {
                throw new Exception("Nie zostala wybrana baza danych");
            }
            string command = "SELECT * FROM Kategorie";
            //ReSharper disable once UseCollectionExpression, ponieważ po co komplikować proste rzeczy
            List<string> columns = new List<string> { "ID", "Nazwa" };

            List<Category> categories = new();
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
                            int id = TryGetValue<int>("ID", columns, reader);
                            string name = TryGetValue<string>("Nazwa", columns, reader);
                            categories.Add(new Category(id, name));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Nie spodziewany bład GetFromDatabase", MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    connection.Close();
                }
            }
            return categories;
        }
        catch (Exception ex)
        {
            return new List<Category>();
        }
    }

    public static void SaveTransaction(string nazwa, string kwotaText, string data, string uwagi, int idkategorii)
    {
        string dataBaseName = ReturnDataBasePath();
        EnsureNotEmpty(dataBaseName);

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
                        command.CommandText =
                            "INSERT INTO ListaTranzakcji(Nazwa, Kwota, Data, Uwagi, KategoriaID) VALUES ($nazwa, $kwota, $data, $uwagi, $idkat)";
                    command.Parameters.AddWithValue("$nazwa", nazwa);
                    command.Parameters.AddWithValue("$kwota", kwota);
                    command.Parameters.AddWithValue("$data", data);
                    command.Parameters.AddWithValue("$uwagi", uwagi);
                    command.Parameters.AddWithValue("$idkat", idkategorii);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Nie spodziewany bład SaveTransaction", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        else
        {
            MessageBox.Show("Błąd", "Nie udało się zapisać w bazie danych", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
    public static void SaveCategory(string nazwa)
    {
        try
        {
            string dataBaseName = DbUtility.ReturnDataBasePath();
            if (dataBaseName == "")
            {
                throw new Exception("Nie zostala wybrana baza danych");
            }
            SQLitePCL.Batteries.Init();
            try
            {

                using (var connection = new SqliteConnection($"Data Source={dataBaseName}"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText =
                        "INSERT INTO Kategorie(Nazwa) VALUES ($nazwa)";
                    command.Parameters.AddWithValue("$nazwa", nazwa);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Nie spodziewany bład SaveTransaction", MessageBoxButton.OK, MessageBoxImage.Error);
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
            string dataBaseName = ReturnDataBasePath();
            EnsureNotEmpty(dataBaseName);
            List<Transaction> transactions = GetFromDatabase();
            if (transactions == null)
            {
                throw new Exception("Nie zostala wybrana baza danych");
            }

            var i = transactions.Count;
            return i;
        }
        catch (Exception)
        {
            return 0;
        }
    }

    public static void DeleteFromDatabase(int index, string tableName = $"ListaTranzakcji")
    {
        try
        {
            string dataBaseName = ReturnDataBasePath();
            EnsureNotEmpty(dataBaseName);
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
            MessageBox.Show(ex.Message);
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

    private static void EnsureNotEmpty(string dataBaseName)
    {
        if (dataBaseName == String.Empty)
        {
            throw new Exception("Nie zostala wybrana baza danych");
        }
    }
}