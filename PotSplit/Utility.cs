using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PotSplit
{
    class Utility
    {
        public static string[] Rgx2StrArr(string reg, string str, RegexOptions Option)
        {
            try
            {
                int i = 0;

                string[] Result = new string[Regex.Matches(str, reg, Option).Count];
                if (Result.Length == 0) Result = new string[1];
                foreach (Match match in Regex.Matches(str, reg, Option))
                {
                    Result[i] = match.Value;
                    i++;
                }
                return Result;
            }
            catch (Exception)
            {
                string[] Result = new string[1];
                return Result;
            }
        }
        public static string TagConverter(string Comment, string[] NewDonator)
        {
            if (Comment.Contains("{팟수}")) Comment = Comment.Replace("{팟수}", NewDonator[0]);
            if (Comment.Contains("{금액}")) Comment = Comment.Replace("{금액}", NewDonator[1]);
            if (Comment.Contains("{담팟금액}")) Comment = Comment.Replace("{담팟금액}", NewDonator[1] + "0000");
            if (Comment.Contains("{메세지}")) Comment = Comment.Replace("{메세지}", NewDonator[2]);
            return Comment;
        }
        public static bool Filter(string[] NewDonator)
        {
            foreach(string Str in Setting.PotsuFilter)
            {
                if (NewDonator[0] == Str) return false;
            }
            foreach(string Str in Setting.MessageFilter)
            {
                if (NewDonator[2].Contains(Str)) return false;
            }
            foreach(string Str in Setting.YoutubeFilter)
            {
                if (NewDonator[2].Contains(YoutubeIdExtractor(Str))) return false;
            }
            return true;
        }
        public static void SplitDonation(string[] Donator)
        {
            string[] FDMDonator = new string[4];
            if (Donator[2].Contains("youtu.be/") || Donator[2].Contains("watch?v="))
            {
                if(Setting.Youtube_Filter)
                {
                    if(Filter(Donator)) Donators.YoutubeWaitingList.Add(Donator);
                }
                else Donators.YoutubeWaitingList.Add(Donator);
            }
            else
            {
                FDMDonator[0] = Donator[0];
                FDMDonator[1] = Donator[1];
                FDMDonator[2] = Donator[2];
                FDMDonator[3] = "";
                
                if(Setting.TTS_Filter)
                {
                    if(Filter(Donator)) Donators.TTSWaitingList.Add(Donator);
                }
                else Donators.TTSWaitingList.Add(Donator);

                if(Setting.FDM_Filter)
                {
                    if(Filter(Donator)) Donators.FDMWaitingList.Add(FDMDonator);
                }
                else Donators.FDMWaitingList.Add(FDMDonator);
            }
        }
        public static string GetFDMHTMLSource(string Effect, string Content)
        {
            string FDMHTML = @"
                        <meta http-equiv='X-UA-Compatible' content='IE=edge'>
                        <head>
                            <link rel='stylesheet' href='animate.css'>
                        </head>
                        <body scroll='no' bgcolor='00FF00'>
                            <br /><br /><br /><br /><br />
                            <div style='text-align:center' class='animated infinite " + Effect + @"'>"
                                + Content + 
                          @"</div>
                        </bddy>
                        ";
            return FDMHTML;
        }
        public static string FDMTagToContent(string Content, string[] NewDonator)
        {
            string BodyTag = "";
            string Return = Content;
            string[] Tags = null;
            while (true)
            {
                if (Return.Contains("{본문"))
                {
                    BodyTag = Return.Substring(Return.IndexOf("{본문"));
                    if (BodyTag.Contains('}'))
                    {
                        BodyTag = BodyTag.Split('}')[0];
                        if (BodyTag.Contains(','))
                        {
                            Tags = BodyTag.Split(',');
                            BodyTag = BodyTag + "}";
                            if (Tags.Length > 1 && Tags.Length < 5)
                            {
                                if (Tags.Length == 2)
                                {
                                    Return = "<font color='" + Tags[1].Trim() + "'>" + Return.Replace(BodyTag, "") + "</font>";
                                }
                                else if (Tags.Length == 3)
                                {
                                    Return = "<font color='" + Tags[1].Trim() + "' face='" + Tags[2].Trim() + "'>" + Return.Replace(BodyTag, "") + "</font>";
                                }
                                else if (Tags.Length == 4)
                                {
                                    Return = "<font color='" + Tags[1].Trim() + "' face='" + Tags[2].Trim() + "' size='" + Tags[3].Trim() + "'>" + Return.Replace(BodyTag, "") + "</font>";
                                }
                                else Return = Return.Replace(BodyTag, "");
                            }
                            else Return = Return.Replace(BodyTag, "");
                        }
                        else Return = Return.Replace(BodyTag+"}", "");
                    }
                    else Return = Return.Replace(BodyTag, "");
                }
                else if (Return.Contains("{팟수"))
                {
                    Return = TagToHTMLTag("팟수", Return, NewDonator[0]);
                }
                else if (Return.Contains("{금액"))
                {
                    Return = TagToHTMLTag("금액", Return, NewDonator[1]);
                }
                else if (Return.Contains("{담팟금액"))
                {
                    Return = TagToHTMLTag("담팟금액", Return, NewDonator[1] + "0000");
                }
                else if (Return.Contains("{메세지"))
                {
                    Return = TagToHTMLTag("메세지", Return, NewDonator[2]);
                }
                else break;
            }
            return Return;
        }
        private static string TagToHTMLTag(string Tag, string body, string Content)
        {
            string Return = body;
            string ThisTag = "";
            string[] Tags = null;

            ThisTag = Return.Substring(Return.IndexOf("{"+ Tag));
            if (ThisTag.Contains('}'))
            {
                ThisTag = ThisTag.Split('}')[0];
                if (ThisTag.Contains(','))
                {
                    Tags = ThisTag.Split(',');
                    ThisTag = ThisTag + "}";
                    if (Tags.Length > 1 && Tags.Length < 5)
                    {
                        if (Tags.Length == 2)
                        {
                            Return = Return.Replace(ThisTag, "<font color='" + Tags[1].Trim() + "'>" + Content.Trim() + "</font>");
                        }
                        else if (Tags.Length == 3)
                        {
                            Return = Return.Replace(ThisTag, "<font color='" + Tags[1].Trim() + "'face='" + Tags[2].Trim() + "'>" + Content.Trim() + "</font>");
                        }
                        else if (Tags.Length == 4)
                        {
                            Return = Return.Replace(ThisTag, "<font color='" + Tags[1].Trim() + "'face='" + Tags[2].Trim() + "' size='" + Tags[3].Trim() + "'>" + Content.Trim() + "</font>");
                        }
                        else Return = Return.Replace(ThisTag, "");
                    }
                    else Return = Return.Replace(ThisTag, "");
                }
                else Return = Return.Replace(ThisTag+"}", Content);
            }
            else Return = Return.Replace(ThisTag, "");
            return Return;
        }
        public static string MoveModeHTML()
        {
            string HTML = @"
                        <!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>
                        <html xmlns='http://www.w3.org/1999/xhtml'>
                            <head>
                                <meta http-ｅquiv='Content-Type' content='text/html; charset=utf-8' />
                                <style type='text/css'> #center { position:absolute; top:50%; left:50%; } </style>
                            </head>
                            <body scroll='no' bgcolor='808080'>
                                <div id='center'>후원 메세지가 여기에 보여집니다.</div>
                            </body>
                        </html>
                        ";
            return HTML;
        }
        public static string NULLDocument()
        {
            string HTML =   @"
                            <body scroll='no' bgcolor='00FF00'>
                            ";
            return HTML;
        }
        public static string YoutubeIdExtractor(string Comment)
        {
            string Result = "";
            if(Comment.Contains("watch?v=")) Result = Comment.Split('=')[1];
            if (Comment.Contains(".be/")) Result = Comment.Substring(Comment.IndexOf(".be/")+4);

            return Result;
        }
        public static ImageSource PotSplitIcon()
        {
            Bitmap bitmap = Properties.Resources.PotSplit.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap =
                 Imaging.CreateBitmapSourceFromHBitmap(
                      hBitmap, IntPtr.Zero, Int32Rect.Empty,
                      BitmapSizeOptions.FromEmptyOptions());

            return wpfBitmap;
        }
    }
}
