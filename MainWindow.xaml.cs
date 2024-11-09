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
    public MainWindow()
    {
        InitializeComponent();
        StartClock();

        this.ResultTextDisplay.Text = "Some string";
        
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
}