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

        ImageBrush woodBrush = new ImageBrush();
        ImageBrush graniteBrush = new ImageBrush();
        ImageBrush spaceBrush = new ImageBrush();
        SolidColorBrush rgbBrush = new SolidColorBrush();

        private void settingsLoaded(object sender, RoutedEventArgs e)
        {
            // set up pictures
            Image woodImage = new Image();
            woodImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/woodGrain.jpg"));
            woodBrush.ImageSource = woodImage.Source;
        
            Image graniteImage = new Image();
            graniteImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/granite.jpg"));
            graniteBrush.ImageSource = graniteImage.Source;

            Image spaceImage = new Image();
            spaceImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/OuterSpace.jpg"));
            spaceBrush.ImageSource = spaceImage.Source;
        }


        private void rdoBtn_Checked(object sender, RoutedEventArgs e)
        {
            if(rdoWood.IsChecked==true)
            {
                ((MainWindow)Application.Current.MainWindow).background.Background = woodBrush;
            }

            if(rdoGranite.IsChecked==true)
            {
                ((MainWindow)Application.Current.MainWindow).background.Background = graniteBrush;
            }

            if(rdoSpace.IsChecked==true)
            {
                ((MainWindow)Application.Current.MainWindow).background.Background = spaceBrush;
            }

            if(rdoRGB.IsChecked==false)
            {
                rgbPanel.IsEnabled = false;
            }

            if(rdoRGB.IsChecked==true)
            {
                rgbPanel.IsEnabled = true;
            }
        }

        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Color RGBBackground = Color.FromRgb(Convert.ToByte(slColorR.Value), Convert.ToByte(slColorG.Value), Convert.ToByte(slColorB.Value));
            rgbBrush.Color = RGBBackground;
            ((MainWindow)Application.Current.MainWindow).background.Background = rgbBrush;
        }
    }
}
