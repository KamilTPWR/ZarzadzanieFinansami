using System.Windows;
using System.Windows.Controls;

namespace ZarzadzanieFinansami;

public abstract class DataGridUtility
{
    public static void UpdateDataGridView(DataGrid MainDataGrid)
    {
        var scaleRation = 0.20;
        var NON = 0.01;
        var DEF = 1.0; //default
        var gridView = MainDataGrid;
        
        var totalWidth = MainDataGrid.ActualWidth - SystemParameters.VerticalScrollBarWidth;
        if (gridView.Columns.Count == Constants.STATICNUMBEROFCOLUMNS)
        {
            gridView.Columns[0].Width = totalWidth * NON * scaleRation;         // "ID"
            gridView.Columns[1].Width = totalWidth * 1.1 * scaleRation;         // "Nazwa"
            gridView.Columns[2].Width = totalWidth * DEF * scaleRation;         // "Kwota"
            gridView.Columns[3].Width = totalWidth * DEF * scaleRation;         // "Data"
            gridView.Columns[4].Width = totalWidth * 1.5 * scaleRation;         // "Uwagi"
            gridView.Columns[5].Width = totalWidth * 0.5 * scaleRation;         // "Kategorie"
        }
    }
}