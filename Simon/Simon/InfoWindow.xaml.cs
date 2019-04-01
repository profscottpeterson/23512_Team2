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
    /// Interaction logic for InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        public InfoWindow()
        {
            InitializeComponent();
        }

        private void info_loaded(object sender, RoutedEventArgs e)
        {
            // string to store our names
            List<string> devNames = new List<string>();

            // need random int to pick names from list
            Random rand = new Random();

            // int to pick from list of names as list shrinks
            int numNamesRemain = 4;

            // add names
            devNames.Add("Andrew Weekes");
            devNames.Add("Menka Ghai");
            devNames.Add("Zach Lacroix");
            devNames.Add("Jacob Vander Velden");

            // string to concat for placing in text box
            string textforTextbox = "";

            // start the string
            textforTextbox += "Developers: \n";

            // list of names for randomized list
            List<string> devNamesRandomized = new List<string>();

            // loop through dev names list, add randomly to other list, then remove from previous list
            for (int i=0;i<4;i++)
            {
                int j = rand.Next(0, numNamesRemain);

                devNamesRandomized.Add(devNames[j]);

                devNames.RemoveAt(j);

                numNamesRemain--;
            }

            // concat for each member in randomized list
            foreach (var name in devNamesRandomized)
            {
                textforTextbox += name + "\n";
            }

            // add to text block
            textBlockDevNames.Text = textforTextbox;
        }


        // closes the window to get back to the game
        private void InfoWindowClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
