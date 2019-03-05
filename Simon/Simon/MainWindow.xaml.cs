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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Media;
using System.Windows.Threading;

namespace Simon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        // create media element.  We will set items from a list of media elements to this variable throughout the program
        MediaElement me = new MediaElement();

        // list of media elements
        List<MediaElement> buttonSounds = new List<MediaElement>();

        // timer to repeat events
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        // this list of keys.  User can set hot keys to press simon buttons instead of using mouse
        List<Key> buttonHotKeys = new List<Key>();

        // create colors and add to lists - to show buttons lighting and not lighting
        // unpressed colors
        Color greenUnPressed = Color.FromRgb(0, 128, 0);
        Color redUnPressed = Color.FromRgb(128, 0, 0);
        Color yellowUnPressed = Color.FromRgb(128, 128, 0);
        Color blueUnPressed = Color.FromRgb(0, 0, 128);

        // pressed color for gradient (1st argument)
        Color greenPressed1 = Color.FromRgb(200, 255, 200);
        Color redPressed1 = Color.FromRgb(255, 200, 200);
        Color yellowPressed1 = Color.FromRgb(255, 255, 200);
        Color bluePressed1 = Color.FromRgb(200, 200, 255);

        // pressed color for gradient (2nd argument)
        Color greenPressed2 = Color.FromRgb(0, 255, 0);
        Color redPressed2 = Color.FromRgb(255, 0, 0);
        Color yellowPressed2 = Color.FromRgb(255, 255, 0);
        Color bluePressed2 = Color.FromRgb(0, 0, 255);

        // window is loaded.  Create stuff
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // get sounds from relative filepath and add to list of mediaElement objects
            MediaElement sound = new MediaElement();
            sound.Source = new Uri(@"../../Sounds/209hz_green_g#3.wav", UriKind.Relative);
            sound.LoadedBehavior = MediaState.Manual;
            sound.UnloadedBehavior = MediaState.Manual;
            buttonSounds.Add(sound);

            sound = new MediaElement();
            sound.Source = new Uri(@"../../Sounds/252hz_red_b3.wav", UriKind.Relative);
            sound.LoadedBehavior = MediaState.Manual;
            sound.UnloadedBehavior = MediaState.Manual;
            buttonSounds.Add(sound);

            sound = new MediaElement();
            sound.Source = new Uri(@"../../Sounds/310hz_yellow_d#4.wav", UriKind.Relative);
            sound.LoadedBehavior = MediaState.Manual;
            sound.UnloadedBehavior = MediaState.Manual;
            buttonSounds.Add(sound);

            sound = new MediaElement();
            sound.Source = new Uri(@"../../Sounds/415hz_blue_g#4.wav", UriKind.Relative);
            sound.LoadedBehavior = MediaState.Manual;
            sound.UnloadedBehavior = MediaState.Manual;
            buttonSounds.Add(sound);

            sound = new MediaElement();
            sound.Source = new Uri(@"../../Sounds/42hz_losing_tone.wav", UriKind.Relative);
            sound.LoadedBehavior = MediaState.Manual;
            sound.UnloadedBehavior = MediaState.Manual;
            buttonSounds.Add(sound);

            // create list of keys
            for (int i = 0; i < 4; i++)
            {
                Key k = new Key();
                buttonHotKeys.Add(k);
            }

            // set positions of all controls
            VolSlider.Value = 2; // 66% sound
            me.Volume = VolSlider.Value * .33;

            me.LoadedBehavior = MediaState.Manual;
            me.UnloadedBehavior = MediaState.Manual;

            btnConfirmKeys.Height = 0;
            btnConfirmKeys.Width = 0;

        }

        // button is pressed
        private void ButtonPressed(Path gameBoardButton)
        {
            // start timer
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            if (gameBoardButton.Name == "buttonGreen")
            {
                RadialGradientBrush r = new RadialGradientBrush(greenPressed1, greenPressed2);
                gameBoardButton.Fill = r;
                me = buttonSounds[0];
            }

            if (gameBoardButton.Name == "buttonRed")
            {
                RadialGradientBrush r = new RadialGradientBrush(redPressed1, redPressed2);
                gameBoardButton.Fill = r;
                me = buttonSounds[1];
            }

            if (gameBoardButton.Name == "buttonYellow")
            {
                RadialGradientBrush r = new RadialGradientBrush(yellowPressed1, yellowPressed2);
                gameBoardButton.Fill = r;
                me = buttonSounds[2];
            }

            if (gameBoardButton.Name == "buttonBlue")
            {
                RadialGradientBrush r = new RadialGradientBrush(bluePressed1, bluePressed2);
                gameBoardButton.Fill = r;
                me = buttonSounds[3];
            }
            me.Play();
        }

        // button is released - called in event handlers mouse hovers off, mouse unclicked, key press up
        private void ButtonReleased(Path gameBoardButton)
        {
            if (gameBoardButton.Name == "buttonGreen")
            {
                SolidColorBrush b = new SolidColorBrush(greenUnPressed);
                gameBoardButton.Fill = b;
                dispatcherTimer.Stop();
            }

            if (gameBoardButton.Name == "buttonRed")
            {
                SolidColorBrush b = new SolidColorBrush(redUnPressed);
                gameBoardButton.Fill = b;
                dispatcherTimer.Stop();
            }

            if (gameBoardButton.Name == "buttonYellow")
            {
                SolidColorBrush b = new SolidColorBrush(yellowUnPressed);
                gameBoardButton.Fill = b;
                dispatcherTimer.Stop();
            }

            if (gameBoardButton.Name == "buttonBlue")
            {
                SolidColorBrush b = new SolidColorBrush(blueUnPressed);
                gameBoardButton.Fill = b;
                dispatcherTimer.Stop();
            }
            me.Stop();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            me.Position = new TimeSpan(0, 0, 0, 0, 500);
            me.Play();
        }

        private void VolSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            foreach (MediaElement m in buttonSounds)
            {
                m.Volume = VolSlider.Value * .33;
            }
        }

        // event handler for pressing button (clicking mouse)
        private void ButtonPressed(object sender, MouseButtonEventArgs e)
        {
            Path gameBoardButton = new Path();

            gameBoardButton = e.Source as Path;

            ButtonPressed(gameBoardButton);
        }

        // event handler for releasing mouse button
        private void ButtonReleased(object sender, MouseButtonEventArgs e)
        {
            Path gameBoardButton = new Path();

            gameBoardButton = e.Source as Path;

            ButtonReleased(gameBoardButton);
        }

        // event handler for hovering your mouse off of button
        private void ButtonReleased(object sender, MouseEventArgs e)
        {
            Path gameBoardButton = new Path();

            gameBoardButton = e.Source as Path;

            ButtonReleased(gameBoardButton);
        }

        // this is the event handler for key down
        private void ButtonPressed(object sender, KeyEventArgs e)
        {
            // if this if statment isn't here, the audio won't repeat
            if (!e.IsRepeat)
            {
                if (buttonHotKeys[0] == e.Key && !txtSetGreenKey.IsFocused)
                {
                    ButtonPressed(buttonGreen);
                }

                if (buttonHotKeys[1] == e.Key && !txtSetRedKey.IsFocused)
                {
                    ButtonPressed(buttonRed);
                }

                if (buttonHotKeys[2] == e.Key && !txtSetYellowKey.IsFocused)
                {
                    ButtonPressed(buttonYellow);
                }

                if (buttonHotKeys[3] == e.Key && !txtSetBlueKey.IsFocused)
                {
                    ButtonPressed(buttonBlue);
                }
            }
        }
   
        // This is the event handler for key up
        private void ButtonReleased(object sender, KeyEventArgs e)
        {
            if (!txtSetGreenKey.IsFocused)
            {
                if (buttonHotKeys[0] == e.Key && !txtSetGreenKey.IsFocused)
                {
                    ButtonReleased(buttonGreen);
                }
            }

            if (!txtSetRedKey.IsFocused)
            {
                if (buttonHotKeys[1] == e.Key && !txtSetRedKey.IsFocused)
                {
                    ButtonReleased(buttonRed);
                }
            }

            if (!txtSetYellowKey.IsFocused)
            {
                if (buttonHotKeys[2] == e.Key && !txtSetYellowKey.IsFocused)
                {
                    ButtonReleased(buttonYellow);
                }
            }

            if (!txtSetBlueKey.IsFocused)
            {
                if (buttonHotKeys[3] == e.Key && !txtSetBlueKey.IsFocused)
                {
                    ButtonReleased(buttonBlue);
                }
            }
        }
         
        DispatcherTimer setKeys = new DispatcherTimer();
        
        private void SetKey(object sender, KeyEventArgs e)
        {

            if (txtSetGreenKey.IsFocused)
            { 
                buttonHotKeys[0] = e.Key;
                txtSetGreenKey.Text = e.Key.ToString();
            }

            if (txtSetRedKey.IsFocused)
            {
                buttonHotKeys[1] = e.Key;
                txtSetRedKey.Text = e.Key.ToString();      
            }

            if (txtSetYellowKey.IsFocused)
            {
                buttonHotKeys[2] = e.Key;
                txtSetYellowKey.Text = e.Key.ToString();             
            }

            if (txtSetBlueKey.IsFocused)
            {
                buttonHotKeys[3] = e.Key;
                txtSetBlueKey.Text = e.Key.ToString();          
            }

            Keyboard.ClearFocus();

            setKeys.Tick += new EventHandler(SetKeys);
            setKeys.Interval = new TimeSpan(0, 0, 0, 0, 500);
            setKeys.Start();
        }

        private void SetKeys(object sender, EventArgs e)
        {
            setKeys.Stop();

            Keyboard.Focus(btnConfirmKeys);        
        }

        private void BtnConfirmKeys_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
