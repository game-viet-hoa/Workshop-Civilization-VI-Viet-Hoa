using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Civilization_VI_Workshop
{
    class Program
    {

        private String pathScan = @"C:\Users\XKVNN\Desktop\Civilization-VI-Viet-Hoa\Vietnamese";
        string[] tagsOld = new string[] { "<GameData>", "<BaseGameText>", "<Text>", "<EnglishText>",
            "<Gender>", "<FrontEndText>", "<Plurality>", "<LocalizedText>", "<Update>" };

        HashSet<String> tags = new HashSet<String>();
        HashSet<String> icons = new HashSet<String>();
        String[] listsFile;

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Init();
        }

        void Init()
        {
            Console.WriteLine("Scan: " + pathScan);

            ScanTags();

            PrintIcon();
            PrintTags();

            if (HaveNewTags())
            {
                Console.ReadLine();
                return;
            };

            Console.ReadLine();
        }

        private void ScanTags()
        {
            listsFile = Directory.GetFiles(pathScan, "*.xml", SearchOption.AllDirectories);

            foreach (string item in listsFile)
            {
                string[] readText = File.ReadAllLines(item);

                foreach (string s in readText)
                {
                    MatchCollection mc = Regex.Matches(s, @"\<([A-Za-z0-9_]+)\>");
                    MatchCollection mc_icon = Regex.Matches(s, @"\[([A-Za-z0-9_]+)\]");
                    foreach (Match m in mc)
                        tags.Add(m.Value);

                    foreach (Match m in mc_icon)
                        icons.Add(m.Value);
                }
            }
        }

        private void PrintTags()
        {
            Console.WriteLine("\n\nList Tag:");
            foreach (string t in tags)
                Console.WriteLine(t);
        }

        private void PrintIcon()
        {
            Console.WriteLine("\n\nList Icon:");
            int count = 1;
            foreach (string t in icons)
            {
                Console.Write(count + t);
                count++;
            }
        }

        private Boolean HaveNewTags()
        {
            Boolean HaveNew = false;

            Console.WriteLine("\n\nScanning New Tags:");
            int count = 0;

            string old = "";
            foreach (string t in tagsOld)
                old += t;

            foreach (string t in tags)
            {
                if (old.IndexOf(t) < 0)
                {
                    HaveNew = true;
                    Console.WriteLine("  " + t);
                }
            }
                
            if(HaveNew)
                Console.WriteLine(" Have new Tags, need change code");
            else
                Console.WriteLine(" Haven't new Tags");

            return HaveNew;
        }
    }
}
