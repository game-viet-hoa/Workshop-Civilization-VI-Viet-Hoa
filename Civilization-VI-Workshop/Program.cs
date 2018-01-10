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

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Init();

        }

        void Init()
        {
            Console.WriteLine("Scan: " + pathScan);

            String[] lists = Directory.GetFiles(pathScan, "*.xml", SearchOption.AllDirectories);
            HashSet<String> tags = new HashSet<String>();
            HashSet<String> icons = new HashSet<String>();

            foreach (string item in lists)
            {
                string[] readText = File.ReadAllLines(item);

                
                foreach (string s in readText)
                {
                    MatchCollection mc = Regex.Matches(s, @"\<([A-Za-z0-9_]+)\>");
                    MatchCollection mc_icon = Regex.Matches(s, @"\[([A-Za-z0-9_]+)\]");
                    foreach (Match m in mc)
                    {
                        tags.Add(m.Value);
                    }

                    foreach (Match m in mc_icon)
                    {
                        icons.Add(m.Value);
                    }
                }

            }

            Console.WriteLine("\nList Tag:");
            foreach (string t in tags)
            {
                Console.WriteLine(t);
            }

            Console.WriteLine("\nList Icon:");
            int count = 1;
            foreach (string t in icons)
            {
                Console.Write(count + t);
                count++;
            }

            Console.ReadLine();
        }
    }
}
