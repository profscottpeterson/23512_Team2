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
using System.IO;
using Path = System.Windows.Shapes.Path;

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

        // timer to repeat button sounds when button is held
        DispatcherTimer TimertoLoopBtnSounds = new DispatcherTimer();

        // timer to change focus away from text field and set custom keys
        DispatcherTimer timerToSetCustomKeys = new DispatcherTimer();

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

        //slider values need to be global.  Their positions are accessed when the start button is pressed
        int gameSliderPosition;
        int skillLevelSliderPosition;
        int powerSliderPosition;

        // List used to fill with random paths for random sequence of lights
        List<Path> randomPaths = new List<Path>();

        // Game running variables
        int currentRound;
        int roundIndex;
        int maxRounds;
        bool gameOver = false;
        bool outcome = false;

        // window is loaded.  Create stuff
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // call setup timers method
            SetupTimers();

            // call setup media elements method
            SetupMediaElements();

            // create list of keys
            for (int i = 0; i < 4; i++)
            {
                Key k = new Key();
                buttonHotKeys.Add(k);
            }

            buttonHotKeys[0] = Key.Q;
            buttonHotKeys[1] = Key.W;
            buttonHotKeys[2] = Key.A;
            buttonHotKeys[3] = Key.S;

            // set positions of all controls
            VolSlider.Value = 2; // 66% sound
            me.Volume = VolSlider.Value * .33;

            me.LoadedBehavior = MediaState.Manual;
            me.UnloadedBehavior = MediaState.Manual;

            btnConfirmKeys.Height = 0;
            btnConfirmKeys.Width = 0;


            if (File.Exists("saveFile.txt"))
            {
                // stream reader, read save file
                StreamReader sr = new StreamReader("saveFile.txt");

                string temp = "";
                temp = sr.ReadLine();
                gameSlider.Value = Int32.Parse(temp);
                temp = sr.ReadLine();
                skillLevelSlider.Value = Int32.Parse(temp);
                temp = sr.ReadLine();
                pwrSlider.Value = Int32.Parse(temp);

                sr.Close();
            }

            // slider positions
            gameSliderPosition = Convert.ToInt32(gameSlider.Value);
            skillLevelSliderPosition = Convert.ToInt32(skillLevelSlider.Value);
            powerSliderPosition = Convert.ToInt32(pwrSlider.Value);
        }

        // button is pressed 
        private void ButtonActivated(System.Windows.Shapes.Path gameBoardButton)
        {
            // start timer
           
            TimertoLoopBtnSounds.Start();

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
        private void ButtonDeactivated(System.Windows.Shapes.Path gameBoardButton)
        {
            if (gameBoardButton.Name == "buttonGreen")
            {
                SolidColorBrush b = new SolidColorBrush(greenUnPressed);
                gameBoardButton.Fill = b;
                TimertoLoopBtnSounds.Stop();
            }

            if (gameBoardButton.Name == "buttonRed")
            {
                SolidColorBrush b = new SolidColorBrush(redUnPressed);
                gameBoardButton.Fill = b;
                TimertoLoopBtnSounds.Stop();
            }

            if (gameBoardButton.Name == "buttonYellow")
            {
                SolidColorBrush b = new SolidColorBrush(yellowUnPressed);
                gameBoardButton.Fill = b;
                TimertoLoopBtnSounds.Stop();
            }

            if (gameBoardButton.Name == "buttonBlue")
            {
                SolidColorBrush b = new SolidColorBrush(blueUnPressed);
                gameBoardButton.Fill = b;
                TimertoLoopBtnSounds.Stop();
            }
            me.Stop();
        }

        // When the button sound loop timer finishes, set the media's position and replay
        private void TimertoLoopBtnSounds_Tick(object sender, EventArgs e)
        {
            me.Position = new TimeSpan(0, 0, 0, 0, 500);
            me.Play();
        }

        // This sets the volume for all button sounds
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

            ButtonActivated(gameBoardButton);
        }

        // event handler for releasing mouse button
        private void ButtonReleased(object sender, MouseButtonEventArgs e)
        {
            Path gameBoardButton = new Path();

            gameBoardButton = e.Source as Path;

            ButtonDeactivated(gameBoardButton);
        }

        // event handler for hovering your mouse off of button
        private void ButtonReleased(object sender, MouseEventArgs e)
        {
            Path gameBoardButton = new Path();

            gameBoardButton = e.Source as Path;

            ButtonDeactivated(gameBoardButton);
        }

        // this is the event handler for key down
        private void ButtonPressed(object sender, KeyEventArgs e)
        {
            // if this if statment isn't here, the audio won't repeat
            if (!e.IsRepeat)
            {
                if (buttonHotKeys[0] == e.Key && !txtSetGreenKey.IsFocused)
                {
                    ButtonActivated(buttonGreen);
                }

                if (buttonHotKeys[1] == e.Key && !txtSetRedKey.IsFocused)
                {
                    ButtonActivated(buttonRed);
                }

                if (buttonHotKeys[2] == e.Key && !txtSetYellowKey.IsFocused)
                {
                    ButtonActivated(buttonYellow);
                }

                if (buttonHotKeys[3] == e.Key && !txtSetBlueKey.IsFocused)
                {
                    ButtonActivated(buttonBlue);
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
                    ButtonDeactivated(buttonGreen);
                }
            }

            if (!txtSetRedKey.IsFocused)
            {
                if (buttonHotKeys[1] == e.Key && !txtSetRedKey.IsFocused)
                {
                    ButtonDeactivated(buttonRed);
                }
            }

            if (!txtSetYellowKey.IsFocused)
            {
                if (buttonHotKeys[2] == e.Key && !txtSetYellowKey.IsFocused)
                {
                    ButtonDeactivated(buttonYellow);
                }
            }

            if (!txtSetBlueKey.IsFocused)
            {
                if (buttonHotKeys[3] == e.Key && !txtSetBlueKey.IsFocused)
                {
                    ButtonDeactivated(buttonBlue);
                }
            }
        }
             
        // this allows the user to set the hot key.
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

        private void GameSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            gameSliderPosition = Convert.ToInt32(gameSlider.Value);
            
            //debug
            //debugGameSlider.Content = "Game Slider Pos: " + gameSliderPosition.ToString();

           // debugGameSlider.Content = "asdf";
        }

        private void SkillLevelSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            skillLevelSliderPosition = Convert.ToInt32(skillLevelSlider.Value);

            //debug
            // debugSkillSlider.Content = "Skill Slider Pos: " + skillLevelSliderPosition.ToString();
        }

        private void PwrSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            powerSliderPosition = Convert.ToInt32(pwrSlider.Value);
          
            // game turned on
            if (powerSliderPosition == 1)
            {
                // game is turned on
                // make lights flash?
                // Start inactivity countdown - game flashes  if left on for over 45s
            }
        }

        // round dispatcher timer
        DispatcherTimer roundTimer = new DispatcherTimer();
        DispatcherTimer delaytimer = new DispatcherTimer();
        
        // Start the game - set up variables based on slider positions
        private void btnStartGame(object sender, MouseButtonEventArgs e)
        {
            // set the roundTimer's properties
            roundTimer.Tick += new EventHandler(roundTimer_Tick);
            roundTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);

            delaytimer.Tick += new EventHandler(delayTimer_Tick);
            delaytimer.Interval= new TimeSpan(0, 0, 0, 0, 250);



            // set the slider position
            skillLevelSliderPosition = Convert.ToInt32(skillLevelSlider.Value);

            // get the max number of possible rounds, store it in the global variable 
            maxRounds = GetMaxRounds(skillLevelSliderPosition);

            // generate random numbers for order --- I think we should move this into the game class later on, under the CreateRandomList() method
            Random rand = new Random();

            //create list of random paths to be passed into button pressed/button released methods
            for (int i = 0; i < maxRounds; i++)
            {
                Path p = new Path();

                int tempRandom = rand.Next(0, 4);

                if (tempRandom == 0)
                {
                    p = buttonGreen;
                }

                if (tempRandom == 1)
                {
                    p = buttonRed;
                }

                if (tempRandom == 2)
                {
                    p = buttonYellow;
                }

                if (tempRandom == 3)
                {
                    p = buttonBlue;
                }

                randomPaths.Add(p);
            }

            // while it is not game over, run the round code
             Round();
        }

        

        // will return the maximum number of rounds
        public int GetMaxRounds(int lvl)
        {
            int m;

            if (lvl == 1)
            {
                m = 8;
            }
            else if (lvl == 2)
            {
                m = 14;
            }
            else if (lvl == 3)
            {
                m = 20;
            }
            else
            {
                m = 31;
            }

            return m;
        }

        // code that runs during each round
        public void Round()
        {
            // resets round index
            roundIndex=0;

            // increment currentRound
            currentRound=31;
           
            roundTimer.Start();
            ButtonActivated(randomPaths[roundIndex]);
            
        }

        private void roundTimer_Tick(object sender, EventArgs e)
        {
            roundTimer.Stop();
            ButtonDeactivated(randomPaths[roundIndex]);
            roundIndex++;

            //put delay timer here
            delaytimer.Start();
    }

        private void delayTimer_Tick(object sender, EventArgs e)
        {
            delaytimer.Stop();

            if (roundIndex!=currentRound)
            {
                roundTimer.Start();
                ButtonActivated(randomPaths[roundIndex]);
            }
        }


        // ------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------
        // GAME RUNNING CODE ABOVE THIS POINT
        // ------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------

        // save button uses stream writer to write to a text file
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // stream reader for reading/writing info
            StreamWriter sw = new StreamWriter("saveFile.txt");

            sw.WriteLine(gameSliderPosition.ToString());
            sw.WriteLine(skillLevelSliderPosition.ToString());
            sw.WriteLine(powerSliderPosition.ToString());
            
            sw.Flush();
        }



        // used to test sequence.  This will all get deleted
        int testsequence = -1;

        DispatcherTimer testTime = new DispatcherTimer();

        private void BtnTestSequence_Click(object sender, RoutedEventArgs e)
        {
            testsequence++;

            testTime.Tick += new EventHandler(testTime_Tick);
            testTime.Interval = new TimeSpan(0, 0, 0, 0, 250);
            testTime.Start();

            ButtonActivated(randomPaths[testsequence]);            
        }

        private void testTime_Tick(object sender, EventArgs e)
        {
            ButtonDeactivated(randomPaths[testsequence]);

            testTime.Stop();
        }

        

        //  open the settings window when clicked
        private void settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            
            
            
            settingsWindow.ShowDialog();
            
        }

        

        // open the info window when clicked
        private void info_Click(object sender, RoutedEventArgs e)
        {
            InfoWindow infoWindow = new InfoWindow();
            infoWindow.ShowDialog();
        }







        // ------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------
        // BELOW THIS PIONT USED TO SET UP VARIABLES - WILL BE CALLED ON FORM LOAD
        // ------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------



        private void SetupTimers()
        {

            TimertoLoopBtnSounds.Tick += new EventHandler(TimertoLoopBtnSounds_Tick);
            TimertoLoopBtnSounds.Interval = new TimeSpan(0, 0, 1);


            timerToSetCustomKeys.Tick += new EventHandler(TimerToSetCustomKeys_Tick);
            timerToSetCustomKeys.Interval = new TimeSpan(0, 0, 0, 0, 500);

        }


        private void SetupMediaElements()
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


        }

    }
}
