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
        private String pathDestitional = @"C:\Users\XKVNN\Desktop\Civilization-VI-Viet-Hoa\Workshop";

        string[] tagsOld = new string[] { "<GameData>", "<BaseGameText>", "<Text>", "<EnglishText>",
            "<Gender>", "<FrontEndText>", "<Plurality>", "<LocalizedText>", "<Update>" };
        string[] tagsNeedReplace = new string[] { "BaseGameText>", "EnglishText>", "FrontEndText>" };
        string tagReplace = "LocalizedText>";

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
                Console.WriteLine("\n\nPress any key to exit...");
                Console.ReadKey();
                return;
            };

            if (CopyToDestitional())
            {
                CheckLocalizedText();
                CreateListsFileForMod();
            }

            Console.WriteLine("\n\nPress any key to exit...");
            Console.ReadKey();
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

        private Boolean CopyToDestitional()
        {
            Console.WriteLine("\n\nStart copy to " + pathDestitional);

            DirectoryInfo dirDestitional = new DirectoryInfo(pathDestitional);
            try
            {
                if (dirDestitional.Exists)
                    dirDestitional.Delete(true);

                Directory.CreateDirectory(pathDestitional);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't not delete or create directory " + pathDestitional);
                Console.WriteLine(ex);
                return false;
            }

            foreach (string item in listsFile)
            {
                string[] readText = File.ReadAllLines(item);
                
                for(int i = 0; i < readText.Length; i++)
                {
                    readText[i] = readText[i].Replace("<Row", "<Replace Language='en_US'").Replace("</Row>", "</Replace>");

                    foreach (string t in tagsNeedReplace)
                        readText[i] = readText[i].Replace(t, tagReplace);
                }

                string filePath = item.Replace(pathScan, pathDestitional);
                System.IO.FileInfo file = new System.IO.FileInfo(filePath);
                if (!Directory.Exists(file.DirectoryName))  // if it doesn't exist, create
                    Directory.CreateDirectory(file.DirectoryName);

                File.WriteAllLines(filePath, readText);
            }

            Console.WriteLine(" Done");

            return true;
        }

        private void CheckLocalizedText()
        {
            Console.WriteLine("\n\nStart check <LocalizedText>");

            foreach (string item in listsFile)
            {
                string filePath = item.Replace(pathScan, pathDestitional);
                string[] readText = File.ReadAllLines(filePath);

                int count = 0;
                foreach(string s in readText)
                {
                    if (s.IndexOf("<LocalizedText>") >= 0)
                        count++;
                }

                if(count > 1)
                {
                    Console.WriteLine("  " + filePath.Replace(pathDestitional,"") + ": " + count);
                    Boolean open = false;
                    Boolean close = false;

                    for (int i = 0; i < readText.Length; i++)
                    {
                        if (open && readText[i].IndexOf("<LocalizedText>") >= 0)
                            readText[i] = readText[i].Replace("<LocalizedText>", "");

                        if (readText[i].IndexOf("<LocalizedText>") >= 0)
                            open = true;
                    }

                    for (int i = readText.Length - 1; i >= 0; i--)
                    {
                        if (close && readText[i].IndexOf("</LocalizedText>") >= 0)
                            readText[i] = readText[i].Replace("</LocalizedText>", "");

                        if (readText[i].IndexOf("</LocalizedText>") >= 0)
                            close = true;
                    }

                    File.WriteAllLines(filePath, readText);
                }
                    
            }

            Console.WriteLine(" Done");
        }

        private void CreateListsFileForMod()
        {
            Console.WriteLine("\n\nList Files for ModInfo");

            foreach (string item in listsFile)
            {
                Console.WriteLine("<File>XKVNN" + item.Replace(pathScan,"").Replace(@"\", "/") + "</File>");
            }
        }
    }
}
