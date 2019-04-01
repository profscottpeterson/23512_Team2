using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Simon
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        MainWindow mw = new MainWindow();

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

            

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ImageBrush myBrush = new ImageBrush();
            Image image = new Image();
            image.Source = new BitmapImage(
                new Uri(
                   "pack://application:,,,/Images/woodGrain.jpg"));
            myBrush.ImageSource = image.Source;

            (mw as MainWindow).background.Background = myBrush;

           

            ((MainWindow)Application.Current.MainWindow).background.Background = myBrush;
        }
    }
}
