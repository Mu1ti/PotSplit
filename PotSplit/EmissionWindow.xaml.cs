using Gma.System.MouseKeyHook;
using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using mshtml;
using System.Windows.Media;

namespace PotSplit
{
    public partial class EmissionWindow : Window
    {
        DispatcherTimer FDMTimer = new DispatcherTimer();
        DispatcherTimer YoutubeTimer = new DispatcherTimer();
        DispatcherTimer Detector = new DispatcherTimer();
        IKeyboardMouseEvents TransferController;
        string NowDirectory = Directory.GetCurrentDirectory();
        bool YoutubeMove = false;
        bool FDMMove = false;
        bool WindowMove = false;
        int FDMCount = 0;

        public EmissionWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeTransferController();
            InitializeTimer();
            SetupSetting();
            File.WriteAllText(NowDirectory + @"\tmp.html", Utility.MoveModeHTML(), Encoding.UTF8);
            FDM_Main.Source = new Uri(string.Format("file:///{0}/tmp.html", NowDirectory));
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            FDMTimer.Stop();
            YoutubeTimer.Stop();
            Detector.Stop();
        }
        private void SetupSetting()
        {
            Canvas.SetTop(YoutubeGrid, Setting.Youtube_Y);
            Canvas.SetLeft(YoutubeGrid, Setting.Youtube_X);
            YoutubeGrid.Width = Setting.Youtube_Width;
            YoutubeGrid.Height = Setting.Youtube_Height;

            Canvas.SetTop(FDM_Main, Setting.FDM_Y);
            Canvas.SetLeft(FDM_Main, Setting.FDM_X);
            FDM_Main.Width = Setting.FDM_Width;
            FDM_Main.Height = Setting.FDM_Height;
        }

        //************************************************************
        //*                     Timer Functions                      *
        //************************************************************

        private void InitializeTimer()
        {
            Detector.Tick += Detector_Tick;
            Detector.Interval = TimeSpan.FromMilliseconds(100);
            Detector.IsEnabled = true;

            FDMTimer.Tick += FDMTimer_Tick;
            FDMTimer.Interval = TimeSpan.FromMilliseconds(1000);

            YoutubeTimer.Tick += YoutubeTimer_Tick;
            YoutubeTimer.Interval = TimeSpan.FromMilliseconds(100);
        }
        private void Detector_Tick(object sender, EventArgs e)
        {
            bool DoShow = true;
            if (Setting.FDM_Enable || Setting.Youtube_Enable)
            {
                if (Donators.FDMWaitingList.Count > 0)
                {
                    DoShow = true;
                    if (Setting.FDM_Min_Enable == true && int.Parse(Donators.FDMWaitingList[0][1]) < Setting.FDM_Min) { DoShow = false; }
                    if (Setting.FDM_Max_Enable == true && int.Parse(Donators.FDMWaitingList[0][1]) > Setting.FDM_Max) { DoShow = false; }
                    if (DoShow)
                    {
                        FDMTimer.Start();
                    }
                    else Donators.FDMWaitingList.RemoveAt(0);
                }
                if (Donators.YoutubeWaitingList.Count > 0)
                {
                    DoShow = true;
                    if (Setting.Youtube_Min_Enable == true && int.Parse(Donators.YoutubeWaitingList[0][1]) < Setting.Youtube_Min) { DoShow = false; }
                    if (Setting.Youtube_Max_Enable == true && int.Parse(Donators.YoutubeWaitingList[0][1]) > Setting.Youtube_Max) { DoShow = false; }
                    if (DoShow)
                    {
                        YoutubeTimer.Start();
                    }
                    else Donators.YoutubeWaitingList.RemoveAt(0);
                }
            }
            else
            {
                Donators.FDMWaitingList.Clear();
                Donators.YoutubeWaitingList.Clear();
            }
        }
        private void FDMTimer_Tick(object sender, EventArgs e)
        {
            string Content = Utility.FDMTagToContent(Setting.FDM_Content, Donators.FDMWaitingList[0]); ;
            if (FDMCount < 1)
            {
                File.WriteAllText(NowDirectory + @"\DonateMessage.html", Utility.GetFDMHTMLSource(Setting.FDM_Intro, Content), Encoding.UTF8);
            }
            else if (FDMCount == int.Parse(Donators.FDMWaitingList[0][3]) -1)
            {
                File.WriteAllText(NowDirectory + @"\DonateMessage.html", Utility.GetFDMHTMLSource(Setting.FDM_Outtro, Content), Encoding.UTF8);
            }
            else if (FDMCount == int.Parse(Donators.FDMWaitingList[0][3]))
            {
                File.WriteAllText(NowDirectory + @"\DonateMessage.html", Utility.NULLDocument(), Encoding.UTF8);
                Donators.FDMWaitingList.RemoveAt(0);
                FDMTimer.IsEnabled = false;
                FDMCount = 0;
            }
            else
            {
                File.WriteAllText(NowDirectory + @"\DonateMessage.html", Utility.GetFDMHTMLSource(Setting.FDM_Effect, Content), Encoding.UTF8);

            }
            FDMCount++;
            FDM_Main.Source = new Uri(string.Format("file:///{0}/DonateMessage.html", NowDirectory));
        }
        private void YoutubeTimer_Tick(object sender, EventArgs e)
        {
            string VideoId = "";
            bool DoShow = true;
            if (Setting.Youtube_Enable)
            {
                if ((Youtube.PlayerState == YoutubePlayerLib.YoutubePlayerState.ended || 
                     Youtube.VideoId == "uBBDMqZKagY" || 
                     Youtube.PlayerState == YoutubePlayerLib.YoutubePlayerState.unstarted ||
                     Youtube.PlayerState == YoutubePlayerLib.YoutubePlayerState.videoCued) && Donators.YoutubeWaitingList.Count > 0 && Youtube.AutoPlay)
                {
                    if (Setting.Youtube_Min_Enable == true && int.Parse(Donators.YoutubeWaitingList[0][1]) < Setting.Youtube_Min) { DoShow = false; }
                    if (Setting.Youtube_Max_Enable == true && int.Parse(Donators.YoutubeWaitingList[0][1]) > Setting.Youtube_Max) { DoShow = false; }
                    if (DoShow)
                    {
                        VideoId = Utility.YoutubeIdExtractor(Donators.YoutubeWaitingList[0][2]);
                        Youtube.VideoId = VideoId;
                        Youtube.StartCommand.Execute(null);
                    } else Donators.FDMWaitingList.RemoveAt(0);
                }
            } else Donators.FDMWaitingList.RemoveAt(0);

            if (Setting.Youtube_Enable) { YoutubeGrid.Visibility = Visibility.Visible; }
            else { YoutubeGrid.Visibility = Visibility.Hidden; Youtube.StopCommand.Execute(null); };

            Youtube.AutoPlay = Setting.Youtube_Next_Enable;
        }

        //************************************************************
        //*             TransferController Functions                 *
        //************************************************************

        private void InitializeTransferController()
        {
            TransferController = Hook.GlobalEvents();
            TransferController.MouseDown += ControlMoveOn;
            TransferController.MouseUp += ControlMoveOff;
            TransferController.MouseMove += ControlMove;
            TransferController.MouseDoubleClick += ControlResize;
        }
        private void ControlResize(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            double MouseX = e.X - this.Left;
            double MouseY = e.Y - this.Top;
            if(this.IsActive)
            {
                if (MouseX > Canvas.GetLeft(FDM_Main) &&
                    MouseX < Canvas.GetLeft(FDM_Main) + FDM_Main.Width &&
                    MouseY > Canvas.GetTop(FDM_Main) &&
                    MouseY < Canvas.GetTop(FDM_Main) + FDM_Main.Height)
                {
                    if (FDM_Main.Width >= 1280)
                    {
                        FDM_Main.Width = 320;
                        FDM_Main.Height = 75;
                    }
                    else
                    {
                        FDM_Main.Width = FDM_Main.Width * 2;
                        FDM_Main.Height = FDM_Main.Height * 2;
                    }
                    File.WriteAllText(NowDirectory + @"\tmp.html", Utility.MoveModeHTML(), Encoding.UTF8);
                    FDM_Main.Source = new Uri(string.Format("file:///{0}/tmp.html", NowDirectory));
                }
                else if (MouseX > Canvas.GetLeft(YoutubeGrid) &&
                        MouseX < Canvas.GetLeft(YoutubeGrid) + YoutubeGrid.Width &&
                        MouseY > Canvas.GetTop(YoutubeGrid) &&
                        MouseY < Canvas.GetTop(YoutubeGrid) + YoutubeGrid.Height)
                {
                    if (YoutubeGrid.Width >= 640)
                    {
                        YoutubeGrid.Width = 160;
                        YoutubeGrid.Height = 130;
                    }
                    else if(YoutubeGrid.Width >= 160)
                    {
                        YoutubeGrid.Width = 320;
                        YoutubeGrid.Height = 230;
                    }
                    else if(YoutubeGrid.Width >= 320)
                    {
                        YoutubeGrid.Width = 640;
                        YoutubeGrid.Height = 400;
                    }
                }
                Setting.Youtube_X = (int)Canvas.GetLeft(YoutubeGrid);
                Setting.Youtube_Y = (int)Canvas.GetTop(YoutubeGrid);
                Setting.Youtube_Height = (int)YoutubeGrid.Height;
                Setting.Youtube_Width = (int)YoutubeGrid.Width;

                Setting.FDM_X = (int)Canvas.GetLeft(FDM_Main);
                Setting.FDM_Y = (int)Canvas.GetTop(FDM_Main);
                Setting.FDM_Height = (int)FDM_Main.Height;
                Setting.FDM_Width = (int)FDM_Main.Width;
                if (this.IsLoaded) Setting.SaveSetting();
            }
        }
        private void ControlMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            double MouseX = e.X - this.Left;
            double MouseY = e.Y - this.Top;
            if(YoutubeMove)
            {
                Canvas.SetTop(YoutubeGrid, MouseY - YoutubeGrid.Height / 2);
                Canvas.SetLeft(YoutubeGrid, MouseX - YoutubeGrid.Width / 2);
            }
            else if(FDMMove)
            {
                Canvas.SetTop(FDM_Main, MouseY - FDM_Main.Height / 2);
                Canvas.SetLeft(FDM_Main, MouseX - FDM_Main.Width / 2);
                MainCanvas.Background = Brushes.LimeGreen;
                MainCanvas.Background = Brushes.Lime;
            }
            else if(WindowMove)
            {
                this.Left = e.X - this.Width / 2;
                this.Top = e.Y - this.Height / 2;
            }
        }
        private void ControlMoveOff(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
            YoutubeMove = false;
            WindowMove = false;
            if(FDMMove)
            {
                FDMMove = false;
                if(Path.GetFileName(FDM_Main.Source.AbsolutePath).Contains("tmp"))
                {
                    File.WriteAllText(NowDirectory + @"\tmp.html", Utility.NULLDocument(), Encoding.UTF8);
                    FDM_Main.Source = new Uri(string.Format("file:///{0}/tmp.html", NowDirectory));
                }
                MainCanvas.Background = Brushes.LimeGreen;
                MainCanvas.Background = Brushes.Lime;
            }
            Setting.Youtube_X = (int)Canvas.GetLeft(YoutubeGrid);
            Setting.Youtube_Y = (int)Canvas.GetTop(YoutubeGrid);
            Setting.Youtube_Height = (int)YoutubeGrid.Height;
            Setting.Youtube_Width = (int)YoutubeGrid.Width;

            Setting.FDM_X = (int)Canvas.GetLeft(FDM_Main);
            Setting.FDM_Y = (int)Canvas.GetTop(FDM_Main);
            Setting.FDM_Height = (int)FDM_Main.Height;
            Setting.FDM_Width = (int)FDM_Main.Width;
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void ControlMoveOn(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            double MouseX = e.X - this.Left;
            double MouseY = e.Y - this.Top;
            if(this.IsActive)
            {
                if (MouseX > Canvas.GetLeft(FDM_Main) &&
                    MouseX < Canvas.GetLeft(FDM_Main) + FDM_Main.Width &&
                    MouseY > Canvas.GetTop(FDM_Main) &&
                    MouseY < Canvas.GetTop(FDM_Main) + FDM_Main.Height)
                {
                    FDMMove = true;
                    Cursor = Cursors.Hand;
                    if (Path.GetFileName(FDM_Main.Source.AbsolutePath).Contains("tmp"))
                    {
                        File.WriteAllText(NowDirectory + @"\tmp.html", Utility.MoveModeHTML(), Encoding.UTF8);
                        FDM_Main.Source = new Uri(string.Format("file:///{0}/tmp.html", NowDirectory));
                    }
                }
                else if (MouseX > Canvas.GetLeft(YoutubeGrid) &&
                         MouseX < Canvas.GetLeft(YoutubeGrid) + YoutubeGrid.Width &&
                         MouseY > Canvas.GetTop(YoutubeGrid) &&
                         MouseY < Canvas.GetTop(YoutubeGrid) + YoutubeGrid.Height)
                {
                    YoutubeMove = true;
                    Cursor = Cursors.Hand;
                }
                else
                {
                    if(e.X > this.Left &&
                       e.X < this.Left + this.Width &&
                       e.Y > this.Top &&
                       e.Y < this.Top + this.Height)
                    {
                        Cursor = Cursors.Hand;
                        WindowMove = true;
                    }
                }
            }
        }

        //************************************************************
        //*                Youtube Control Functions                 *
        //************************************************************

        private void YoutubeStopButton_Click(object sender, RoutedEventArgs e)
        {
            YoutubePlayButton.Command = Youtube.PauseCommand;
            YoutubePlayButton.Content = "Play";
        }
        private void YoutubePlayButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string)YoutubePlayButton.Content == "Play")
            {
                YoutubePlayButton.Command = Youtube.StartCommand;
                YoutubePlayButton.Content = "Pause";
            }
            else
            {
                YoutubePlayButton.Command = Youtube.PauseCommand;
                YoutubePlayButton.Content = "Play";
            }
        }
        private void YoutubeNextButton_Click(object sender, RoutedEventArgs e)
        {
            string VideoId = "";
            bool DoShow = true;
            if(Setting.Youtube_Enable && Donators.YoutubeWaitingList.Count > 0)
            {
                Youtube.PlayerState = YoutubePlayerLib.YoutubePlayerState.ended;
                while (DoShow)
                {
                    if (Setting.Youtube_Min_Enable == true && int.Parse(Donators.YoutubeWaitingList[0][1]) < Setting.Youtube_Min) { DoShow = false; }
                    if (Setting.Youtube_Max_Enable == true && int.Parse(Donators.YoutubeWaitingList[0][1]) > Setting.Youtube_Max) { DoShow = false; }
                    if(DoShow)
                    {
                        VideoId = Utility.YoutubeIdExtractor(Donators.YoutubeWaitingList[0][2]);
                        Youtube.VideoId = VideoId;
                        Youtube.StartCommand.Execute(null);
                        break;
                    } else Donators.FDMWaitingList.RemoveAt(0);
                }
            }
        }
        private void YoutubeGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            YoutubeControlGrid.Visibility = Visibility.Visible;
        }
        private void YoutubeGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            YoutubeControlGrid.Visibility = Visibility.Hidden;
        }
        private void YoutubePlaystateChanged(object sender, TextChangedEventArgs e)
        {
            if (YoutubePlayState.Text == "ended")
            {
                Donators.YoutubeWaitingList.RemoveAt(0);
            }
        }
    }
}
