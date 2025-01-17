using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using ZarzadzanieFinansami;
using ZarządzanieFinansami.Instances;

namespace ZarządzanieFinansami.Utility;

public class PieCharsUtility
{
    public object DataContext { get; set; }
    public bool DatabaseState { get; set; }
    public DateHandler dateHandler { get; set; }
    
    public void UpdateCharts(PieChart pie, PieChart transactionPieChart, PieChart catPieChart)
    {
        UpdateSumPieChart(pie);
        UpdateTransactionPieChart(transactionPieChart);
        UpdateCatPieChart(catPieChart);
    }
    
    //pie
    private void UpdateSumPieChart(PieChart pie)
    {
        SwitchVisibilityOfChart(pie, DatabaseState);
        
        var startDate = dateHandler.GetStartDateFromRange(Core.GlobalDataRange).ToString(Constants.DATEFORMAT);
        var endDate = dateHandler.GetLastDateFromRange(Core.GlobalDataRange).ToString(Constants.DATEFORMAT);

        dateHandler.PrintDates();
        
        var sumOfKwotaInTimeRangeFromDatabase = DbUtility.GetSumOfKwotaInTimeRangeFromDatabase(out _, startDate, endDate );
        
        var tempSaldo = Math.Round(Core.GlobalSaldo - sumOfKwotaInTimeRangeFromDatabase, 2);
        var tempExpenses = Math.Round(sumOfKwotaInTimeRangeFromDatabase, 2);
        
        if (tempSaldo < 0) tempSaldo = 0;
        
        pie.SeriesColors = Constants.COLORSFOR2;
        
        SeriesCollection pieSeries = new SeriesCollection
        {
            CreatePieSeries("Wolny budżet", tempSaldo),
            CreatePieSeries("Wydatki", tempExpenses),
        };
        
        pie.Series = pieSeries;
        
        DataContext = this;
    }
    
    //transactionPieChart
    private void UpdateTransactionPieChart(PieChart transactionPieChart)
    {
        SwitchVisibilityOfChart(transactionPieChart, DatabaseState);
        
        var transactions = DbUtility.GetTransactionsFromDatabase(out _);
        transactions.Sort((x, y) => x.CompareTo(y, ComparisonField.Kwota));
        
        transactionPieChart.SeriesColors = Constants.COLORS;
        SeriesCollection transactionPieSeries = new();

        AddTransactionsToPieSeries(transactionPieSeries, transactions);
        
        transactionPieChart.Series = transactionPieSeries;
        DataContext = this;
    }
    private void AddTransactionsToPieSeries(SeriesCollection transactionPieSeries , List<Transaction> transactions , int amount = 10)
    {
        foreach (var transaction in transactions.Take(amount))
        {
            string title = transaction.Nazwa;
            
            transactionPieSeries.Add(new PieSeries
            {
                Title = title.Length <= Constants.SIZEOFLEGEND ? title : title.Substring(0, Constants.SIZEOFLEGEND) + "...",
                Values = new ChartValues<double> { transaction.Kwota },
                DataLabels = true
            });
        }
    }
    
    //catPieChart
    private void UpdateCatPieChart(PieChart catPieChart)
    {
        SwitchVisibilityOfChart(catPieChart, DatabaseState);
        
        var cat = DbUtility.GetSumByCategory(out _);
        
        catPieChart.SeriesColors = Constants.COLORS;
        SeriesCollection transactionPieSeries = new();

        AddCatToPieSeries(transactionPieSeries, cat);
        
        catPieChart.Series = transactionPieSeries;
        DataContext = this;
    }
    private void AddCatToPieSeries(SeriesCollection cats , List<Tuple<double,string>> cat , int amount = 10)
    {
        foreach (var c in cat.Take(amount))
        {
            double sum = c.Item1;
            string title = c.Item2; 
            
            cats.Add(new PieSeries
            {
                Title = title.Length <= Constants.SIZEOFLEGEND ? title : title.Substring(0, Constants.SIZEOFLEGEND) + "...",
                Values = new ChartValues<double> { sum },
                DataLabels = true
            });
        }
    }
    
    //priv
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
}