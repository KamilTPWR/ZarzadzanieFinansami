using System.Windows;
using ZarzadzanieFinansami;

namespace ZarzadzanieFinansami;

    public partial class CategoryAdd
    {
        public CategoryAdd()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DbUtility.SaveCategory(Nazwa.Text);
            Close();
        }
    }
