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

        //instantiate the settings class
        public Settings MainSettings = new Settings();

        // create media element.  We will set items from a list of media elements to this variable throughout the program
        MediaElement me = new MediaElement();

        // list of media elements
        List<MediaElement> buttonSounds = new List<MediaElement>();

        // timer to repeat button sounds when button is held
        DispatcherTimer TimertoLoopBtnSounds = new DispatcherTimer();

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

        // background images
        public ImageBrush woodBrush = new ImageBrush();
        public ImageBrush graniteBrush = new ImageBrush();
        public ImageBrush spaceBrush = new ImageBrush();
        public SolidColorBrush rgbBrush = new SolidColorBrush();

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

            me.LoadedBehavior = MediaState.Manual;
            me.UnloadedBehavior = MediaState.Manual;

            // load game settings
            if (File.Exists("SaveFile.txt"))
            {
                try
                {
                    LoadSettings();
                }
                catch
                {
                    // if the save file fails to be read, the user probably edited it.
                    // their file is corrupted and must be reset to defaults
                    MessageBox.Show("Your save file has been corrupted. \nDefault Settings will be loaded.");
                    DefaultSettings();
                }
            }
            else
            {
                MessageBox.Show("Your save file, \"SaveFile.txt\" is not in the \nsame folder as the game's .exe file. \n\nDefault Settings will be loaded.");
                // set defaults for all fields in class
                DefaultSettings();

                SaveSettings();
            }


            //Now must set up all items based on what is loaded into class settings

            //set slider positions graphically
            gameSlider.Value = Convert.ToDouble(MainSettings.GameSlider);
            skillLevelSlider.Value = Convert.ToDouble(MainSettings.SkillLevelSlider);
            pwrSlider.Value = Convert.ToDouble(MainSettings.PowerSlider);

            //set key bindings from class variables
            setkeyBindingsFromOtherWindow();

            // set background.  Unfortunately this exists in both windows slightly differently
            if (MainSettings.BackGrndWood)
            { background.Background=woodBrush; }
            if (MainSettings.BackGrndGranite)
            { background.Background=graniteBrush; }
            if (MainSettings.BackGrndSpace)
            { background.Background=spaceBrush; }
            if (MainSettings.BackGrndRGB)
            {
                Color RGBBackground = Color.FromRgb(Convert.ToByte(MainSettings.RgbRedSlider), Convert.ToByte(MainSettings.RgbGreenSlider), Convert.ToByte(MainSettings.RgbBlueSlider));
                rgbBrush.Color = RGBBackground;
                background.Background = rgbBrush;
            }

          
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

                if (timeoutTimer.IsEnabled)
                {
                    CheckCorrectButton("buttonGreen");
                }
            }

            if (gameBoardButton.Name == "buttonRed")
            {
                RadialGradientBrush r = new RadialGradientBrush(redPressed1, redPressed2);
                gameBoardButton.Fill = r;
                me = buttonSounds[1];

                if (timeoutTimer.IsEnabled)
                {
                    CheckCorrectButton("buttonRed");
                }
            }

            if (gameBoardButton.Name == "buttonYellow")
            {
                RadialGradientBrush r = new RadialGradientBrush(yellowPressed1, yellowPressed2);
                gameBoardButton.Fill = r;
                me = buttonSounds[2];

                if (timeoutTimer.IsEnabled)
                {
                    CheckCorrectButton("buttonYellow");
                }
            }

            if (gameBoardButton.Name == "buttonBlue")
            {
                RadialGradientBrush r = new RadialGradientBrush(bluePressed1, bluePressed2);
                gameBoardButton.Fill = r;
                me = buttonSounds[3];

                if (timeoutTimer.IsEnabled)
                {
                    CheckCorrectButton("buttonBlue");
                }
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
                if (buttonHotKeys[0] == e.Key)
                {
                    ButtonActivated(buttonGreen);
                }

                if (buttonHotKeys[1] == e.Key)
                {
                    ButtonActivated(buttonRed);
                }

                if (buttonHotKeys[2] == e.Key)
                {
                    ButtonActivated(buttonYellow);
                }

                if (buttonHotKeys[3] == e.Key)
                {
                    ButtonActivated(buttonBlue);
                }
            }
        }
   
        // This is the event handler for key up
        private void ButtonReleased(object sender, KeyEventArgs e)
        {         
            if (buttonHotKeys[0] == e.Key)
            {
                ButtonDeactivated(buttonGreen);
            }

            if (buttonHotKeys[1] == e.Key)
            {
                ButtonDeactivated(buttonRed);
            }

            if (buttonHotKeys[2] == e.Key)
            {
                ButtonDeactivated(buttonYellow);
            }

            if (buttonHotKeys[3] == e.Key)
            {
                ButtonDeactivated(buttonBlue);
            }
        }

        private void GameSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MainSettings.GameSlider = Convert.ToInt32(gameSlider.Value);
            
        }

        private void SkillLevelSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MainSettings.SkillLevelSlider = Convert.ToInt32(skillLevelSlider.Value);
        }

        private void PwrSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MainSettings.PowerSlider = Convert.ToInt32(pwrSlider.Value);
          
            // game turned on
            if (MainSettings.PowerSlider == 1)
            {
                // game is turned on
                // make lights flash?
                // Start inactivity countdown - game flashes  if left on for over 45s
            }
        }

        // ------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------
        // GAME RUNNING CODE BELOW THIS POINT
        // ------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------

        // round dispatcher timer
        DispatcherTimer roundTimer = new DispatcherTimer();
        DispatcherTimer delaytimer = new DispatcherTimer();

        // time-out timer
        DispatcherTimer timeoutTimer = new DispatcherTimer();

        // Start the game - set up variables based on slider positions
        private void btnStartGame_Click (object sender, MouseButtonEventArgs e)
        {
            // set the roundTimer's properties
            roundTimer.Tick += new EventHandler(roundTimer_Tick);
            roundTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);

            delaytimer.Tick += new EventHandler(delayTimer_Tick);
            delaytimer.Interval= new TimeSpan(0, 0, 0, 0, 250);

            timeoutTimer.Tick += new EventHandler(timeoutTimer_Tick);
            timeoutTimer.Interval = new TimeSpan(0, 0, 0, 3, 0);


            // set the slider position
            MainSettings.SkillLevelSlider = Convert.ToInt32(skillLevelSlider.Value);

            // get the max number of possible rounds, store it in the global variable 
            maxRounds = GetMaxRounds(MainSettings.SkillLevelSlider);

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
            roundIndex = 0;

            // increment currentRound
            currentRound++;
           
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
            else
            {
                // start time-out timer
                timeoutTimer.Start();

                // reset the roundIndex
                roundIndex = 0; 
            }
        }

        // 3 second timer that checks for game over
        private void timeoutTimer_Tick(object sender, EventArgs e)
        {
            timeoutTimer.Stop();

            GameOver(); 
        }

        public void CheckCorrectButton (string b)
        {
            if (b == randomPaths[roundIndex].Name)
            {
                timeoutTimer.Stop();
                timeoutTimer.Start();
                roundIndex++; 
            }
            else
            {
                GameOver(); 
            }

            if (roundIndex == currentRound)
            {
                timeoutTimer.Stop();
                
                Round(); 
            }
        }

        // method that holds the game over code
        public void GameOver()
        {
            // stop the timer
            timeoutTimer.Stop();

            // disable all buttons <-- Do we want to do something like this? 


            //game over sound.  Might need to be set on a timer. For this, I guess I could just recreate the audio file for the exact time length
            me = buttonSounds[4];
            me.Play();
            me.Position = new TimeSpan(0);



            // game over
            MessageBox.Show("Game Over!"); 
        }

        // ------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------
        // GAME RUNNING CODE ABOVE THIS POINT
        // ------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------



        //  open the settings window when clicked
        private void settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.settingsWindowSettings = MainSettings;
            settingsWindow.ShowDialog();
        }

        public void setkeyBindingsFromOtherWindow()
        {
            buttonHotKeys[0] = (Key)Enum.Parse(typeof(Key), char.ToString(MainSettings.GreenKey));
            buttonHotKeys[1] = (Key)Enum.Parse(typeof(Key), char.ToString(MainSettings.RedKey));
            buttonHotKeys[2] = (Key)Enum.Parse(typeof(Key), char.ToString(MainSettings.YellowKey));
            buttonHotKeys[3] = (Key)Enum.Parse(typeof(Key), char.ToString(MainSettings.BlueKey));
        }

        public void setVolumeFromOtherWindow()
        {
            foreach (MediaElement m in buttonSounds)
            {
                m.Volume = MainSettings.VolumeSlider * .33;
            }
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

        private void DefaultSettings()
        {
            MainSettings.GreenKey = 'Q';
            MainSettings.RedKey = 'W';
            MainSettings.YellowKey = 'A';
            MainSettings.BlueKey = 'S';
            MainSettings.GameSlider = 1;
            MainSettings.SkillLevelSlider = 1;
            MainSettings.PowerSlider = 0;
            MainSettings.VolumeSlider = 3;
            MainSettings.BackGrndWood = true;
            MainSettings.BackGrndGranite = false;
            MainSettings.BackGrndSpace = false;
            MainSettings.BackGrndRGB = false;
            MainSettings.RgbRedSlider = 192;
            MainSettings.RgbGreenSlider = 192;
            MainSettings.RgbBlueSlider = 192;
            MainSettings.LongestGame = 0;
        }

        public void SaveSettings()
        {
            // stream reader for reading/writing info
            StreamWriter sw = new StreamWriter("SaveFile.txt");

            sw.WriteLine(MainSettings.GreenKey.ToString());
            sw.WriteLine(MainSettings.RedKey.ToString());
            sw.WriteLine(MainSettings.YellowKey.ToString());
            sw.WriteLine(MainSettings.BlueKey.ToString());
            sw.WriteLine(MainSettings.GameSlider.ToString());
            sw.WriteLine(MainSettings.SkillLevelSlider.ToString());
            sw.WriteLine(MainSettings.PowerSlider.ToString());
            sw.WriteLine(MainSettings.VolumeSlider.ToString());
            sw.WriteLine(MainSettings.BackGrndWood.ToString());
            sw.WriteLine(MainSettings.BackGrndGranite.ToString());
            sw.WriteLine(MainSettings.BackGrndSpace.ToString());
            sw.WriteLine(MainSettings.BackGrndRGB.ToString());
            sw.WriteLine(MainSettings.RgbRedSlider.ToString());
            sw.WriteLine(MainSettings.RgbGreenSlider.ToString());
            sw.WriteLine(MainSettings.RgbBlueSlider.ToString());
            sw.WriteLine(MainSettings.LongestGame.ToString());
            
            sw.Flush();
            sw.Close();

        }

        private void LoadSettings()
        {

            // stream reader, read save file
            StreamReader sr = new StreamReader("SaveFile.txt");
            string temp = "";

            //load hot keys
            temp = sr.ReadLine();
            MainSettings.GreenKey = Convert.ToChar(temp);
            temp = sr.ReadLine();
            MainSettings.RedKey = Convert.ToChar(temp);
            temp = sr.ReadLine();
            MainSettings.YellowKey = Convert.ToChar(temp);
            temp = sr.ReadLine();
            MainSettings.BlueKey = Convert.ToChar(temp);

            //load sliders
            temp = sr.ReadLine();
            MainSettings.GameSlider = Convert.ToInt32(temp);
            temp = sr.ReadLine();
            MainSettings.SkillLevelSlider = Convert.ToInt32(temp);
            temp = sr.ReadLine();
            MainSettings.PowerSlider = Convert.ToInt32(temp);
            temp = sr.ReadLine();
            MainSettings.VolumeSlider = Convert.ToInt32(temp);

            //load background selections
            temp = sr.ReadLine();
            MainSettings.BackGrndWood = Convert.ToBoolean(temp);
            temp = sr.ReadLine();
            MainSettings.BackGrndGranite = Convert.ToBoolean(temp);
            temp = sr.ReadLine();
            MainSettings.BackGrndSpace = Convert.ToBoolean(temp);
            temp = sr.ReadLine();
            MainSettings.BackGrndRGB = Convert.ToBoolean(temp);

            //load rgb slider locations
            temp = sr.ReadLine();
            MainSettings.RgbRedSlider = Convert.ToInt32(temp);
            temp = sr.ReadLine();
            MainSettings.RgbGreenSlider = Convert.ToInt32(temp);
            temp = sr.ReadLine();
            MainSettings.RgbBlueSlider = Convert.ToInt32(temp);

            //load game record settings
            temp = sr.ReadLine();
            MainSettings.LongestGame = Convert.ToInt32(temp);

            sr.Close();
        }

        private void SetupTimers()
        {
            TimertoLoopBtnSounds.Tick += new EventHandler(TimertoLoopBtnSounds_Tick);
            TimertoLoopBtnSounds.Interval = new TimeSpan(0, 0, 1);
        }

        public void SetupMediaElements()
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


        private void saveMenu_click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        private void closeMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnResizeWindow_Click(object sender, RoutedEventArgs e)
        {
            SimonMainWindow.Height = 870;
            SimonMainWindow.Width = 870;
        }


        private void btnLast_Click(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void btnLongest_Click(object sender, MouseButtonEventArgs e)
        {

        }

      
    }
}
