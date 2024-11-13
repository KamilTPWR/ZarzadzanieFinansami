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
public partial class MainWindow : Window
{
    Core core = new Core();
    public List<Transaction> transactions = new List<Transaction>();
    public MainWindow()
    {   
        InitializeComponent();
        StartClock();
        UpdateDataGrid();
        InitializeDataGridView();
        ChangeSaldoEvent(0);
        
        ResultTextDisplay.Text = $"Saldo: {core.saldo} $";
        
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
        void TickEvent(object sender, EventArgs e)
        {
            SystemClock.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }

    private void InitializeDataGridView()
    {
        UpdateDataGrid();
        var scaleRation = 0.20;
        var gridView = MyListView as DataGrid;
        double totalWidth = MyListView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
        if (4 == gridView.Columns.Count)
        {
            UpdateDataGrid();
            gridView.Columns[0].Width = totalWidth * scaleRation;  // "Saldo"
            gridView.Columns[1].Width = totalWidth * scaleRation;  // "Zmiana"
            gridView.Columns[2].Width = totalWidth * scaleRation;  // "Data"
            gridView.Columns[3].Width = totalWidth * 2*(scaleRation + 0.01); // "Uwagi"
        }
        else
        {
            UpdateDataGrid();
        }
    }

    private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateDataGrid();
        var scaleRation = 0.20;
        var gridView = MyListView as DataGrid;

        double totalWidth = MyListView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
        if (4 == gridView.Columns.Count)
        {
            UpdateDataGrid();
            gridView.Columns[0].Width = totalWidth * scaleRation;  // "Saldo"
            gridView.Columns[1].Width = totalWidth * scaleRation;  // "Zmiana"
            gridView.Columns[2].Width = totalWidth * scaleRation;  // "Data"
            gridView.Columns[3].Width = totalWidth * 2*(scaleRation + 0.01); // "Uwagi"
        }
        
    }
    private void Button_OnClick(object sender, RoutedEventArgs e)
    {
        // ChangeSaldoEvent(core.saldo+1);
        
        IncreaseSaldo increaseSaldo = new IncreaseSaldo();
        increaseSaldo.ShowDialog();
        UpdateDataGrid();
        
    }
    private void ChangeSaldoEvent(double newSaldo)
    {
        core.ChangeSaldo(newSaldo);
        ResultTextDisplay.Text = $"Saldo: {core.saldo} $";
    }

    private void UpdateDataGrid() {
        transactions.Clear();
        this.DataContext = null;
        SQLitePCL.Batteries.Init();
        using (var connection = new SqliteConnection("Data Source=FinanseDataBase.db"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = @"SELECT Nazwa, Kwota, Data, Uwagi FROM ListaTranzakcji";
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var Nazwa = reader.GetString(0);
                    var Kwota = reader.GetDouble(1);
                    var Data = reader.GetString(2);
                    var Uwagi = reader.GetString(3);
                    transactions.Add(new Transaction(Nazwa, Kwota, Data, Uwagi));
                }
            }
        }
        this.DataContext = transactions;
    }
}


public class Core
{
    public double saldo { get; set; }

    public Core()
    {
        saldo = 0;
    }
    public void ChangeSaldo(double newValue)
    {
        saldo = newValue;
    }

}