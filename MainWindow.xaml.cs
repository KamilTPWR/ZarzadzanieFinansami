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
    //flags
    private bool _fIsClosing = false;
    protected Core PCore = new();
    
    public List<Transaction> Transactions = new();
    public SeriesCollection PieSeries { get; set; }
    public SeriesCollection TransactionPieSeries { get; set; }

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
        PCore.SetSaldo(newSaldo);
        Saldo.Text = $"Saldo: {(GetSaldoFromDatabase()*0.5):F2} $";
        Wydatki.Text = $"Wydatki: {(GetSaldoFromDatabase()):F2} $";
    }

    private void UpdateDataGrid()
    {
        MyDataGridView.ContextMenu!.Visibility = Visibility.Visible;
        Transactions.Clear();
        DataContext = null;
        Transactions = DbUtility.GetFromDatabase(@"SELECT * FROM ListaTranzakcji");
        var paginatedTransactions = Transactions
            .Skip((Core.Page - 1) * Core.NumberOfRows)
            .Take(Core.NumberOfRows)
            .ToList();
        MyDataGridView.DataContext = paginatedTransactions;
        MyDataGridView.ContextMenu!.Visibility = Visibility.Hidden;
        UpdatePieChart();
        UpdateTransactionPieChart();
    }
    private void UpdatePieChart()
    {
        double x = GetSaldoFromDatabase() * 1.5;
        double zostalo = GetSaldoFromDatabase();
        
        double wydano = x - zostalo;
        PieSeries = new SeriesCollection{
                new PieSeries { Title = "Wydati", Values = new ChartValues<double> {Math.Round(zostalo, 2)}, DataLabels = true },
                new PieSeries { Title = "Wolny budzet", Values = new ChartValues<double> {Math.Round(wydano, 2)}, DataLabels = true }
            };
        DataContext = this;
    }
    private void UpdateTransactionPieChart()
    {
        double x = GetSaldoFromDatabase()*1.5;
        double zostalo = GetSaldoFromDatabase();
        double wydano = x - zostalo;
        var _transactions = DbUtility.GetFromDatabase();
        _transactions.Sort((x, y) => y.Kwota.CompareTo(x.Kwota));
        TransactionPieSeries = new SeriesCollection();
        foreach (var transaction in _transactions.Take(10)) // Pobierz maksymalnie 10 największych transakcji
        {
            TransactionPieSeries.Add(new PieSeries
            {
                Title = transaction.Nazwa, // Użyj nazwy transakcji jako tytuł
                Values = new ChartValues<double> { transaction.Kwota },
                DataLabels = true
            });
        }
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
        if (Constants.STATICNUMBEROFCOLUMNS == MyDataGridView.Columns.Count) DataGridUtility.UpdateDataGridView(MyDataGridView);
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
    
    private void ButtonNumberControll_OnClick(object sender, RoutedEventArgs e)
    {
        var numberOfRecordsOnPage = new NumberOfRecordsOnPage(Core.NumberOfRows);
        numberOfRecordsOnPage.ShowDialog();
        UpdateWindow();
    }
    private void MenuItem_View_OnClick(object sender, RoutedEventArgs e)
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

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (!_fIsClosing)
        {
            MessageBoxResult result = MessageBox.Show(
                "Na pewno chcesz zamknąć program? Niezapisane dane zostaną utracone.", "Zamknij program",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
    
/***********************************************************************************************************************/
/*                                                 ContextMenuLogic                                                    */
/***********************************************************************************************************************/

    private void MyDataGridView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        var dataGrid = sender as DataGrid;
        object? dataItem;
        var selectedItem = dataGrid?.SelectedIndex;
        if (selectedItem != null && Convert.ToInt32(selectedItem) >= 0)
        {
            if (MessageBox.Show("Czy napewno chcesz usunąć tranzakcje ?", "Usuń tranzakcję.",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                dataItem = MyDataGridView.Items[selectedItem ?? 0]; // Retrieve the data object
                //MessageBox.Show($"Row {selectedItem} data: {(dataItem as Transaction)!.ID}");
                DbUtility.DeleteFromDatabase((dataItem as Transaction)!.ID);
                UpdateDataGrid();
            }
    
        }
    }
    
    private void MyDataGridView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        DataGrid dataGrid = sender as DataGrid;
        if (dataGrid.ContextMenu.Visibility == Visibility.Collapsed)
        {
            Point mousePosition = Mouse.GetPosition(Application.Current.MainWindow);
            Point screenPoint = Application.Current.MainWindow.PointToScreen(mousePosition);
            dataGrid.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Absolute;
            dataGrid.ContextMenu.HorizontalOffset = screenPoint.X;
            dataGrid.ContextMenu.VerticalOffset = screenPoint.Y;
        }

        if (MyDataGridView.SelectedItems != null || dataGrid.ContextMenu != null ||MyDataGridView.SelectedItems.Count > 0)
        {
            foreach (var selectedItem in MyDataGridView.SelectedItems)
            {
                var item = selectedItem as dynamic;
                if (item != null)
                {
                    dataGrid.ContextMenu.Visibility = Visibility.Visible;
                    dataGrid.ContextMenu.PlacementTarget = dataGrid;
                    dataGrid.ContextMenu.IsOpen = true;
                }
            }
        }
    }
    
    private void MenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        if (MyDataGridView.SelectedItems != null && MyDataGridView.SelectedItems.Count > 0)
        {
            var columnValues = new List<object>();

            foreach (var selectedItem in MyDataGridView.SelectedItems)
            {
                var item = selectedItem as dynamic;
                if (item != null)
                {
                    columnValues.Add(item.ID);
                }
            }
            
            string message = $"Czy napewno chcesz usunąć następującą ilość tranzakcji: {columnValues.Count} ?\nTej operacji nie da się odwrócić.";
            if (MessageBox.Show(message, "Usuń tranzakcję.", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (var id in columnValues)
                {
                    DbUtility.DeleteFromDatabase(Convert.ToInt32(id));
                }
                UpdateDataGrid();
            }
        }
        else
        {
            MessageBox.Show("Nie wybrano żadnych tranzakcji do usunięcia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

/***********************************************************************************************************************/
/*                                                 State Set-up                                                        */
/***********************************************************************************************************************/

    private void MyDataGridView_OnBeginningEdit(object? sender, DataGridBeginningEditEventArgs e)
    {
        e.Cancel = true;
    }

    private void MenuItem_Plik_Zamknij_OnClick(object sender, RoutedEventArgs e)
    {
        MessageBoxResult result = MessageBox.Show(
            "Na pewno chcesz zamknąć program? Niezapisane dane zostaną utracone.", "Zamknij program", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
            _fIsClosing = true;
            Application.Current.Shutdown();
        }
    }
}
