using System.Windows;
using System.Windows.Controls;

namespace ZarzadzanieFinansami;

public abstract class DataGridUtility
{
    public static void UpdateDataGridView(DataGrid mainDataGrid)
    {
        const double scaleRation = 0.20;
        var NON = 0.01;
        var DEF = 1.0; //default

        var totalWidth = mainDataGrid.ActualWidth - SystemParameters.VerticalScrollBarWidth;
        if (mainDataGrid.Columns.Count == Constants.STATICNUMBEROFCOLUMNS)
        {
            mainDataGrid.Columns[0].Width = totalWidth * NON * scaleRation;         // "ID"
            mainDataGrid.Columns[1].Width = totalWidth * 1.1 * scaleRation;         // "Nazwa"
            mainDataGrid.Columns[2].Width = totalWidth * DEF * scaleRation;         // "Kwota"
            mainDataGrid.Columns[3].Width = totalWidth * DEF * scaleRation;         // "Data"
            mainDataGrid.Columns[4].Width = totalWidth * 1.5 * scaleRation;         // "Uwagi"
            mainDataGrid.Columns[5].Width = totalWidth * 0.5 * scaleRation;         // "Kategorie"
        }
    }
}