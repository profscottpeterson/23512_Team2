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

        // timer to repeat button sounds when button is held
        DispatcherTimer TimertoWaitForNextSequence = new DispatcherTimer();

        // round dispatcher timer
        DispatcherTimer roundTimer = new DispatcherTimer();

        // delay between buttons in playback
        DispatcherTimer delaytimer = new DispatcherTimer();

        // For if user takes too long to enter sequence
        DispatcherTimer timeoutTimer = new DispatcherTimer();


        public int currentRound = 0;
        public int roundIndex = 0;
        // List used to fill with random paths for random sequence of lights
        List<Path> randomPaths = new List<Path>();

        List<Path> randomPaths2 = new List<Path>();

        // List for the "Last" button
        List<Path> lastPaths = new List<Path>();

        // List for the "Longest" button
        List<Path> longestPaths = new List<Path>();

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

        // Game running variables
        public bool gameActive = false;
        int maxRounds;
        bool gameOver = false;
        bool outcome = false;

        // The game slider value will be assigned to this when the start button is pressed
        int gameType = -1;

        // need to prevent double selection on mouse unclick and mouse unhover
        bool btnHasBeenPressed = false;

        // this has to do with game 3.  In Game 3, if the wrong color is selected, the game continues with a new sequence (without that color) until only one color is left
        bool greenIsPlaying = true;
        bool redIsPlaying = true;
        bool yellowIsPlaying = true;
        bool blueIsPlaying = true;

        // window is loaded.  Create stuff
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TimertoLoopBtnSounds.Tick += new EventHandler(TimertoLoopBtnSounds_Tick);
            TimertoLoopBtnSounds.Interval = new TimeSpan(0, 0, 1);

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
            { background.Background = woodBrush; }
            if (MainSettings.BackGrndGranite)
            { background.Background = graniteBrush; }
            if (MainSettings.BackGrndSpace)
            { background.Background = spaceBrush; }
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
        private void ButtonDeactivated(System.Windows.Shapes.Path gameBoardButton, bool humanInput)
        {

                if (gameBoardButton.Name == "buttonGreen")
                {
                    SolidColorBrush b = new SolidColorBrush(greenUnPressed);
                    gameBoardButton.Fill = b;
                    TimertoLoopBtnSounds.Stop();

                    if (gameActive == true & humanInput == true)
                    {
                        CheckCorrectButton("buttonGreen");
                    }

                }

                if (gameBoardButton.Name == "buttonRed")
                {
                    SolidColorBrush b = new SolidColorBrush(redUnPressed);
                    gameBoardButton.Fill = b;
                    TimertoLoopBtnSounds.Stop();

                    if (gameActive == true & humanInput == true)
                    {
                        CheckCorrectButton("buttonRed");
                    }

                }

                if (gameBoardButton.Name == "buttonYellow")
                {
                    SolidColorBrush b = new SolidColorBrush(yellowUnPressed);
                    gameBoardButton.Fill = b;
                    TimertoLoopBtnSounds.Stop();

                    if (gameActive == true & humanInput == true)
                    {
                        CheckCorrectButton("buttonYellow");
                    }

                }

                if (gameBoardButton.Name == "buttonBlue")
                {
                    SolidColorBrush b = new SolidColorBrush(blueUnPressed);
                    gameBoardButton.Fill = b;
                    TimertoLoopBtnSounds.Stop();

                    if (gameActive == true & humanInput == true)
                    {
                        CheckCorrectButton("buttonBlue");
                    }

                }

                if (gameActive == true & humanInput == true)
                {
                    timeoutTimer.Stop();
                    timeoutTimer.Start();
                    
                }

                if (me != buttonSounds[4])
                {
                    me.Stop();
                }
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

            btnHasBeenPressed = true;
           
        }

        // event handler for releasing mouse button
        private void ButtonReleased(object sender, MouseButtonEventArgs e)
        {
            Path gameBoardButton = new Path();

            gameBoardButton = e.Source as Path;

            ButtonDeactivated(gameBoardButton,true);

            btnHasBeenPressed = false;
        }

        // event handler for hovering your mouse off of button
        private void ButtonReleased(object sender, MouseEventArgs e)
        {
            if (btnHasBeenPressed == true)
            {
                Path gameBoardButton = new Path();

                gameBoardButton = e.Source as Path;

                ButtonDeactivated(gameBoardButton, true);
                btnHasBeenPressed = false;
            }       
        }

        // this is the event handler for key down
        private void ButtonPressed(object sender, KeyEventArgs e)
        {
            SimonWindow.IsHitTestVisible = false;
            
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
                ButtonDeactivated(buttonGreen,true);
            }

            if (buttonHotKeys[1] == e.Key)
            {
                ButtonDeactivated(buttonRed,true);
            }

            if (buttonHotKeys[2] == e.Key)
            {
                ButtonDeactivated(buttonYellow,true);
            }

            if (buttonHotKeys[3] == e.Key)
            {
                ButtonDeactivated(buttonBlue,true);
            }

            SimonWindow.IsHitTestVisible = true;
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

        // Start the game - set up variables based on slider positions
        private void btnStartGame_Click(object sender, MouseButtonEventArgs e)
        {
            greenIsPlaying = true;
            blueIsPlaying = true;
            redIsPlaying = true;
            yellowIsPlaying = true;

            

            gameActive = true;
            btnStart.IsEnabled = false;

            randomPaths = new List<Path>();
            roundTimer = new DispatcherTimer();
            delaytimer = new DispatcherTimer();
            TimertoWaitForNextSequence = new DispatcherTimer();

            // time-out timer
            timeoutTimer = new DispatcherTimer();

            currentRound = 0;

            
            
            roundIndex = 0;
            timeoutTimer.Stop();
            roundTimer.Stop();
            delaytimer.Stop();
            TimertoWaitForNextSequence.Stop();

            // set the roundTimer's properties  - this is how long the cpu plays the button, 420 ms sequence length <=5, 320ms sequence length <=13, 220ms <=31
            roundTimer.Tick += new EventHandler(roundTimer_Tick);
            roundTimer.Interval = new TimeSpan(0, 0, 0, 0, 420);


            delaytimer.Tick += new EventHandler(delayTimer_Tick);
            delaytimer.Interval = new TimeSpan(0, 0, 0, 0, 50);

            // timer to end the game if user takes too long
            timeoutTimer.Tick += new EventHandler(timeoutTimer_Tick);
            timeoutTimer.Interval = new TimeSpan(0, 0, 0, 3, 0);

            TimertoWaitForNextSequence.Tick += new EventHandler(TimertoWaitForNextSequence_Tick);
            TimertoWaitForNextSequence.Interval = new TimeSpan(0, 0, 0, 0, 800);

            // get the max number of possible rounds, store it in the global variable 
            maxRounds = GetMaxRounds(MainSettings.SkillLevelSlider);

            gameType = MainSettings.GameSlider;

            if(gameType==2)
            { currentRound = 1;
              maxRounds=  GetMaxRounds(MainSettings.SkillLevelSlider)+1;
            }

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


            if(gameType==2)

            {
                randomPaths.RemoveRange(1, randomPaths.Count-1);
                Path p = new Path();
                p = null;
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
                m = 5;
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

        bool timeToAddButton;
        // code that runs during each round
        public void Round()
        {
            SimonWindow.IsHitTestVisible = false;
            timeToAddButton = false;
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
            if (randomPaths[roundIndex]!=null)
            {
                ButtonDeactivated(randomPaths[roundIndex], false);
                timeToAddButton = true;
                
            }
            timeoutTimer.Start();
            roundIndex++;

            //put delay timer here
            delaytimer.Start();
        }

        private void delayTimer_Tick(object sender, EventArgs e)
        {
            delaytimer.Stop();

            if (roundIndex != currentRound)
            {
                roundTimer.Start();
                try { 
               if (randomPaths[roundIndex] != null)
                {
                    ButtonActivated(randomPaths[roundIndex]);
                    timeToAddButton = true;
                }
                timeoutTimer.Stop();
                }
                catch
                {
                    roundIndex--;
                }
            }
            else
            {
                // reset the roundIndex
                roundIndex = 0;
                SimonWindow.IsHitTestVisible = true;
            }
        }

        // 3 second timer that checks for game over
        private void timeoutTimer_Tick(object sender, EventArgs e)
        {
            GameOver(false);
        }

        public void CheckCorrectButton(string b)
        {
            bool nextRoundBegun = false;

            if (gameType == 2)
            {
                Path path = new Path();
                path.Name = "Add";
                randomPaths[randomPaths.Count - 1] = path;
            }
            
                if (b == randomPaths[roundIndex].Name)
                {
                    roundIndex++;
                    
                }

            
            
                else
                {
                    int numColorsRemaining = 0;

                    if (gameType == 3)
                    {
                        if (b == "buttonGreen")
                        { greenIsPlaying = false; }
                        if (b == "buttonRed")
                        { redIsPlaying = false; }
                        if (b == "buttonYellow")
                        { yellowIsPlaying = false; }
                        if (b == "buttonBlue")
                        { blueIsPlaying = false; }


                        if (greenIsPlaying == true)
                        { numColorsRemaining++; }
                        if (redIsPlaying == true)
                        { numColorsRemaining++; }
                        if (yellowIsPlaying == true)
                        { numColorsRemaining++; }
                        if (blueIsPlaying == true)
                        { numColorsRemaining++; }

                        if (numColorsRemaining < 2)
                        { GameOver(false); }

                        Random rand = new Random();

                        randomPaths2.Clear();
                        //create list of random paths to be passed into button pressed/button released methods
                        while (randomPaths2.Count < maxRounds)
                        {
                            bool pathSuccess = false;

                            Path p = new Path();

                            int tempRandom = rand.Next(0, 4);

                            if (tempRandom == 0 & greenIsPlaying)
                            {
                                p = buttonGreen;
                                pathSuccess = true;
                            }

                            if (tempRandom == 1 & redIsPlaying)
                            {
                                p = buttonRed;
                                pathSuccess = true;
                            }

                            if (tempRandom == 2 & yellowIsPlaying)
                            {
                                p = buttonYellow;
                                pathSuccess = true;
                            }

                            if (tempRandom == 3 & blueIsPlaying)
                            {
                                p = buttonBlue;
                                pathSuccess = true;
                            }

                            if (pathSuccess == true)
                            {
                                randomPaths2.Add(p);
                            }
                        }

                        randomPaths = randomPaths2;

                        currentRound--;

                        TimertoWaitForNextSequence.Stop();

                        TimertoWaitForNextSequence.Start();

                        nextRoundBegun = true;
                    }
                    

                    if (gameType != 3)
                    {

                    if (b != randomPaths[roundIndex].Name  & randomPaths[roundIndex].Name!="Add")

                    {
                        // game over(false) means you lost, true means you won
                        GameOver(false);
                    }

                    if (randomPaths[randomPaths.Count - 1].Name == "Add")
                        {
                            Path p = new Path();
                            if (b == "buttonGreen")
                            { p = buttonGreen; }
                            if (b == "buttonRed")
                            { p = buttonRed; }
                            if (b == "buttonYellow")
                            { p = buttonYellow; }
                            if (b == "buttonBlue")
                            { p = buttonBlue; }

                            randomPaths.Insert(randomPaths.Count - 1, p);
                            roundIndex++;


                            randomPaths.RemoveAt(randomPaths.Count - 1);
                            p = null;
                            randomPaths.Add(p);                    
                        }
                    }
                }
            

            if (roundIndex == currentRound)
            {
                if (nextRoundBegun == false)
                {
                    


                    TimertoWaitForNextSequence.Start();



                }
            }
        }

        private void TimertoWaitForNextSequence_Tick(object sender, EventArgs e)
        {
            if (currentRound < maxRounds)
            {
                TimertoWaitForNextSequence.Stop();
                Round();
               
            }

            else
            {
                TimertoWaitForNextSequence.Stop();
                GameOver(true);
            }
           
        }


        // method that holds the game over code
        public void GameOver(bool won)
        {
            ButtonDeactivated(buttonRed,false);
            ButtonDeactivated(buttonGreen,false);
            ButtonDeactivated(buttonBlue,false);
            ButtonDeactivated(buttonYellow,false);

            gameActive = false;

            btnStart.IsEnabled = true;

            lastPaths.Clear();
            // add to list of last paths
            for (int i = 0; i < currentRound; i++)
            {

                if (randomPaths[i] != null & randomPaths[i].Name!="Add")
                {
                    lastPaths.Add(randomPaths[i]);
                }
            }

            if (lastPaths.Count > longestPaths.Count)
            {
                longestPaths.Clear();
                for (int i = 0; i < currentRound; i++)
                {
                    longestPaths.Add(randomPaths[i]);
                }

                if(longestPaths.Count>MainSettings.LongestGame)
                {
                    MainSettings.LongestGame = longestPaths.Count;
                }
            }

            SimonWindow.IsHitTestVisible = false;

            currentRound = 0;
            roundIndex = 0;

            // stop the timer
            timeoutTimer.Stop();
            timeoutTimer.Tick -= timeoutTimer_Tick;
            timeoutTimer = null;

            roundTimer.Stop();
            roundTimer.Tick -= roundTimer_Tick;
            roundTimer = null;

            delaytimer.Stop();
            delaytimer.Tick -= delayTimer_Tick;
            delaytimer = null;

            TimertoWaitForNextSequence.Stop();
            TimertoWaitForNextSequence.Tick -= TimertoWaitForNextSequence_Tick;
            TimertoWaitForNextSequence = null;

            roundTimer = new DispatcherTimer();
            delaytimer = new DispatcherTimer();

            // time-out timer
            timeoutTimer = new DispatcherTimer();
            TimertoWaitForNextSequence = new DispatcherTimer();

            // disable all buttons <-- Do we want to do something like this? 

            

            if (won == true)
            {
                MessageBox.Show("You've won!");
            }

            else
            {
                //game over sound.  Might need to be set on a timer. For this, I guess I could just recreate the audio file for the exact time length
                me = buttonSounds[4];
                me.Play();
                me.Position = new TimeSpan(0);
            }


            
            
           
            SimonWindow.IsHitTestVisible = true;

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

        DispatcherTimer roundTimerLast = new DispatcherTimer();
        DispatcherTimer delaytimerLast = new DispatcherTimer();

        private void btnLast_Click(object sender, MouseButtonEventArgs e)
        {
            if (lastPaths.Count > 0)
            {

                SimonWindow.IsHitTestVisible = false;

                roundTimerLast = new DispatcherTimer();
                delaytimerLast = new DispatcherTimer();

                currentRound = 0;
                roundIndex = 0;

                roundTimerLast.Stop();
                delaytimerLast.Stop();


                // set the roundTimer's properties  - this is how long the cpu plays the button, 420 ms sequence length <=5, 320ms sequence length <=13, 220ms <=31
                roundTimerLast.Tick += new EventHandler(roundTimerLast_Tick);
                roundTimerLast.Interval = new TimeSpan(0, 0, 0, 0, 420);


                delaytimerLast.Tick += new EventHandler(delayTimerLast_Tick);
                delaytimerLast.Interval = new TimeSpan(0, 0, 0, 0, 50);


                // get the max number of possible rounds, store it in the global variable 
                maxRounds = lastPaths.Count;

                // while it is not game over, run the round code
                RoundLast();
            }
        }

        // will return the maximum number of rounds


        // code that runs during each round
        public void RoundLast()
        {
            // resets round index
            roundIndex = 0;

            // increment currentRound
            currentRound++;

            roundTimerLast.Start();
            ButtonActivated(lastPaths[roundIndex]);
        }

        private void roundTimerLast_Tick(object sender, EventArgs e)
        {
            roundTimerLast.Stop();
            ButtonDeactivated(lastPaths[roundIndex],false);
            roundIndex++;

            //put delay timer here
            delaytimerLast.Start();
        }

        private void delayTimerLast_Tick(object sender, EventArgs e)
        {
            delaytimerLast.Stop();

            roundTimerLast.Start();

            if (roundIndex < maxRounds)
            {
                ButtonActivated(lastPaths[roundIndex]);
            }
            else
            {
                roundTimerLast.Stop();
                roundTimerLast.Tick -= roundTimerLast_Tick;
                roundTimerLast = null;

                delaytimerLast.Stop();
                delaytimerLast.Tick -= delayTimerLast_Tick;
                delaytimerLast = null;
                SimonWindow.IsHitTestVisible = true;
            }

        }



        DispatcherTimer roundTimerLongest = new DispatcherTimer();
        DispatcherTimer delaytimerLongest = new DispatcherTimer();
        private void btnLongest_Click(object sender, MouseButtonEventArgs e)
        {

            if (longestPaths.Count > 0)
            {
                SimonWindow.IsHitTestVisible = false;

                roundTimerLongest = new DispatcherTimer();
                delaytimerLongest = new DispatcherTimer();

                currentRound = 0;
                roundIndex = 0;

                roundTimerLongest.Stop();
                delaytimerLongest.Stop();


                // set the roundTimer's properties  - this is how long the cpu plays the button, 420 ms sequence length <=5, 320ms sequence length <=13, 220ms <=31
                roundTimerLongest.Tick += new EventHandler(roundTimerLongest_Tick);
                roundTimerLongest.Interval = new TimeSpan(0, 0, 0, 0, 420);


                delaytimerLongest.Tick += new EventHandler(delayTimerLongest_Tick);
                delaytimerLongest.Interval = new TimeSpan(0, 0, 0, 0, 50);


                // get the max number of possible rounds, store it in the global variable 
                maxRounds = longestPaths.Count;

                // while it is not game over, run the round code
                RoundLongest();
            }
        }



        // code that runs during each round
        public void RoundLongest()
        {
            // resets round index
            roundIndex = 0;

            // increment currentRound
            currentRound++;

            roundTimerLongest.Start();
            ButtonActivated(longestPaths[roundIndex]);
        }

        private void roundTimerLongest_Tick(object sender, EventArgs e)
        {
            roundTimerLongest.Stop();
            ButtonDeactivated(longestPaths[roundIndex],false);
            roundIndex++;

            //put delay timer here
            delaytimerLongest.Start();
        }

        private void delayTimerLongest_Tick(object sender, EventArgs e)
        {
            delaytimerLongest.Stop();

            roundTimerLongest.Start();

            if (roundIndex < maxRounds)
            {
                ButtonActivated(longestPaths[roundIndex]);
            }
            else
            {
                roundTimerLongest.Stop();
                roundTimerLongest.Tick -= roundTimerLongest_Tick;
                roundTimerLongest = null;

                delaytimerLongest.Stop();
                delaytimerLongest.Tick -= delayTimerLongest_Tick;
                delaytimerLongest = null;
                SimonWindow.IsHitTestVisible = true;
            }
        }

        private void ProgramClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
        }
    }
}