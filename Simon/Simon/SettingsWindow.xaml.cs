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


        public Settings settingsWindowSettings = new Settings();

        // need this to prevent slider/radio button events from occuring while form is loading
        private bool formIsLoaded = false;

        // timer to change focus away from text field and set custom keys
        DispatcherTimer timerToSetCustomKeys = new DispatcherTimer();

        private void settingsLoaded(object sender, RoutedEventArgs e)
        {
            // set up pictures

            // make button "invisible"
            btnConfirmKeys.Height = 0;
            btnConfirmKeys.Width = 0;

            // Put current key bindings in text boxes
            txtSetGreenKey.Text = settingsWindowSettings.GreenKey.ToString();
            txtSetRedKey.Text = settingsWindowSettings.RedKey.ToString();
            txtSetYellowKey.Text = settingsWindowSettings.YellowKey.ToString();
            txtSetBlueKey.Text = settingsWindowSettings.BlueKey.ToString();

            // key binding timer
            timerToSetCustomKeys.Tick += new EventHandler(TimerToSetCustomKeys_Tick);
            timerToSetCustomKeys.Interval = new TimeSpan(0, 0, 0, 0, 100);

            
            //set up items on page based on settings class

            // rgb slider values
            slColorR.Value = Convert.ToDouble(settingsWindowSettings.RgbRedSlider);
            slColorG.Value = Convert.ToDouble(settingsWindowSettings.RgbGreenSlider);
            slColorB.Value = Convert.ToDouble(settingsWindowSettings.RgbBlueSlider);

            // radio buttons
            if(settingsWindowSettings.BackGrndWood)
            { rdoWood.IsChecked = true; }
            if(settingsWindowSettings.BackGrndGranite)
            { rdoGranite.IsChecked = true; }
            if(settingsWindowSettings.BackGrndSpace)
            { rdoSpace.IsChecked = true; }
            if(settingsWindowSettings.BackGrndRGB)
            { rdoRGB.IsChecked = true; }


            //volume slider
            VolSlider.Value = Convert.ToDouble(settingsWindowSettings.VolumeSlider);

            formIsLoaded = true;

            // longest game
            lngstGmLbl.Content = settingsWindowSettings.LongestGame;
        }


        private void rdoBtn_Checked(object sender, RoutedEventArgs e)
        {
           // if (formIsLoaded == true)
            {

                if (rdoWood.IsChecked == true)
                {
                    ((MainWindow)Application.Current.MainWindow).background.Background = ((MainWindow)Application.Current.MainWindow).woodBrush;
                    settingsWindowSettings.BackGrndWood = true;
                    settingsWindowSettings.BackGrndGranite = false;
                    settingsWindowSettings.BackGrndSpace = false;
                    settingsWindowSettings.BackGrndRGB = false;
                }

                if (rdoGranite.IsChecked == true)
                {
                    ((MainWindow)Application.Current.MainWindow).background.Background = ((MainWindow)Application.Current.MainWindow).graniteBrush;
                    settingsWindowSettings.BackGrndWood = false;
                    settingsWindowSettings.BackGrndGranite = true;
                    settingsWindowSettings.BackGrndSpace = false;
                    settingsWindowSettings.BackGrndRGB = false;
                }

                if (rdoSpace.IsChecked == true)
                {
                    ((MainWindow)Application.Current.MainWindow).background.Background = ((MainWindow)Application.Current.MainWindow).spaceBrush;
                    settingsWindowSettings.BackGrndWood = false;
                    settingsWindowSettings.BackGrndGranite = false;
                    settingsWindowSettings.BackGrndSpace = true;
                    settingsWindowSettings.BackGrndRGB = false;
                }

                if (rdoRGB.IsChecked == false)
                {
                    rgbPanel.IsEnabled = false;
                }

                if (rdoRGB.IsChecked == true)
                {
                    rgbPanel.IsEnabled = true;

                    SetRGBBackground();

                    settingsWindowSettings.BackGrndWood = false;
                    settingsWindowSettings.BackGrndGranite = false;
                    settingsWindowSettings.BackGrndSpace = false;
                    settingsWindowSettings.BackGrndRGB = true;
                }
            }
        }

        private void ColorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (formIsLoaded == true)
            {
                SetRGBBackground();            
            }
        }

        private void SetRGBBackground()
        {
            Color RGBBackground = Color.FromRgb(Convert.ToByte(slColorR.Value), Convert.ToByte(slColorG.Value), Convert.ToByte(slColorB.Value));
            ((MainWindow)Application.Current.MainWindow).rgbBrush.Color = RGBBackground;
            ((MainWindow)Application.Current.MainWindow).background.Background = ((MainWindow)Application.Current.MainWindow).rgbBrush;

            settingsWindowSettings.RgbRedSlider = Convert.ToInt32(slColorR.Value);
            settingsWindowSettings.RgbGreenSlider = Convert.ToInt32(slColorG.Value);
            settingsWindowSettings.RgbBlueSlider = Convert.ToInt32(slColorB.Value);
        }

        // this allows the user to set the hot key.
        private void SetKey(object sender, KeyEventArgs e)
        {

            try
            { 
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
                MessageBox.Show("Your key bindings must be letters A-Z");

                // It is possible to allow key bindings not A-Z, see example below
                // would have to reformat class from char to string
                // buttonHotKeys[0] = (Key)Enum.Parse(typeof(Key), "NumPad8");

            }

            Keyboard.ClearFocus();

            timerToSetCustomKeys.Start();
        }

        private void TimerToSetCustomKeys_Tick(object sender, EventArgs e)
        {
            timerToSetCustomKeys.Stop();

            Keyboard.Focus(btnConfirmKeys);
        }


        private void VolSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            settingsWindowSettings.VolumeSlider = Convert.ToInt32(VolSlider.Value);

            ((MainWindow)Application.Current.MainWindow).setVolumeFromOtherWindow();
        }

        private void SettingsClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {           
            ((MainWindow)Application.Current.MainWindow).setkeyBindingsFromOtherWindow();
            ((MainWindow)Application.Current.MainWindow).SaveSettings();
            ((MainWindow)Application.Current.MainWindow).MainSettings = settingsWindowSettings;
        }

        private void BtnConfirmKeys_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
