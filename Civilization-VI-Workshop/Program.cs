using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Civilization_VI_Workshop
{
    class Program
    {

        private String pathScan = @"D:\Dropbox\Translation\Civilization VI\Vietnamese";
        private String pathDestitional = @"D:\Dropbox\Translation\Civilization VI\Workshop\XKVNN";
        private String pathModInfo = @"D:\Dropbox\Translation\Civilization VI\Workshop\XKVNN - Vietnamese Localization.modinfo";

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
            String str1 = 
@"<?xml version=""1.0"" encoding=""utf-8""?>
<Mod id=""08305371-1edc-4898-a77f-49f69939278b"">

	<Properties>
        <Name>Civilization VI Việt Hóa</Name>
        <Teaser>Civilization VI Việt Hóa</Teaser>
        <Description>LOC_DESCRIPTION</Description>
        <Authors>Xuân Khánh - XKVNN</Authors>
		<EnabledByDefault>1</EnabledByDefault>
		<EnabledAtStartup>1</EnabledAtStartup>
		<AffectsSavedGames>0</AffectsSavedGames>
	</Properties>
	
	<LocalizedText>
		<Text id=""LOC_DESCRIPTION"">
			<en_US>Group Civilization VI Việt Hóa [COLOR_FLOAT_FOOD]https://fb.com/groups/Civilization6VietHoa[ENDCOLOR][NEWLINE][NEWLINE][ICON_GreatWriter][COLOR_RED]Việt hóa bởi:[ENDCOLOR][NEWLINE][ICON_Bullet][ICON_Capital]Xuân Khánh - XKVNN[NEWLINE][ICON_Bullet][ICON_Amenities]Group Civilization VI Việt Hóa[NEWLINE][NEWLINE][ICON_GOLD][COLOR_FLOAT_GOLD]Cảm ơn các nhà hảo tâm:[ENDCOLOR][NEWLINE][ICON_Bullet]Luxanna Crownguard[NEWLINE][ICON_Bullet]Huỳnh Thanh Tùng</en_US>
		</Text>
	</LocalizedText>
	
	<Files>";

            String str2 = 
@"	</Files>
	
	<FrontEndActions>
		<UpdateText id=""FrontEndUpdateLocalization"">";
            Console.WriteLine("\n\nList Files for ModInfo");

            String str3 = 
@"		</UpdateText>
    </FrontEndActions>
	
	<InGameActions>
		<UpdateText id=""UpdateLocalization"">";

            String str4 = 
@"		</UpdateText>
	</InGameActions>
</Mod>";

            String listOfFiles1 = "";
            String listOfFiles2 = "";
            foreach (string item in listsFile)
            {
                String file = "<File>XKVNN" + item.Replace(pathScan, "").Replace(@"\", "/") + "</File>";

                listOfFiles1 += "		" + file + "\r\n";
                listOfFiles2 += "			" + file + "\r\n";
                Console.WriteLine("<File>XKVNN" + item.Replace(pathScan,"").Replace(@"\", "/") + "</File>");
            }

            File.WriteAllText(pathModInfo, str1 + "\r\n" + listOfFiles1 + str2 + "\r\n" + listOfFiles2 + str3 + "\r\n" + listOfFiles2 + str4);
        }
    }
}
