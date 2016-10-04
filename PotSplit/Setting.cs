using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace PotSplit
{
    class Setting
    {
        //************************************************************
        //*                   All setting variables                  *
        //************************************************************
        public static bool TTS_Enable = true;
        public static bool TTS_Min_Enable = false;
        public static bool TTS_Max_Enable = false;
        public static bool TTS_Content_Enable = false;
        public static bool TTS_Effect_Enable = false;
        public static bool TTS_Filter = false;
        public static string TTS_Content = "{메세지}";
        public static string TTS_Voice = "TTS_Voice_Google";
        public static string TTS_Effect_List = "슈퍼마리오 코인.wav/일반 코인 소리.wav/소닉 코인.wav";
        public static int TTS_Effect = 0;
        public static int TTS_Min = 0;
        public static int TTS_Max = 0;
        
        public static bool FDM_Enable = false;
        public static bool FDM_Min_Enable = false;
        public static bool FDM_Max_Enable = false;
        public static bool FDM_Filter = false;
        public static string FDM_Content = "{메세지}";
        public static string FDM_Intro = "bounceIn";
        public static string FDM_Effect = "tada";
        public static string FDM_Outtro = "bounceOut";
        public static int FDM_Min = 0;
        public static int FDM_Max = 0;

        public static bool Youtube_Enable = true;
        public static bool Youtube_Next_Enable = true;
        public static bool Youtube_Min_Enable = false;
        public static bool Youtube_Max_Enable = false;
        public static bool Youtube_Filter = false;
        public static int Youtube_Min = 0;
        public static int Youtube_Max = 0;

        public static int Youtube_X = 0;
        public static int Youtube_Y = 320;
        public static int Youtube_Width = 640;
        public static int Youtube_Height = 400;
        public static int FDM_X = 0;
        public static int FDM_Y = 0;
        public static int FDM_Width = 1280;
        public static int FDM_Height = 300;

        public static List<string> MessageFilter = new List<string>();
        public static List<string> PotsuFilter = new List<string>();
        public static List<string> YoutubeFilter = new List<string>();

        //************************************************************
        //*                Setting Load/Save Functions               *
        //************************************************************

        public static void SaveSetting()
        {
            string path = Directory.GetCurrentDirectory() + "\\Setting.ini";
            List<string> SettingInfo = new List<string>();
            SettingInfo.Add("TTS_Enable : " + TTS_Enable.ToString());
            SettingInfo.Add("TTS_Min_Enable : " + TTS_Min_Enable.ToString());
            SettingInfo.Add("TTS_Max_Enable : " + TTS_Max_Enable.ToString());
            SettingInfo.Add("TTS_Content_Enable : " + TTS_Content_Enable.ToString());
            SettingInfo.Add("TTS_Effect_Enable : " + TTS_Effect_Enable.ToString());
            SettingInfo.Add("TTS_Filter : " + TTS_Filter.ToString());
            SettingInfo.Add("TTS_Content : " + TTS_Content);
            SettingInfo.Add("TTS_Voice : " + TTS_Voice);
            SettingInfo.Add("TTS_Effect_List : " + TTS_Effect_List);
            SettingInfo.Add("TTS_Effect : " + TTS_Effect.ToString());
            SettingInfo.Add("TTS_Min : " + TTS_Min.ToString());
            SettingInfo.Add("TTS_Max : " + TTS_Max.ToString());

            SettingInfo.Add("FDM_Enable : " + FDM_Enable.ToString());
            SettingInfo.Add("FDM_Min_Enable : " + FDM_Min_Enable.ToString());
            SettingInfo.Add("FDM_Max_Enable : " + FDM_Max_Enable.ToString());
            SettingInfo.Add("FDM_Filter : " + FDM_Filter.ToString());
            SettingInfo.Add("FDM_Content : " + FDM_Content);
            SettingInfo.Add("FDM_Intro : " + FDM_Intro);
            SettingInfo.Add("FDM_Effect : " + FDM_Effect);
            SettingInfo.Add("FDM_Outtro : " + FDM_Outtro);
            SettingInfo.Add("FDM_Min : " + FDM_Min.ToString());
            SettingInfo.Add("FDM_Max : " + FDM_Max.ToString());

            SettingInfo.Add("Youtube_Enable : " + Youtube_Enable.ToString());
            SettingInfo.Add("Youtube_Next_Enable : " + Youtube_Next_Enable.ToString());
            SettingInfo.Add("Youtube_Min_Enable : " + Youtube_Min_Enable.ToString());
            SettingInfo.Add("Youtube_Max_Enable : " + Youtube_Max_Enable.ToString());
            SettingInfo.Add("Youtube_Filter : " + Youtube_Filter.ToString());
            SettingInfo.Add("Youtube_Min : " + Youtube_Min.ToString());
            SettingInfo.Add("Youtube_Max : " + Youtube_Max.ToString());

            SettingInfo.Add("Youtube_X : " + Youtube_X.ToString());
            SettingInfo.Add("Youtube_Y : " + Youtube_Y.ToString());
            SettingInfo.Add("Youtube_Width : " + Youtube_Width.ToString());
            SettingInfo.Add("Youtube_Height : " + Youtube_Height.ToString());
            SettingInfo.Add("FDM_X : " + FDM_X.ToString());
            SettingInfo.Add("FDM_Y : " + FDM_Y.ToString());
            SettingInfo.Add("FDM_Width : " + FDM_Width.ToString());
            SettingInfo.Add("FDM_Height : " + FDM_Height.ToString());

            File.WriteAllLines(path, SettingInfo);
        }
        public static bool LoadSetting(string path)
        {
            string[] StrValue = null;
            List<string> SettingInfo = File.ReadAllLines(path).ToList();
            if(CheckSettingFile(SettingInfo))
            {
                foreach (string SettingLine in SettingInfo)
                {
                    StrValue = Regex.Split(SettingLine, " : ");
                    if (StrValue[0] == "TTS_Enable") TTS_Enable = bool.Parse(StrValue[1]);
                    else if (StrValue[0] == "TTS_Min_Enable") TTS_Min_Enable = bool.Parse(StrValue[1]);
                    else if (StrValue[0] == "TTS_Max_Enable") TTS_Max_Enable = bool.Parse(StrValue[1]);
                    else if (StrValue[0] == "TTS_Content_Enable") TTS_Content_Enable = bool.Parse(StrValue[1]);
                    else if (StrValue[0] == "TTS_Effect_Enable") TTS_Effect_Enable = bool.Parse(StrValue[1]);
                    else if (StrValue[0] == "TTS_Filter") TTS_Filter = bool.Parse(StrValue[1]);
                    else if (StrValue[0] == "TTS_Content") TTS_Content = StrValue[1];
                    else if (StrValue[0] == "TTS_Voice") TTS_Voice = StrValue[1];
                    else if (StrValue[0] == "TTS_Effect_List") TTS_Effect_List = StrValue[1]; 
                    else if (StrValue[0] == "TTS_Effect") TTS_Effect = int.Parse(StrValue[1]);
                    else if (StrValue[0] == "TTS_Min") TTS_Min = int.Parse(StrValue[1]);
                    else if (StrValue[0] == "TTS_Max") TTS_Max = int.Parse(StrValue[1]);

                    if (StrValue[0] == "FDM_Enable") FDM_Enable = bool.Parse(StrValue[1]);
                    else if (StrValue[0] == "FDM_Min_Enable") FDM_Min_Enable = bool.Parse(StrValue[1]);
                    else if (StrValue[0] == "FDM_Max_Enable") FDM_Max_Enable = bool.Parse(StrValue[1]);
                    else if (StrValue[0] == "FDM_Filter") FDM_Filter = bool.Parse(StrValue[1]);
                    else if (StrValue[0] == "FDM_Content") FDM_Content = StrValue[1];
                    else if (StrValue[0] == "FDM_Intro") FDM_Intro = StrValue[1];
                    else if (StrValue[0] == "FDM_Effect") FDM_Effect = StrValue[1];
                    else if (StrValue[0] == "FDM_Outtro") FDM_Outtro = StrValue[1];
                    else if (StrValue[0] == "FDM_Min") FDM_Min = int.Parse(StrValue[1]);
                    else if (StrValue[0] == "FDM_Max") FDM_Max = int.Parse(StrValue[1]);

                    if (StrValue[0] == "Youtube_Enable") Youtube_Enable = bool.Parse(StrValue[1]);
                    else if (StrValue[0] == "Youtube_Next_Enable") Youtube_Next_Enable = bool.Parse(StrValue[1]);
                    else if (StrValue[0] == "Youtube_Min_Enable") Youtube_Min_Enable = bool.Parse(StrValue[1]);
                    else if (StrValue[0] == "Youtube_Max_Enable") Youtube_Max_Enable = bool.Parse(StrValue[1]);
                    else if (StrValue[0] == "Youtube_Filter") Youtube_Filter = bool.Parse(StrValue[1]);
                    else if (StrValue[0] == "Youtube_Min") Youtube_Min = int.Parse(StrValue[1]);
                    else if (StrValue[0] == "Youtube_Max") Youtube_Max = int.Parse(StrValue[1]);

                    if (StrValue[0] == "Youtube_X") Youtube_X = int.Parse(StrValue[1]);
                    else if (StrValue[0] == "Youtube_Y") Youtube_Y = int.Parse(StrValue[1]);
                    else if (StrValue[0] == "Youtube_Width") Youtube_Width = int.Parse(StrValue[1]);
                    else if (StrValue[0] == "Youtube_Height") Youtube_Height = int.Parse(StrValue[1]);
                    else if (StrValue[0] == "FDM_X") FDM_X = int.Parse(StrValue[1]);
                    else if (StrValue[0] == "FDM_Y") FDM_Y = int.Parse(StrValue[1]);
                    else if (StrValue[0] == "FDM_Width") FDM_Width = int.Parse(StrValue[1]);
                    else if (StrValue[0] == "FDM_Height") FDM_Height = int.Parse(StrValue[1]);
                }
            }
            else MessageBox.Show("설정파일에 문제가 있습니다!");
            return true;
        }
        private static bool CheckSettingFile(List<string> SettingContent)
        {
            bool IsOkay = false;
            bool BoolValue = false;
            string StrValue = "";
            int IntValue = 0;

            foreach(string SettingLine in SettingContent)
            {
                #region TTSSettingCheck
                if (SettingLine.Contains("TTS_Enable : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = bool.TryParse(StrValue, out BoolValue);
                }
                else if (SettingLine.Contains("TTS_Min_Enable : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = bool.TryParse(StrValue, out BoolValue);
                }
                else if (SettingLine.Contains("TTS_Max_Enable : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = bool.TryParse(StrValue, out BoolValue);
                }
                else if (SettingLine.Contains("TTS_Content_Enable : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = bool.TryParse(StrValue, out BoolValue); 
                }
                else if (SettingLine.Contains("TTS_Effect_Enable : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = bool.TryParse(StrValue, out BoolValue);
                }
                else if (SettingLine.Contains("TTS_Filter : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = bool.TryParse(StrValue, out BoolValue);
                }
                else if (SettingLine.Contains("TTS_Content : "))
                {
                    IsOkay = true;
                }
                else if (SettingLine.Contains("TTS_Effect_List : "))
                {
                    IsOkay = true;
                }
                else if (SettingLine.Contains("TTS_Voice : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    if (StrValue == "TTS_Voice_Heami" || 
                        StrValue == "TTS_Voice_Google" || 
                        StrValue == "TTS_Voice_Randome")
                        IsOkay = true; 
                }
                else if (SettingLine.Contains("TTS_Effect : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = int.TryParse(StrValue, out IntValue);
                }
                else if (SettingLine.Contains("TTS_Min : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = int.TryParse(StrValue, out IntValue);
                }
                else if (SettingLine.Contains("TTS_Max : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = int.TryParse(StrValue, out IntValue);
                }
                #endregion
                #region FloatDonationMessageSettingCheck
                if (SettingLine.Contains("FDM_Enable : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = bool.TryParse(StrValue, out BoolValue);
                }
                else if (SettingLine.Contains("FDM_Min_Enable : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = bool.TryParse(StrValue, out BoolValue);
                }
                else if (SettingLine.Contains("FDM_Max_Enable : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = bool.TryParse(StrValue, out BoolValue);
                }
                else if (SettingLine.Contains("FDM_Filter : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = bool.TryParse(StrValue, out BoolValue);
                }
                else if (SettingLine.Contains("FDM_Content : "))
                {
                    IsOkay = true;
                }
                else if (SettingLine.Contains("FDM_Intro : "))
                {
                    IsOkay = true;
                }
                else if (SettingLine.Contains("FDM_Effect : "))
                {
                    IsOkay = true;
                }
                else if (SettingLine.Contains("FDM_Outtro : "))
                {
                    IsOkay = true;
                }
                else if (SettingLine.Contains("FDM_Time : "))
                {
                    IsOkay = true;
                }
                else if (SettingLine.Contains("FDM_Min : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = int.TryParse(StrValue, out IntValue);
                }
                else if (SettingLine.Contains("FDM_Max : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = int.TryParse(StrValue, out IntValue);
                }
                #endregion
                #region YoutubeSettingCheck
                if (SettingLine.Contains("Youtube_Enable : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = bool.TryParse(StrValue, out BoolValue);
                }
                else if (SettingLine.Contains("Youtube_Next_Enable : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = bool.TryParse(StrValue, out BoolValue);
                }
                else if (SettingLine.Contains("Youtube_Min_Enable : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = bool.TryParse(StrValue, out BoolValue);
                }
                else if (SettingLine.Contains("Youtube_Max_Enable : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = bool.TryParse(StrValue, out BoolValue);
                }
                else if (SettingLine.Contains("Youtube_Filter : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = bool.TryParse(StrValue, out BoolValue);
                }
                else if (SettingLine.Contains("Youtube_Min : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = int.TryParse(StrValue, out IntValue);
                }
                else if (SettingLine.Contains("Youtube_Max : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = int.TryParse(StrValue, out IntValue);
                }
                #endregion
                #region BroadCastFrmCheck
                if (SettingLine.Contains("Youtube_X : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = int.TryParse(StrValue, out IntValue);
                }
                else if (SettingLine.Contains("Youtube_Y : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = int.TryParse(StrValue, out IntValue);
                }
                else if (SettingLine.Contains("Youtube_Width : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = int.TryParse(StrValue, out IntValue);
                }
                else if (SettingLine.Contains("Youtube_Height : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = int.TryParse(StrValue, out IntValue);
                }
                else if (SettingLine.Contains("FDM_X : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = int.TryParse(StrValue, out IntValue);
                }
                else if (SettingLine.Contains("FDM_Y : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = int.TryParse(StrValue, out IntValue);
                }
                else if (SettingLine.Contains("FDM_Width : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = int.TryParse(StrValue, out IntValue);
                }
                else if (SettingLine.Contains("FDM_Height : "))
                {
                    StrValue = Regex.Split(SettingLine, " : ")[1];
                    IsOkay = int.TryParse(StrValue, out IntValue);
                }
                #endregion
                if (IsOkay == false)
                {
                    return false;
                }
            }
            return true;
        }

        //************************************************************
        //*                Filter Load/Save Functions                *
        //************************************************************

        public static void LoadFilter()
        {
            string path = null;

            path = Directory.GetCurrentDirectory() + "\\CommentFilter.txt";
            if(File.Exists(path)) MessageFilter = File.ReadAllLines(path).ToList();

            path = Directory.GetCurrentDirectory() + "\\PotsuFilter.txt";
            if (File.Exists(path)) PotsuFilter = File.ReadAllLines(path).ToList();

            path = Directory.GetCurrentDirectory() + "\\YoutubeFilter.txt";
            if (File.Exists(path)) YoutubeFilter = File.ReadAllLines(path).ToList();
        }
        public static void SaveFilter()
        {
            string path = null;

            path = Directory.GetCurrentDirectory() + "\\CommentFilter.txt";
            File.WriteAllLines(path, MessageFilter);

            path = Directory.GetCurrentDirectory() + "\\PotsuFilter.txt";
            File.WriteAllLines(path, PotsuFilter);

            path = Directory.GetCurrentDirectory() + "\\YoutubeFilter.txt";
            File.WriteAllLines(path, YoutubeFilter);
        }
    }
}
