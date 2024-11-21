using System.Data;
using Microsoft.Data.Sqlite;
using System.Media;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ZarządzanieFinansami;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>

// ReSharper disable once RedundantExtendsListEntry
public partial class MainWindow : Window 
{
    Core _core = new Core();
    public List<Transaction> Transactions = new List<Transaction>();
    public MainWindow()
    {   
        InitializeComponent();
        StartClock();
        UpdateDataGrid();
        ChangeSaldoEvent(GetSaldoFromDatabase());
        
        ResultTextDisplay.Text = $"Suma: {_core.Saldo} $";
        
        
        //Napisy domyśne
        SystemClock.Text = "00/00/0000 00:00:00";
        
        //Zarządzanie zegarem w prawym górnym rogu
        void StartClock()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TickEvent;
            timer.Start();
        }
        void TickEvent(object? sender, EventArgs e) //Nie mam pojęcia dlaczego tam ma być znak zapytania : P
        {
            SystemClock.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
    private void UpdateWindow()
    {
        ChangeSaldoEvent(GetSaldoFromDatabase());
        UpdateDataGrid();
        UpdateDataGridView();
    }
    private void UpdateDataGridView()
    {
        var scaleRation = 0.20;
        var gridView = MyDataGridView;

        double totalWidth = MyDataGridView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
        
        gridView.Columns[0].Width = totalWidth * scaleRation;  // "Saldo"
        gridView.Columns[1].Width = totalWidth * scaleRation;  // "Zmiana"
        gridView.Columns[2].Width = totalWidth * scaleRation;  // "Data"
        gridView.Columns[3].Width = totalWidth * 2*(scaleRation + 0.01); // "Uwagi"
    }
    private void MyDataGridView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var scaleRation = 0.20;
        var gridView = MyDataGridView;
        double totalWidth = MyDataGridView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
        if (4 == gridView.Columns.Count)
        {
            gridView.Columns[0].Width = totalWidth * scaleRation;  // "Saldo"
            gridView.Columns[1].Width = totalWidth * scaleRation;  // "Zmiana"
            gridView.Columns[2].Width = totalWidth * scaleRation;  // "Data"
            gridView.Columns[3].Width = totalWidth * 2*(scaleRation + 0.01); // "Uwagi"
        }
    }
    private void DodajRekord_OnClick(object sender, RoutedEventArgs e)
    {
        IncreaseSaldo increaseSaldo = new IncreaseSaldo();
        increaseSaldo.ShowDialog();
        UpdateWindow();
    }
    private void ChangeSaldoEvent(double newSaldo)
    {
        _core.ChangeSaldo(newSaldo);
        ResultTextDisplay.Text = $"Saldo: {_core.Saldo} $";
    }
    private void UpdateDataGrid() {
        Transactions.Clear();
        DataContext = null;
        /*SQLitePCL.Batteries.Init();
        using (var connection = new SqliteConnection("Data Source=FinanseDataBase.db"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT Nazwa, Kwota, Data, Uwagi FROM ListaTranzakcji";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var nazwa = reader.GetString(0);
                    var kwota = reader.GetDouble(1);
                    var data = reader.GetString(2);
                    var uwagi = reader.GetString(3);
                    Transactions.Add(new Transaction(nazwa, kwota, data, uwagi));
                }
            }
        }*/
        Transactions = DbUtility.GetFromDatabase(@"SELECT Nazwa, Kwota, Data, Uwagi FROM ListaTranzakcji");
        DataContext = Transactions;
    }
    private void MyDataGridView_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateDataGrid();
        var scaleRation = 0.20;
        var gridView = MyDataGridView;

        double totalWidth = MyDataGridView.ActualWidth - SystemParameters.VerticalScrollBarWidth;

        UpdateDataGrid();
        gridView.Columns[0].Width = totalWidth * scaleRation;  // "Saldo"
        gridView.Columns[1].Width = totalWidth * scaleRation;  // "Zmiana"
        gridView.Columns[2].Width = totalWidth * scaleRation;  // "Data"
        gridView.Columns[3].Width = totalWidth * 2*(scaleRation + 0.01); // "Uwagi"
    }
    private double GetSaldoFromDatabase()
    {
        double returnValue = 0.0;
        Transactions.Clear();
        DataContext = null;
        SQLitePCL.Batteries.Init();
        using (var connection = new SqliteConnection("Data Source=FinanseDataBase.db"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT Kwota FROM ListaTranzakcji";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var kwota = reader.GetDouble(0);
                    returnValue += kwota;
                }
            }
        }
        DataContext = Transactions;
        return returnValue;
    }


    private void UsunRekord_OnClick(object sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }
    
    }