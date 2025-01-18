using System.Media;
using System.Windows;
using ZarzadzanieFinansami;

namespace ZarządzanieFinansami.Windows
{
    public partial class CategoryDeleter : Window
    {
        private bool _isClosing = false;
        private bool _isAccepted = false;
        private List<string> _categories = new();

        public CategoryDeleter()
        {
            InitializeComponent();
            UpdateComboBox();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            var message = SetMessageBoxMessage();
            if (ShowMessageBox(message)) return;
            int kategoria = Convert.ToInt32(Cats.SelectedItem.ToString()!.Split(".")[0]);
            DbUtility.DeleteFromDatabase(kategoria, "Kategorie");
            Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _isClosing = true;
            Close();
        }
        private void UpdateComboBox()
        {
            var categories = DbUtility.GetCategoriesFromDatabase();
            _categories.Clear();
            AddCategoriesToList(categories);
            Cats.ItemsSource = _categories;
            if (_categories.Count == 0)
            {
                MessageBox.Show("Brak kategorii do usunięcia");
                _isClosing = true;
                Close();
            }
        }

        
        //ex
        private static bool ShowMessageBox(string message)
        {
            if (MessageBox.Show(message, "Usuń kategorię.", MessageBoxButton.YesNo, MessageBoxImage.Question) !=
                MessageBoxResult.Yes) return true;
            return false;
        }
        private string SetMessageBoxMessage()
        {
            var message = $"Czy napewno chcesz usunąć kategorię: {Cats.Text.Split(". ")[1]} ?\n" +
                          $"Wszystkie pozycje tej kategori zostaną usunięte\n" +
                          $"Tej operacji nie da się odwrócić.";
            return message;
        }
        private void AddCategoriesToList(List<Category> temp)
        {
            foreach (Category category in temp)
            {
                _categories.Add(category.ID + ". " + category.Name);
            }
        }


        /***********************************************************************************************************************/
        /*                                              Back Events Handlers                                                   */
        /*                                                   Black Magic                                                       */
        /***********************************************************************************************************************/
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwndSource = System.Windows.Interop.HwndSource.FromHwnd(new System.Windows.Interop.WindowInteropHelper(this).Handle);
            if (hwndSource != null) hwndSource.AddHook(HwndMessageHook);
        }

        private IntPtr HwndMessageHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MOVE = 0xF010;
            if (msg == WM_SYSCOMMAND && (wParam.ToInt32() & 0xFFF0) == SC_MOVE) handled = true;
            return IntPtr.Zero;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (_isClosing) return;
            if (_isAccepted) return;
            SystemSounds.Beep.Play();
            Activate();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _isClosing = true;
        }
    }
}
