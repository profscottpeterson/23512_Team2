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
using System.Windows.Threading;

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

        public Settings settingsWindowSettings = new Settings();

        // timer to change focus away from text field and set custom keys
        DispatcherTimer timerToSetCustomKeys = new DispatcherTimer();

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

            // make button "invisible"
            btnConfirmKeys.Height = 0;
            btnConfirmKeys.Width = 0;

            // key binding timer
            timerToSetCustomKeys.Tick += new EventHandler(TimerToSetCustomKeys_Tick);
            timerToSetCustomKeys.Interval = new TimeSpan(0, 0, 0, 0, 100);
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


        // this allows the user to set the hot key.
        private void SetKey(object sender, KeyEventArgs e)
        {
            try { 
            if (txtSetGreenKey.IsFocused)
            {
                settingsWindowSettings.GreenKey = Convert.ToChar(e.Key.ToString());
                txtSetGreenKey.Text = e.Key.ToString();
            }

            if (txtSetRedKey.IsFocused)
            {
                settingsWindowSettings.RedKey = Convert.ToChar(e.Key.ToString());
                txtSetRedKey.Text = e.Key.ToString();
            }

            if (txtSetYellowKey.IsFocused)
            {
                settingsWindowSettings.YellowKey = Convert.ToChar(e.Key.ToString());
                txtSetYellowKey.Text = e.Key.ToString();
            }

            if (txtSetBlueKey.IsFocused)
            {
                settingsWindowSettings.BlueKey = Convert.ToChar(e.Key.ToString());
                txtSetBlueKey.Text = e.Key.ToString();
            }
                 }
            catch
            {
                MessageBox.Show("Your key bindings must be letters A-Z, numbers 0-9");
            }
            Keyboard.ClearFocus();

            timerToSetCustomKeys.Start();
        }

        private void TimerToSetCustomKeys_Tick(object sender, EventArgs e)
        {
            timerToSetCustomKeys.Stop();

            Keyboard.Focus(btnConfirmKeys);
        }

        private void BtnConfirmKeys_Click(object sender, RoutedEventArgs e)
        {

        }

        private void VolSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            settingsWindowSettings.VolumeSlider = Convert.ToInt32(VolSlider.Value); 
        }

        private void SettingsClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {           
            ((MainWindow)Application.Current.MainWindow).setkeyBindingsFromOtherWindow();
        }
    }
}
