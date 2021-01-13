using Microsoft.Win32;
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

namespace KrazzePropHuntLoader
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Title = "Krazze PropHunt Loader";
            (formsHost.Child as System.Windows.Forms.WebBrowser).Navigate("https://steamcommunity.com/sharedfiles/filedetails/?id=2210301113");
        }
        String presetLocation;
        String presetFileName;
        List<char> numbers = new List<char>(){'0','1','2','3','4','5','6','7','8','9'};
        List<long> listOfIDs= new List<long>();
        


        private void find_File(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                presetLocation = fileDialog.FileName;
                presetFileName = fileDialog.SafeFileName;
                locationBox.Text = presetLocation;
                Console.WriteLine(presetFileName);
            }
        }

        private void refresh(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.HtmlDocument document = browser.Document;
            System.Windows.Forms.HtmlElementCollection allDivs = document.GetElementsByTagName("a");
            List<String> addonIDs = new List<String>();
            foreach(System.Windows.Forms.HtmlElement element in allDivs)
            {
                String addonUrl = element.GetAttribute("href");
                if (addonUrl != null)
                {
                    if (addonUrl.Contains("?id") && element.Parent.GetAttribute("className") == "workshopItem")
                    {
                        int idIndex = addonUrl.IndexOf("?id");
                        addonIDs.Add(addonUrl.Substring(idIndex + 4));
                        addonIDs = addonIDs.Distinct().ToList();
                    }
                }
            }
            foreach (String id in addonIDs)
            {
                String temp = "";
                foreach (char c in id) 
                {
                    if (numbers.Contains(c))
                    {
                        temp += c;
                    }
                }
                if (temp != "2210301113") {
                    listOfIDs.Add(long.Parse(temp));
                }
            }
            listOfIDs.Sort();
            String text = System.IO.File.ReadAllText(presetLocation);
            text = text.Substring(0, text.Length - 1);
            if(text.Contains("Krazze PropHunt"))
            {
                String presetInText = text.Substring(text.IndexOf("\"Krazze PropHunt\""),text.IndexOf("\"Krazze PropHunt\",\"newAction\":\"disable\"}")-text.IndexOf("\"Krazze PropHunt\"")+40);
                String beforeText;
                String afterText;
                if (text.Substring(text.IndexOf(presetInText)-1, 1) != ",")
                {
                    beforeText = text.Substring(0, text.IndexOf(presetInText));
                    afterText = text.Substring(text.IndexOf("\"Krazze PropHunt\",\"newAction\":\"disable\"}") + 40, text.Length - beforeText.Length - presetInText.Length);
                }
                else
                {
                    beforeText = text.Substring(0, text.IndexOf(presetInText)-1);
                    afterText = text.Substring(text.IndexOf("\"Krazze PropHunt\",\"newAction\":\"disable\"}") + 40, text.Length - beforeText.Length - presetInText.Length-1);
                }
                text = beforeText + afterText;
            }
            text += ",\"Krazze PropHunt\":{\"disabled\":[],\"enabled\":[";
            for(int i = 0; i<listOfIDs.Count;i++)
            {
                if (i != listOfIDs.Count-1) 
                {
                    text += "\"" + listOfIDs[i] + "\",";
                }
                else
                {
                    text += "\"" + listOfIDs[i] + "\"";
                }
            }
            text += "],\"name\":\"Krazze PropHunt\",\"newAction\":\"disable\"}}";
            System.IO.File.WriteAllText(presetLocation, text);
            
        }
    }
}
