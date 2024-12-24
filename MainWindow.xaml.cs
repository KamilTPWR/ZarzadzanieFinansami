using LiveCharts;
using LiveCharts.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using LiveCharts.Dtos;

namespace ZarzadzanieFinansami;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>

// ReSharper disable once RedundantExtendsListEntry
public partial class MainWindow : Window
{
    private bool _isClosing = false;
    private List<Transaction> _transactions = new();
    private string? _columnHeader;
    private bool _sortDirection = true;
    public required SeriesCollection PieSeries { get; set; }
    public required SeriesCollection TransactionPieSeries { get; set; }
    public MainWindow()
    {
        InitializeComponent();
        StartClock();
        UpdateDataGrid();
        ChangeSaldoEvent();
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
        ChangeSaldoEvent();
        UpdateDataGrid();
        UpdateCharts();
        DataGridUtility.UpdateDataGridView(MyDataGridView);
        ButtonNumberControll.Content = Core.NumberOfRows + "/" + DbUtility.GetNumberOfTransactions();
    }

    private void UpdateDataGrid()
    {
        MyDataGridView.ContextMenu!.Visibility = Visibility.Visible;
        _transactions = DbUtility.GetFromDatabase();
        if (Enum.TryParse<ComparisonField>(_columnHeader, out var field))
        {
            var transactions = _transactions;
            transactions.Sort((x, y) => x.CompareTo(y, field));
            if (_sortDirection)
            {
                transactions.Reverse();   
            }
            MyDataGridView.DataContext = CropTransactionsToPaginatedTransactions(transactions);
        }
        else
        {
            MyDataGridView.DataContext = CropTransactionsToPaginatedTransactions(DbUtility.GetFromDatabase());
        }
        MyDataGridView.ContextMenu!.Visibility = Visibility.Hidden;
        UpdateCharts();
    }

    private static List<Transaction> CropTransactionsToPaginatedTransactions(List<Transaction> transactions)
    {
        var paginatedTransactions = transactions
            .Skip((Core.Page - 1) * Core.NumberOfRows)
            .Take(Core.NumberOfRows)
            .ToList();
        return paginatedTransactions;
    }

    private void UpdatePieChart()
    {
        double x = GetSaldoFromDatabase() * 1.5;
        double zostalo = GetSaldoFromDatabase();
        double wydano = x - zostalo;
        Pie.SeriesColors = Constants.COLORS;
        PieSeries = new SeriesCollection
        {
            CreateSetupSeries(),
            CreatePieSeries("Wolny budzet", wydano),
            CreatePieSeries("Zostało", zostalo),
        };
        DataContext = this;
    }

    private void UpdateTransactionPieChart()
    {
        var transactions = _transactions;
        transactions.Sort((x, y) => x.CompareTo(y, ComparisonField.Kwota));
        TransactionPieSeries = new SeriesCollection
        {
            CreateSetupSeries(),
        };
        TransactionPieChart.SeriesColors = Constants.COLORS;
        foreach (var transaction in transactions.Take(10))
        {
            TransactionPieSeries.Add(new PieSeries
            {
                Title = transaction.Nazwa,
                Values = new ChartValues<double> { transaction.Kwota },
                DataLabels = true
            });
        }
        DataContext = this;
    }

    private static PieSeries CreatePieSeries(string title, double value)
    {
        return new PieSeries
        {
            Title = title,
            Values = new ChartValues<double> { Math.Round(value, 2) },
            DataLabels = true
        };
    }

    private static PieSeries CreateSetupSeries()
    {
        return new PieSeries
        {
            Title = Constants.WHITESPACEPIECHART,
            Values = new ChartValues<double> { 0 },
            Opacity = 0.1,
        };
    }
    
    private void UpdateCharts()
    {
        UpdatePieChart();
        UpdateTransactionPieChart();
    }

    private double GetSaldoFromDatabase()
    {
        var returnValue = 0.0;
        _transactions.Clear();
        DataContext = null;
        _transactions = DbUtility.GetFromDatabase();
        foreach (var transaction in _transactions) returnValue += transaction.Kwota;

        DataContext = _transactions;
        return returnValue;
    }

    private void ChangeSaldoEvent()
    {
        Saldo.Text = $"Saldo: {GetSaldoFromDatabase() * 0.5:F2} $";
        Wydatki.Text = $"Wydatki: {GetSaldoFromDatabase():F2} $";
    }

    /***********************************************************************************************************************/
    /*                                                Clock Events                                                         */
    /***********************************************************************************************************************/

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
    /*                                         Auto Events Handlers                                                        */
    /***********************************************************************************************************************/

    private void MyDataGridView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (Constants.STATICNUMBEROFCOLUMNS == MyDataGridView.Columns.Count)
            DataGridUtility.UpdateDataGridView(MyDataGridView);
    }

    private void MyDataGridView_OnBeginningEdit(object? sender, DataGridBeginningEditEventArgs e)
    {
        e.Cancel = true;
    }

    private void MyDataGridView_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateDataGrid();
        DataGridUtility.UpdateDataGridView(MyDataGridView);
        UpdateWindow();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_isClosing) return;
        MessageBoxResult result = MessageBox.Show(
            "Na pewno chcesz zamknąć program? Niezapisane dane zostaną utracone.", "Zamknij program",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        switch (result)
        {
            case MessageBoxResult.No:
                e.Cancel = true;
                break;
            case MessageBoxResult.Yes:
                Application.Current.Shutdown();
                break;
        }
    }

    /***********************************************************************************************************************/
    /*                                              Events Handlers                                                        */
    /***********************************************************************************************************************/

    private void AddButton_OnClick(object sender, RoutedEventArgs e)
    {
        var increaseSaldo = new IncreaseSaldo();
        increaseSaldo.ShowDialog();
        UpdateWindow();
    }

    private void ButtonNumberControll_OnClick(object sender, RoutedEventArgs e)
    {
        var numberOfRecordsOnPage = new NumberOfRecordsOnPage(Core.NumberOfRows);
        numberOfRecordsOnPage.ShowDialog();
        UpdateWindow();
    }

    private void ButtonRight_OnClick(object sender, RoutedEventArgs e)
    {
        if (Core.Page < Core.PagesNumber())
        {
            Core.Page = ++Core.Page;
            UpdateWindow();
        }

        e.Handled = true;
    }

    private void ButtonLeft_OnClick(object sender, RoutedEventArgs e)
    {
        if (Core.Page > 1)
        {
            Core.Page = --Core.Page;
            UpdateWindow();
        }

        e.Handled = true;
    }
    
    private void GridViewOnSorting_OnClick(object sender, DataGridSortingEventArgs e)
    {
        _sortDirection = !_sortDirection;
        _columnHeader = e.Column.Header.ToString();
        UpdateDataGrid();
        e.Handled = true;
    }
    
    private void DataGrid_MenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        if (MyDataGridView.SelectedItems.Count <= 0)
        {
            MessageBox.Show("Nie wybrano żadnych tranzakcji do usunięcia.", "Błąd", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        else
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

            var message =
                $"Czy napewno chcesz usunąć następującą ilość tranzakcji: {columnValues.Count} ?\nTej operacji nie da się odwrócić.";

            if (MessageBox.Show(message, "Usuń tranzakcję.", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                MessageBoxResult.Yes)
            {
                foreach (var id in columnValues)
                {
                    DbUtility.DeleteFromDatabase(Convert.ToInt32(id));
                }

                UpdateDataGrid();
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
                dataItem = MyDataGridView.Items[(int)selectedItem];
                DbUtility.DeleteFromDatabase((dataItem as Transaction)!.ID);
                UpdateDataGrid();
            }

        }
    }

    private void MyDataGridView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        DataGrid dataGrid = (sender as DataGrid)!;
        if (dataGrid.ContextMenu != null && dataGrid.ContextMenu.Visibility == Visibility.Collapsed)
        {
            Point mousePosition = Mouse.GetPosition(Application.Current.MainWindow);
            if (Application.Current.MainWindow != null)
            {
                Point screenPoint = Application.Current.MainWindow.PointToScreen(mousePosition);
                dataGrid.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Absolute;
                dataGrid.ContextMenu.HorizontalOffset = screenPoint.X;
                dataGrid.ContextMenu.VerticalOffset = screenPoint.Y;
            }
        }

        if (dataGrid.ContextMenu != null && MyDataGridView.SelectedItems.Count > 0)
        {
            foreach (var selectedItem in MyDataGridView.SelectedItems)
            {
                var item = selectedItem as dynamic;
                if (item != null && dataGrid.ContextMenu != null)
                {
                    dataGrid.ContextMenu!.Visibility = Visibility.Visible;
                    dataGrid.ContextMenu.PlacementTarget = dataGrid;
                    dataGrid.ContextMenu.IsOpen = true;
                }
            }
        }
    }

    /***********************************************************************************************************************/
    /*                                           Menu Items Events Handlers                                                */
    /***********************************************************************************************************************/

    private void MenuItem_Otworz_OnClick(object sender, RoutedEventArgs e)
    {
        DbUtility.OpenDatabase();
        UpdateWindow();
    }

    private void MenuItem_Nowa_OnClick(object sender, RoutedEventArgs e)
    {
        DbUtility.CreateDatabase();
        UpdateWindow();
    }

    private void MenuItem_Zapisz_jako_OnClick(object sender, RoutedEventArgs e)
    {
        DbUtility.SaveDatabase();
        UpdateWindow();
    }

    private void MenuItem_View_OnClick(object sender, RoutedEventArgs e)
    {
        var numberOfRecordsOnPage = new NumberOfRecordsOnPage(Core.NumberOfRows);
        numberOfRecordsOnPage.ShowDialog();
        UpdateWindow();
    }

    private void MenuItem_Wyjdz_OnClick(object sender, RoutedEventArgs e)
    {
        MessageBoxResult result = MessageBox.Show(
            "Na pewno chcesz zamknąć program? Niezapisane dane zostaną utracone.", "Zamknij program",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (result == MessageBoxResult.Yes)
        {
            _isClosing = true;
            Application.Current.Shutdown();
        }
    }
    
}
