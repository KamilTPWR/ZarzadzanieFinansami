using System.Media;
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
public partial class MainWindow : Window
{
    Core core = new Core();
    public MainWindow()
    {   
        InitializeComponent();
        StartClock();

        ChangeSaldoEvent(0);
        
        ResultTextDisplay.Text = $"Saldo: {core.saldo} $";
        
        //Napisy domyśne
        SystemClock.Text = "00/00/0000 00:00:00";
        
        //Zarządzanie zegarem w prawym górnym rogu
        void StartClock()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TickEvent;
            timer.Start();
        }
        void TickEvent(object sender, EventArgs e)
        {
            SystemClock.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
    private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var scaleRation = 0.20;
        var gridView = MyListView.View as GridView;

        double totalWidth = MyListView.ActualWidth - SystemParameters.VerticalScrollBarWidth;

        gridView.Columns[0].Width = totalWidth * scaleRation;  // "Saldo"
        gridView.Columns[1].Width = totalWidth * scaleRation;  // "Zmiana"
        gridView.Columns[2].Width = totalWidth * scaleRation;  // "Data"
        gridView.Columns[3].Width = totalWidth * 2*(scaleRation + 0.01); // "Uwagi"
        
    }
    private void Button_OnClick(object sender, RoutedEventArgs e)
    {
        // ChangeSaldoEvent(core.saldo+1);
        
        IncreaseSaldo increaseSaldo = new IncreaseSaldo();
        increaseSaldo.Show();
        
    }
    private void ChangeSaldoEvent(double newSaldo)
    {
        core.ChangeSaldo(newSaldo);
        ResultTextDisplay.Text = $"Saldo: {core.saldo} $";
    }
}

public class Core
{
    public double saldo { get; set; }

    public Core()
    {
        saldo = 0;
    }
    public void ChangeSaldo(double newValue)
    {
        saldo = newValue;
    }

}