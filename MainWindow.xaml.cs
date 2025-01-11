﻿using System.Media;
using LiveCharts;
using LiveCharts.Wpf;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using LiveCharts.Dtos;
using ZarządzanieFinansami.Windows;

namespace ZarzadzanieFinansami;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>

// ReSharper disable once RedundantExtendsListEntry
public partial class MainWindow : Window
{
    private bool _isClosing = false;
    private bool _sortDirection = true;
    private bool _isDatabaseOpen = false;
    
    private List<Transaction> _transactions = new();
    private string? _columnHeader;
    
    public required SeriesCollection PieSeries { get; set; }
    public required SeriesCollection TransactionPieSeries { get; set; }

    public MainWindow()
    {
        InitializeComponent();
        StartClock();
        UpdateDataGrid();
        UpdateCharts();
        UpdateTextBoxes();
        SetConstants();
        SettingsUtility.DebugLoadSettings();
        //DateTime today = DateTime.Today;
        //DayOfWeek firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
        //int diffToFirstDayOfWeek = (7 + (today.DayOfWeek - firstDayOfWeek)) % 7;
        //DateTime firstDayOfWeekDate = today.AddDays(-diffToFirstDayOfWeek);
        //DateTime lastDayOfWeekDate = firstDayOfWeekDate.AddDays(6);

        //Console.WriteLine($"First day of the week: {firstDayOfWeekDate:yyyy-MM-dd}");
        //Console.WriteLine($"Last day of the week: {lastDayOfWeekDate:yyyy-MM-dd}");
        //MessageBox.Show($"First day of the week: {firstDayOfWeekDate:yyyy-MM-dd}");
        //MessageBox.Show($"Last day of the week: {lastDayOfWeekDate:yyyy-MM-dd}");

        //// First and Last Day of the Month
        //DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
        //DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        //Console.WriteLine($"First day of the month: {firstDayOfMonth:yyyy-MM-dd}");
        //Console.WriteLine($"Last day of the month: {lastDayOfMonth:yyyy-MM-dd}");
    }

    /***********************************************************************************************************************/
    /*                                                Private Methods                                                      */
    /***********************************************************************************************************************/

    private void SetConstants()
    {
        SystemClock.Text = Constants.DEFAULTCLOCK;
        PageTextBlock.Text = Constants.NULLPAGE;
        Button_NumberOfRows.Content = Constants.NULLROWNUMBER;
    }

    private void UpdateWindow()
    {
        var pagesNumberFormat = " " + Core.Page + "-" + Core.PagesNumber() + " ";
        PageTextBlock.Text = pagesNumberFormat;
        UpdateTextBoxes();
        UpdateDataGrid();
        UpdateCharts();
        DataGridUtility.UpdateDataGridView(MainDataGrid);
        Button_NumberOfRows.Content = Core.NumberOfRows + "/" + DbUtility.GetNumberOfTransactions();
    }

    private void UpdateDataGrid()
    {
        MainDataGrid.ContextMenu!.Visibility = Visibility.Visible;
        _transactions = DbUtility.GetTransactionsFromDatabase(out var outCome);
        _isDatabaseOpen = outCome;
        AddButtonName.IsEnabled = outCome;
        if (Enum.TryParse<ComparisonField>(_columnHeader, out var field))
        {
            var transactions = _transactions;
            transactions.Sort((x, y) => x.CompareTo(y, field));
            if (_sortDirection)
            {
                transactions.Reverse();   
            }
            MainDataGrid.DataContext = CropTransactionsToPaginatedTransactions(transactions);
        }
        else
        {
            MainDataGrid.DataContext = CropTransactionsToPaginatedTransactions(DbUtility.GetTransactionsFromDatabase(out _));
        }
        MainDataGrid.ContextMenu!.Visibility = Visibility.Hidden;
    }

    private static List<Transaction> CropTransactionsToPaginatedTransactions(List<Transaction> transactions)
    {
        var paginatedTransactions = transactions
            .Skip((Core.Page - 1) * Core.NumberOfRows)
            .Take(Core.NumberOfRows)
            .ToList();
        return paginatedTransactions;
    }

    
    /***********************************************************************************************************************/
    /*                                                   PieSeries                                                         */
    /***********************************************************************************************************************/
    
    private void UpdateCharts()
    {
        UpdatePieChart();
        UpdateTransactionPieChart();
    }
    
    private void UpdatePieChart()
    {
        SwitchVisibilityOfChart(Pie, _isDatabaseOpen);
        
        var tempSaldo = Math.Round(Core.GlobalSaldo - GetSaldoFromDatabase(), 2);
        var tempExpenses = Math.Round(GetSaldoFromDatabase(), 2);
        
        Pie.SeriesColors = Constants.COLORS;
        PieSeries = new SeriesCollection
        {
            CreateSetupSeries(),
            CreatePieSeries("Wolny budżet", tempSaldo),
            CreatePieSeries("Wydatki", tempExpenses),
        };
        DataContext = this;
    }
    
    // WARNING: Do not modify this code unless necessary!
    private void UpdateTransactionPieChart()
    {
        SwitchVisibilityOfChart(TransactionPieChart, _isDatabaseOpen);
        
        var transactions = DbUtility.GetTransactionsFromDatabase(out _);
        transactions.Sort((x, y) => x.CompareTo(y, ComparisonField.Kwota));
        
        TransactionPieChart.SeriesColors = Constants.COLORS;
        TransactionPieSeries = new SeriesCollection { CreateSetupSeries() };

        AddTransactions(transactions);
        
        TransactionPieChart.Series = TransactionPieSeries;
        DataContext = this;
    }

    private void AddTransactions(List<Transaction> transactions , int amount = 10)
    {
        foreach (var transaction in transactions.Take(amount))
        {
            TransactionPieSeries.Add(new PieSeries
            {
                Title = transaction.Nazwa,
                Values = new ChartValues<double> { transaction.Kwota },
                DataLabels = true
            });
        }
    }

    private void SwitchVisibilityOfChart(PieChart chart, bool visibility)
    {
        if (visibility == false)
        {
            chart.Visibility = Visibility.Collapsed;
        }
        else
        {
            chart.Visibility = Visibility.Visible;
        }
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
    
    //TODO: Remove and Rewrite
    private double GetSaldoFromDatabase()
    {
        var returnValue = 0.0;
        var transactions = _transactions;
        transactions.Clear();
        DataContext = null;
        
        transactions = DbUtility.GetTransactionsFromDatabase(out _);
        foreach (var transaction in transactions) returnValue += transaction.Kwota;

        DataContext = transactions;
        return returnValue;
    }

    private void UpdateTextBoxes()
    {
        string tempSaldo = Math.Round(Core.GlobalSaldo - GetSaldoFromDatabase(), 2) + CurrencySymbol.Currency[Core.GlobalCurrency];
        string tempExpenses = Math.Round(GetSaldoFromDatabase(), 2) + CurrencySymbol.Currency[Core.GlobalCurrency];

        Saldo.Text = $"Wolny Budżet: {tempSaldo}";
        Wydatki.Text = $"Wydatki: {tempExpenses}";
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

    private void MainDataGrid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (Constants.STATICNUMBEROFCOLUMNS == MainDataGrid.Columns.Count)
            DataGridUtility.UpdateDataGridView(MainDataGrid);
    }

    private void MainDataGrid_OnBeginningEdit(object? sender, DataGridBeginningEditEventArgs e)
    {
        e.Cancel = true;
    }

    private void MainDataGrid_Loaded(object sender, RoutedEventArgs e)
    {
        UpdateDataGrid();
        UpdateCharts();
        DataGridUtility.UpdateDataGridView(MainDataGrid);
        UpdateWindow();
        //SettingsUtility.LoadSettings();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (_isClosing) return;
        SystemSounds.Exclamation.Play();
        MessageBoxResult result = MessageBox.Show(
            "Czy napewno chcesz zamknąć program? \n Dane zostaną zapisane.", "Zamknij program",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
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
    
    //TODO: rewrite it
    private void DataGrid_MenuItem_Remove_OnClick(object sender, RoutedEventArgs e)
    {
        if (MainDataGrid.SelectedItems.Count <= 0)
        {
            MessageBox.Show("Nie wybrano żadnych tranzakcji do usunięcia.", "Błąd", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        else
        {
            var columnValues = new List<object>();

            foreach (var selectedItem in MainDataGrid.SelectedItems)
            {
                var item = selectedItem as dynamic;
                if (item != null)
                {
                    columnValues.Add(item.ID);
                }
            }

            var message =
                $"Czy napewno chcesz usunąć następującą ilość tranzakcji: {columnValues.Count} ?\nTej operacji nie da się odwrócić.";

            if (MessageBox.Show(message, "Usuń tranzakcję.", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                foreach (var id in columnValues)
                {
                    DbUtility.DeleteFromDatabase(Convert.ToInt32(id));
                }
                UpdateDataGrid();
                UpdateCharts();
            }
        }
    }


    /***********************************************************************************************************************/
    /*                                                 ContextMenuLogic                                                    */
    /***********************************************************************************************************************/

    private void MainDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
                dataItem = MainDataGrid.Items[(int)selectedItem];
                DbUtility.DeleteFromDatabase((dataItem as Transaction)!.ID);
                UpdateDataGrid();
                UpdateCharts();
            }

        }
    }

    private void MainDataGrid_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        DataGrid dataGrid = (sender as DataGrid)!;
        if (dataGrid.ContextMenu != null && dataGrid.ContextMenu.Visibility == Visibility.Collapsed)
        {
            Point mousePosition = Mouse.GetPosition(Application.Current.MainWindow);
            if (Application.Current.MainWindow != null)
            {
                Point screenPoint = Application.Current.MainWindow.PointToScreen(mousePosition);
                dataGrid.ContextMenu.Placement = PlacementMode.Absolute;
                dataGrid.ContextMenu.HorizontalOffset = screenPoint.X;
                dataGrid.ContextMenu.VerticalOffset = screenPoint.Y;
            }
        }

        if (dataGrid.ContextMenu != null && MainDataGrid.SelectedItems.Count > 0)
        {
            foreach (var selectedItem in MainDataGrid.SelectedItems)
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
        SystemSounds.Exclamation.Play();
        MessageBoxResult result = MessageBox.Show(
            "Czy napewno chcesz zamknąć program? \n Dane zostaną zapisane.", "Zamknij program", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            _isClosing = true;
            Application.Current.Shutdown();
        }
    }

    private void MenuItem_Usun_wszystkie_rekordy_OnClick(object sender, RoutedEventArgs e)
    {
        var IDs = new List<int>();
        foreach (var transaction in _transactions)
        {
            IDs.Add(transaction.ID);
        }

        if (IDs.Count <= 0)
        {
            ShowNullDataBaseError();
            return;
        }
        
        SystemSounds.Exclamation.Play();
        var message =
            $"Czy napewno chcesz usunąć następującą ilość tranzakcji: {IDs.Count} ?\nTej operacji nie da się odwrócić.";
        if (MessageBox.Show(message, "Usuń tranzakcję.", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
            MessageBoxResult.Yes)
        {
            foreach (var id in IDs)
            {
                DbUtility.DeleteFromDatabase(Convert.ToInt32(id));
            }
            UpdateWindow();
        }
    }

    private void ShowNullDataBaseError()
    {
        MessageBox.Show("Nie wybrano bazy danych.", "Brak bazy danych", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    private void MenuItem_Ustawienia_OnClick(object sender, RoutedEventArgs e)
    {
        Settings setSettingsWindow = new Settings();
        setSettingsWindow.ShowDialog();
        UpdateWindow();
    }
}
