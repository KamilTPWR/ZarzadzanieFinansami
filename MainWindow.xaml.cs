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
    Core MainCore = new Core();
    public List<Transaction> Transactions = new List<Transaction>();

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
        MainCore.SetSaldo(newSaldo);
        ResultTextDisplay.Text = $"Saldo: {MainCore.Saldo} $";
    }
    private void UpdateDataGrid()
    {
        Transactions.Clear();
        DataContext = null;
        Transactions = DbUtility.GetFromDatabase(@"SELECT Nazwa, Kwota, Data, Uwagi FROM ListaTranzakcji");
        DataContext = Transactions;
    }
    private double GetSaldoFromDatabase()
    {
        double returnValue = 0.0;
        Transactions.Clear();
        DataContext = null;
        Transactions = DbUtility.GetFromDatabase(@"SELECT Kwota FROM ListaTranzakcji");
        foreach (var transaction in Transactions)
        {
            returnValue += transaction.Kwota;
        }

        DataContext = Transactions;
        return returnValue;
    }
    private void StartClock()
    {
        DispatcherTimer timer = new DispatcherTimer();
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
        if (4 == MyDataGridView.Columns.Count)
        {
            DataGridUtility.UpdateDataGridView(MyDataGridView);
        }
    }
    private void DodajRekord_OnClick(object sender, RoutedEventArgs e)
    {
        IncreaseSaldo increaseSaldo = new IncreaseSaldo();
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
        NumberOfRecordsOnPage numberOfRecordsOnPage = new NumberOfRecordsOnPage(Core.NumberOfRows);
        numberOfRecordsOnPage.ShowDialog();
        UpdateWindow();
    }
    private void ButtonRight_OnClick(object sender, RoutedEventArgs e)
    {
        Core.Page = Core.Page <= Core.PagesNumber() ? ++Core.Page : Core.Page;
        UpdateWindow();
    }
    private void ButtonLeft_OnClick(object sender, RoutedEventArgs e)
    {
        Core.Page = Core.Page > 1 ? --Core.Page : Core.Page;
        UpdateWindow();
    }
}