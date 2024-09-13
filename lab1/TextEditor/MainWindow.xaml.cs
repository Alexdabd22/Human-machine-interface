using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TextEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            _fontSize.ItemsSource = FontSizes;
            _fontSize.SelectedIndex = 0;
        }
        public double[] FontSizes
        {
            get
            {
                return new double[] { 3.0, 4.0, 5.0, 6.0, 6.5, 7.0, 7.5, 8.0, 8.5, 9.0,
                9.5, 10.0, 10.5, 11.0, 11.5, 12.0, 12.5, 13.0, 13.5, 14.0, 15.0, 16.0, 17.0, 18.0,
                19.0, 20.0, 22.0, 24.0, 26.0, 28.0, 30.0, 32.0, 34.0, 36.0, 38.0, 40.0, 44.0,
                48.0, 52.0, 56.0, 60.0, 64.0, 68.0, 72.0, 76.0, 80.0, 88.0, 96.0, 104.0, 112.0,
                120.0, 128.0, 136.0, 144.0 };
            }
        }
        // Обробник для кнопки "Create"
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            // Логіка для створення файлу
            MessageBox.Show("Створити новий файл");
        }

        // Обробник для кнопки "Open"
        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            // Логіка для відкриття файлу
            MessageBox.Show("Відкрити файл");
        }

        // Обробник для кнопки "Print"
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            // Логіка для друку файлу
            MessageBox.Show("Друк файлу");
        }

        // Обробник для кнопки "Close"
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            // Логіка для закриття файлу або програми
            this.Close();
        }
    }
}
