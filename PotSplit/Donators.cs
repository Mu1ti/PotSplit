using HtmlAgilityPack;
using mshtml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace PotSplit
{
    class Donators
    {
        private Thread MainThread = null;
        public static List<string[]> AllDonators = new List<string[]>();
        public static List<string[]> TTSWaitingList = new List<string[]>();
        public static List<string[]> FDMWaitingList = new List<string[]>();
        public static List<string[]> YoutubeWaitingList = new List<string[]>();
        
        public Donators()
        {
            MainThread = new Thread(DonationDetect);
            MainThread.IsBackground = true;
        }
        //************************************************************
        //*                  DetectBroadCastChatRoom                 *
        //************************************************************
        public void StartDetect()
        {
            MainThread.Start();
        }
        public void StopDetect()
        {
            MainThread.Abort();
            while(MainThread.ThreadState != ThreadState.Aborted)
            {
                Thread.Sleep(10);
            }
        }
        private void DonationDetect()
        {
            List<string[]> NewDonators = null;
            string ChatContent = null;
            while (true)
            {
                if(PotPlayer.MainChatRoom != IntPtr.Zero)
                {
                    ChatContent = GetChatContent();
                    if(AllDonators.Count == 0) AllDonators = GetAllDonator(ChatContent);
                    NewDonators = GetNewDonator(ChatContent);
                    if (NewDonators != null)
                    {
                        foreach(string[] Donator in NewDonators)
                        {
                            AllDonators.Add(Donator);
                            Utility.SplitDonation(Donator);
                        }
                    }
                }
                Thread.Sleep(10);
            }
        }
        //************************************************************
        //*              Functions of get new donators               *
        //************************************************************
        private string GetChatContent()
        {
            string HTMLContent = null;
            try
            {
                IntPtr Message = RegisterWindowMessage("WM_HTML_GETOBJECT");
                IntPtr MessageTimeOut = SendMessageTimeout(PotPlayer.MainChatRoom, Message);
                IHTMLDocument5 HTMLDocument = GetHTMLDocument(MessageTimeOut);
                HTMLContent = ((HTMLDocumentClass)HTMLDocument).body.innerHTML;
            }
            catch (Exception e)
            {
                File.WriteAllText(@".\GetChatContentError.txt", e.ToString());
            }
            return HTMLContent;
        }
        private List<string[]> GetAllDonator(string HTMLContent)
        {
            HtmlDocument doc = new HtmlDocument();
            HtmlNodeCollection nodeCol;
            List<string[]> Result = new List<string[]>();
            if (HTMLContent != null)
            {
                doc.LoadHtml(HTMLContent);
                nodeCol = doc.DocumentNode.SelectNodes("//div[@class='txt_notice area_space box_sticker']");
                if (nodeCol != null)
                {
                    foreach (HtmlNode Donate in doc.DocumentNode.SelectNodes("//div[@class='txt_notice area_space box_sticker']"))
                    {
                        string[] ResultString = new string[3];
                        ResultString[0] = Utility.Rgx2StrArr("(?<=<p class=\"txt_spon\">).*.(?=님)", Donate.OuterHtml, RegexOptions.None)[0];
                        ResultString[1] = Utility.Rgx2StrArr("(?<=txt_type0.\">).*.(?=</span><)", Donate.OuterHtml, RegexOptions.None)[0];
                        ResultString[2] = Utility.Rgx2StrArr("(?<=</span>).*.(?=</p>)", Donate.OuterHtml, RegexOptions.None)[0];
                        ResultString[1] = ResultString[1].Replace(",", "");
                        Result.Add(ResultString);
                    }
                }
            }
            return Result;
        }
        private List<string[]> GetNewDonator(string ChatContent)
        {
            List<string[]> Donators = GetAllDonator(ChatContent);
            List<string[]> Result = new List<string[]>();
            int DuplicateStart = 0;
            int DuplicateEnd = 0;
            int DuplicateCount = 0;

            if (AllDonators.Count == 0)
                return Donators;
            else if (Donators.Count > AllDonators.Count)
                for (int i = AllDonators.Count; Donators.Count > i; i++)
                    Result.Add(Donators[i]);
            else if (Donators.Count < AllDonators.Count)
            {
                if (Donators.Count == 0) return null;
                for (DuplicateStart = AllDonators.Count - Donators.Count; AllDonators.Count - 1 > DuplicateStart; DuplicateStart++)
                    if (Donators[0].Equals(AllDonators[DuplicateStart])) break;
                for (DuplicateEnd = DuplicateStart; AllDonators.Count - 1 > DuplicateEnd; DuplicateEnd++)
                {
                    if (Donators[0].Equals(AllDonators[DuplicateEnd]))
                        break;
                    else
                        DuplicateCount++;
                }
                for (int i = DuplicateCount; Donators.Count > i; i++)
                    Result.Add(Donators[i]);
            }
            else return null;
            return Result;
        }
        //************************************************************
        //*                      Initialize APIs                     *
        //************************************************************
        #region Initialize APIs
        //************************************************************
        //*              Initialize about message apis               *
        //************************************************************
        [DllImport("user32")]
        public static extern IntPtr RegisterWindowMessage(string lpString);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessageTimeout(
        IntPtr windowHandle,
        IntPtr Msg,
        int wParam,
        int lParam,
        SendMessageTimeoutFlags flags,
        uint timeout,
        out IntPtr result);

        [Flags]
        private enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
            SMTO_ERRORONEXIT = 0x0020
        }

        [DllImport("oleacc.dll", PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        private static extern object ObjectFromLresult(IntPtr lResult,
         [MarshalAs(UnmanagedType.LPStruct)] Guid refiid, IntPtr wParam);

        public static IntPtr SendMessageTimeout(IntPtr IES, IntPtr Message)
        {
            IntPtr ChatRoomHandle = IntPtr.Zero;
            SendMessageTimeout(IES, Message, 0, 0, SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 1000, out ChatRoomHandle);
            return ChatRoomHandle;
        }

        public IHTMLDocument5 GetHTMLDocument(IntPtr ChatRoomHandle)
        {
            IHTMLDocument5 Result = null;
            try
            {
                Result = (IHTMLDocument5)ObjectFromLresult(ChatRoomHandle, typeof(IHTMLDocument5).GUID, IntPtr.Zero);
            }
            catch (COMException)
            {
            }
            catch (InvalidCastException)
            {
                return null;
            }
            return Result;
        }
        #endregion
    }
}
