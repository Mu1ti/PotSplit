using mshtml;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using System.Threading;

namespace PotSplit
{
    class PotPlayer
    {
        public static IntPtr MainChatRoom = IntPtr.Zero;
        private Thread MainThread = null;
        public PotPlayer()
        {
            MainChatRoom = GetBroadCastChatRoom();
            MainThread = new Thread(DetectBroadCastChatRoom);
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
        }
        private void DetectBroadCastChatRoom()
        {
                MainChatRoom = GetBroadCastChatRoom();
                while (MainChatRoom == IntPtr.Zero)
                {
                    MainChatRoom = GetBroadCastChatRoom();
                    Thread.Sleep(100);
                }
        }
        //************************************************************
        //*                   GetBroadCastChatRoom                   *
        //************************************************************
        private IntPtr GetBroadCastChatRoom()
        {
            List<IntPtr> All_IES = Get_All_IES_Class(IntPtr.Zero).Distinct().ToList();
            List<IntPtr> All_ChatRoom = Get_PotPlayer_ChatRooms(All_IES);
            IntPtr Result = IntPtr.Zero;

            if (All_ChatRoom.Count() > 0) Result = All_ChatRoom[0];
            return Result;
        }
        private List<IntPtr> Get_All_IES_Class(IntPtr Parent)
        {
            List<IntPtr> Result = new List<IntPtr>();
            foreach (IntPtr ChildWindow in GetChildWindows(Parent))
            {
                StringBuilder ClassName = new StringBuilder(260);
                GetClassName(ChildWindow, ClassName, 260);
                if (ClassName.ToString() == "Internet Explorer_Server")
                    Result.Add(ChildWindow);
                else
                {
                    foreach(IntPtr IES in Get_All_IES_Class(ChildWindow))
                    {
                        Result.Add(IES);
                    }
                }
            }
            return Result;
        }
        private List<IntPtr> Get_PotPlayer_ChatRooms(List<IntPtr> IESs)
        {
            List<IntPtr> Result = new List<IntPtr>();
            IntPtr Message = IntPtr.Zero;
            IntPtr MessageTimeOut = IntPtr.Zero;
            IHTMLDocument5 HTMLDocument = null;
            string HTMLContent = null;

            foreach (IntPtr IES in IESs)
            {
                Message = RegisterWindowMessage("WM_HTML_GETOBJECT");
                MessageTimeOut = SendMessageTimeout(IES, Message);
                HTMLDocument = GetHTMLDocument(MessageTimeOut);
                if (HTMLDocument != null && HTMLDocument.GetType().Name == "HTMLDocumentClass" && ((HTMLDocumentClass)HTMLDocument).url.Contains("blank"))
                {
                    HTMLContent = ((HTMLDocumentClass)HTMLDocument).body.innerHTML;
                    if (HTMLContent != null && HTMLContent.Contains("wrap_chat"))
                    {
                        Result.Add(IES);
                    }
                }
            }
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
        private static extern IntPtr RegisterWindowMessage(string lpString);

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

        private static IntPtr SendMessageTimeout(IntPtr IES, IntPtr Message)
        {
            IntPtr ChatRoomHandle = IntPtr.Zero;
            SendMessageTimeout(IES, Message, 0, 0, SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 1000, out ChatRoomHandle);
            return ChatRoomHandle;
        }

        private IHTMLDocument5 GetHTMLDocument(IntPtr ChatRoomHandle)
        {
            IHTMLDocument5 Result = null;
            try
            {
                Result = (IHTMLDocument5)ObjectFromLresult(ChatRoomHandle, typeof(IHTMLDocument5).GUID, IntPtr.Zero);
            }
            catch (COMException)
            {
                //방송 종료
            }
            catch (InvalidCastException)
            {
                return null;
            }
            return Result;
        }

        //************************************************************
        //*              Initialize about windows apis               *
        //************************************************************

        private delegate bool EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr i);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        private delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter);

        private static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);
            try
            {
                EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
                EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
            return result;
        }
        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
            if (list == null)
            {
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            }
            list.Add(handle);
            return true;
        }
        #endregion
    }
}
