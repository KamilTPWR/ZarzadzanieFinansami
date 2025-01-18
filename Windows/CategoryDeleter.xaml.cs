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
            int kategoria = Convert.ToInt32(Cats.SelectedItem.ToString()!.Split(".")[0]);
            DbUtility.DeleteFromDatabase(kategoria, "Kategorie");
            MessageBox.Show(_categories.Count.ToString());
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _isClosing = true;
            Close();
        }
        private void UpdateComboBox()
        {
            List<Category> temp = DbUtility.GetCategoriesFromDatabase();
            _categories.Clear();
            Cats.ItemsSource = _categories;
            foreach (Category category in temp)
            {
                _categories.Add(category.ID + ". " + category.Name);
            }
            Cats.ItemsSource = _categories;
            if (_categories.Count == 0)
            {
                MessageBox.Show("Brak kategorii do usunięcia");
                _isClosing = true;
                Close();
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
            //hehe
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
