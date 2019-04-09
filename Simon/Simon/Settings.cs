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
    }
}
