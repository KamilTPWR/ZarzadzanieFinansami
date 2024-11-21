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

        SystemClock.Text = Constants.DEFAULTCLOCK;
        PageTextBlock.Text = Constants.NULLPAGE;
    }

/***********************************************************************************************************************/
/*                                                Private Methods                                                      */
/***********************************************************************************************************************/
    private void UpdateWindow()
    {
        ChangeSaldoEvent(GetSaldoFromDatabase());
        UpdateDataGrid();
        DataGridUtility.UpdateDataGridView(MyDataGridView);
    }
    private void ChangeSaldoEvent(double newSaldo)
    {
        _core.ChangeSaldo(newSaldo);
        ResultTextDisplay.Text = $"Saldo: {_core.Saldo} $";
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
    }
    private void UsunRekord_OnClick(object sender, RoutedEventArgs e)
    {
        
    }

}