using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simon
{
    public class Settings
    {
        // the character representation of custom key bindings
        private char greenKey;
        public char GreenKey
        {
            get { return greenKey; }
            set { greenKey = value; }
        }

        private char redKey;
        public char RedKey
        {
            get { return redKey; }
            set { redKey = value; }
        }

        private char yellowKey;
        public char YellowKey
        {
            get { return yellowKey; }
            set { yellowKey = value; }
        }

        private char blueKey;
        public char BlueKey
        {
            get { return blueKey; }
            set { blueKey = value; }
        }

        // integer values of slider positions
        private int gameSlider;
        public int GameSlider
        {
            get { return gameSlider; }
            set { gameSlider = value; }
        }

        private int skillLevelSlider;
        public int SkillLevelSlider  
        {
            get { return skillLevelSlider; }
            set { skillLevelSlider = value; }
        }

        private int powerSlider;
        public int PowerSlider
        {
            get { return powerSlider; }
            set { powerSlider = value; }
        }

        private int volumeSlider;
        public int VolumeSlider
        {
            get { return volumeSlider; }
            set { volumeSlider = value; }
        }


        // background color related items
        private bool backGrndWood;
        public bool BackGrndWood
        {
            get { return backGrndWood; }
            set { backGrndWood = value; }
        }

        private bool backGrndGranite;
        public bool BackGrndGranite
        {
            get { return backGrndGranite; }
            set { backGrndGranite = value; }
        }

        private bool backGrndSpace;
        public bool BackGrndSpace
        {
            get { return backGrndSpace; }
            set { backGrndSpace = value; }
        }
        private bool backGrndRGB;

        public bool BackGrndRGB
        {
            get { return backGrndRGB; }
            set { backGrndRGB = value; }
        }

        private int rgbRedSlider;
        public int RgbRedSlider
        {
            get { return rgbRedSlider; }
            set { rgbRedSlider = value; }
        }

        private int rgbGreenSlider;
        public int RgbGreenSlider
        {
            get { return rgbGreenSlider; }
            set { rgbGreenSlider = value; }
        }

        private int rgbBlueSlider;
        public int RgbBlueSlider
        {
            get { return rgbBlueSlider; }
            set { rgbBlueSlider = value; }
        }


        // high scores & stats

        private int longestGame;
        public int LongestGame
        {
            get { return longestGame; }
            set { longestGame = value; }
        }





    }
}
