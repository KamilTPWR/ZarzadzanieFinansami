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
using System.CodeDom;
using LiveCharts;
using LiveCharts.Wpf;

namespace ZarzadzanieFinansami;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>

// ReSharper disable once RedundantExtendsListEntry
public partial class MainWindow : Window
{
    private Core _mainCore = new();
    public List<Transaction> Transactions = new();
    public SeriesCollection PieSeries { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        StartClock();
        UpdateDataGrid();
        ChangeSaldoEvent(GetSaldoFromDatabase());
        SetConstants();
    }

/***********************************************************************************************************************/
/*                                                Private Methods                                                      */
/***********************************************************************************************************************/
    private void SetConstants()
    {
        SystemClock.Text = Constants.DEFAULTCLOCK;
        PageTextBlock.Text = Constants.NULLPAGE;
        ButtonNumberControll.Content = Constants.NULLROWNUMBER;
    }

    private void UpdateWindow()
    {
        PageTextBlock.Text = " " + Core.Page + "-" + Core.PagesNumber() + " ";
        ChangeSaldoEvent(GetSaldoFromDatabase());
        UpdateDataGrid();
        DataGridUtility.UpdateDataGridView(MyDataGridView);
        ButtonNumberControll.Content = Core.NumberOfRows + "/" + DbUtility.GetNumberOfTransactions();
    }

    private void ChangeSaldoEvent(double newSaldo)
    {
        _mainCore.SetSaldo(newSaldo);
        ResultTextDisplay.Text = $"Saldo: {_mainCore.Saldo} $";
    }

    private void UpdateDataGrid()
    {
        Transactions.Clear();
        DataContext = null;
        Transactions = DbUtility.GetFromDatabase(@"SELECT * FROM ListaTranzakcji");
        var paginatedTransactions = Transactions
            .Skip((Core.Page - 1) * Core.NumberOfRows)
            .Take(Core.NumberOfRows)
            .ToList();
        MyDataGridView.DataContext = paginatedTransactions;
        UpdatePieChart();
    }
    private void UpdatePieChart()
    {
        double x = GetSaldoFromDatabase()*1.5;
        double zostalo = GetSaldoFromDatabase();
        double wydano = x - zostalo;
        PieSeries = new SeriesCollection(){
                new PieSeries { Title = "Wolny budzet", Values = new ChartValues<double> { wydano}, DataLabels = true },
                new PieSeries { Title = "Wydati", Values = new ChartValues<double> {zostalo}, DataLabels = true }
            };
        DataContext = this;
    }

    private double GetSaldoFromDatabase()
    {
        var returnValue = 0.0;
        Transactions.Clear();
        DataContext = null;
        Transactions = DbUtility.GetFromDatabase(@"SELECT Kwota FROM ListaTranzakcji");
        foreach (var transaction in Transactions) returnValue += transaction.Kwota;

        DataContext = Transactions;
        return returnValue;
    }

    private void StartClock()
    {
        var timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += TickEvent;
        timer.Start();
    }

    private void TickEvent(object? sender, EventArgs e)
    {
        SystemClock.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    }

/***********************************************************************************************************************/
/*                                              Events Handlers                                                        */
/***********************************************************************************************************************/
    private void MyDataGridView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (4 == MyDataGridView.Columns.Count) DataGridUtility.UpdateDataGridView(MyDataGridView);
    }

    private void DodajRekord_OnClick(object sender, RoutedEventArgs e)
    {
        var increaseSaldo = new IncreaseSaldo();
        increaseSaldo.ShowDialog();
        UpdateWindow();
    }

    private void MyDataGridView_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateDataGrid();
        DataGridUtility.UpdateDataGridView(MyDataGridView);
        UpdateWindow();
    }

    private void UsunRekord_OnClick(object sender, RoutedEventArgs e)
    {
    }

    private void ButtonNumberControll_OnClick(object sender, RoutedEventArgs e)
    {
        var numberOfRecordsOnPage = new NumberOfRecordsOnPage(Core.NumberOfRows);
        numberOfRecordsOnPage.ShowDialog();
        UpdateWindow();
    }

    private void ButtonRight_OnClick(object sender, RoutedEventArgs e)
    {
        Core.Page = Core.Page < Core.PagesNumber() ? ++Core.Page : Core.Page;
        UpdateWindow();
    }

    private void ButtonLeft_OnClick(object sender, RoutedEventArgs e)
    {
        Core.Page = Core.Page > 1 ? --Core.Page : Core.Page;
        UpdateWindow();
    }

    private void MyDataGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var dataGrid = sender as DataGrid;
        object dataItem;
        var selectedItem = dataGrid?.SelectedIndex;
        if (selectedItem != null && Convert.ToInt32(selectedItem) >= 0)
        {
            if (MessageBox.Show("Czy napewno chcesz usunac tranzakcje", "Save file",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                dataItem = MyDataGridView.Items[selectedItem ?? 0]; // Retrieve the data object
                MessageBox.Show($"Row {selectedItem} data: {(dataItem as Transaction).ID}");
                DbUtility.DeleteFromDatabase((dataItem as Transaction).ID);
                UpdateDataGrid();
            }

        }
    }
}
