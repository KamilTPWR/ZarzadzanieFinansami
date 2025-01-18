using System.Globalization;
using System.Media;
using LiveCharts;
using LiveCharts.Wpf;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using LiveCharts.Dtos;
using Microsoft.VisualBasic;
using ZarządzanieFinansami.Instances;
using ZarządzanieFinansami.Utility;
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
    
    public MainWindow()
    {
        InitializeComponent();
        StartClock();
        UpdateDataGrid();
        UpdateCharts();
        UpdateTextBoxes();
        SetConstants();
        SettingsUtility.DebugLoadSettings();

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
        PageTextBlock.Text = PagesNumberFormat();
        UpdateTextBoxes();
        UpdateDataGrid();
        UpdateCharts();
        DataGridUtility.UpdateDataGridView(MainDataGrid);
        Button_NumberOfRows.Content = FormatNumberOfRows();
    }
    
    private void UpdateDataGrid()
    {
        MainDataGrid.ContextMenu!.Visibility = Visibility.Visible;
        
        _transactions = DbUtility.GetTransactionsFromDatabase(out var outCome);
        
        var transactions = _transactions;
        
        _isDatabaseOpen = outCome;
        
        ToggleButton(outCome);
        
        if (Enum.TryParse<ComparisonField>(_columnHeader, out var field))
        {
            transactions.Sort((x, y) => x.CompareTo(y, field));
            SetSortDirection(transactions, _sortDirection);
            MainDataGrid.DataContext = CropTransactionsToPaginatedTransactions(transactions);
        }
        else
        {
            MainDataGrid.DataContext = CropTransactionsToPaginatedTransactions(transactions);
        }
        MainDataGrid.ContextMenu!.Visibility = Visibility.Hidden;
    }
    
    //extracted methods
    private static string FormatNumberOfRows()
    {
        return Core.NumberOfRows + "/" + DbUtility.GetNumberOfTransactions();
    }
    private static string PagesNumberFormat()
    {
        return " " + Core.Page + "-" + Core.PagesNumber() + " ";
    }
    private void SetSortDirection(List<Transaction> transactions, bool sortDirection)
    {
        if (sortDirection)
        {
            transactions.Reverse();   
        }
    }
    private void ToggleButton(bool outCome)
    {
        AddButtonName.IsEnabled = outCome;
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
    /*                                                 Charts and Display                                                  */
    /***********************************************************************************************************************/
    
    private void UpdateCharts()
    {
        //creating charts
        PieCharsUtility chars = new()
        {
            DataContext = this,
            DatabaseState = _isDatabaseOpen,
            dateHandler = new DateHandler()
        };
        //displaying charts
        chars.UpdateCharts(Pie , TransactionPieChart, CatPieChart);
        SetVisibility();
    }

    private void SetVisibility()
    {
        Top10.Visibility = _isDatabaseOpen ? Visibility.Visible : Visibility.Collapsed;
        TopCat.Visibility = _isDatabaseOpen ? Visibility.Visible : Visibility.Collapsed;
    }

    private void UpdateTextBoxes()
    {
        CalculateValuesForChart(out var tempExpenses, out var tempSaldo);
        FormatDateString(out var _s, out var _l);
        Date.Text = $"Zakres: {_s} — {_l}";
        Saldo.Text = $"Wolny Budżet: {tempSaldo}";
        Wydatki.Text = $"Wydatki: {tempExpenses}";
    }

    private static void FormatDateString(out string s, out string l)
    {
        DateHandler.GetDatesFromRange(out var ss , out var ll);
        s = ss.ToString(Constants.DATEFORMAT);
        l = ll.ToString(Constants.DATEFORMAT);
    }

    private static void CalculateValuesForChart(out string tempExpenses, out string tempSaldo)
    {
        DateHandler.GetDatesFromRange(out var startDate, out var lastDate);
        
        string currencySymbol = CurrencySymbol.Currency[Core.GlobalCurrency];
        string formattedStartDate = startDate.ToString(Constants.DATEFORMAT);
        string formattedLastDate = lastDate.ToString(Constants.DATEFORMAT);
        
        double sum = DbUtility.GetSumOfKwotaInTimeRangeFromDatabase(out _, formattedStartDate, formattedLastDate);
        
        double expenses = Math.Round(sum, 2);
        double saldo = Math.Round(Core.GlobalSaldo - expenses, 2);
        
        tempExpenses = expenses + currencySymbol;
        tempSaldo = saldo + currencySymbol;
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
        var selectedItem = dataGrid?.SelectedIndex;
        if (selectedItem != null && Convert.ToInt32(selectedItem) >= 0)
        {
            if (MessageBox.Show("Czy napewno chcesz usunąć tranzakcje ?", "Usuń tranzakcję.",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var dataItem = MainDataGrid.Items[(int)selectedItem];
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

    private void MenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        var increaseSaldo = new IncreaseSaldo();
        increaseSaldo.ShowDialog();
        UpdateWindow();
    }
    
    private void MenuItem_CatRemowe_OnClick(object sender, RoutedEventArgs e)
    {
        CategoryDeleter window = new CategoryDeleter();
        try
        {
            window.ShowDialog();
            UpdateDataGrid();
            UpdateCharts();
        }
        catch (Exception ex)
        {
        }
    }
    
}
