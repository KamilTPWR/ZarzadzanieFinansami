using System.Windows;
using System.Windows.Controls;

namespace ZarządzanieFinansami;

public abstract class DataGridUtility
{
    public static void UpdateDataGridView(DataGrid MyDataGridView)
    {
        var scaleRation = 0.20;
        var gridView = MyDataGridView;

        double totalWidth = MyDataGridView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
        
        gridView.Columns[0].Width = totalWidth * scaleRation;  // "Saldo"
        gridView.Columns[1].Width = totalWidth * scaleRation;  // "Zmiana"
        gridView.Columns[2].Width = totalWidth * scaleRation;  // "Data"
        gridView.Columns[3].Width = totalWidth * 2*(scaleRation + 0.01); // "Uwagi"
    }
    
}