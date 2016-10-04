using MahApps.Metro.Controls;
using System;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Interop;

namespace PotSplit
{
    public partial class MainWindow : MetroWindow
    {
        /*
             - DonatorList Structure
             - 0 DonatorUserName
             - 1 DonatorPrice
             - 2 DonatorCommet
        */
        PotPlayer ChatRoomDetector = null;
        Donators NewDonateDetector = null;
        TTSEngine TTSMesgeDetector = null;
        StartFrm Loading = new StartFrm();
        EmissionWindow BroadCastWindow = new EmissionWindow();
        string[] TestDonator = { "팟르비욘", "10000", "모래반지 빵야빵야" };
        string NowDirectory = Directory.GetCurrentDirectory();
        int FDMPreviewCount = 0;

        public MainWindow()
        {
            Loading.Show();
            this.Icon = Utility.PotSplitIcon();

            InitializeComponent();
            SetupSetting();

            ChatRoomDetector = new PotPlayer();
            NewDonateDetector = new Donators();
            TTSMesgeDetector = new TTSEngine();

            ChatRoomDetector.StartDetect();
            NewDonateDetector.StartDetect();
            TTSMesgeDetector.StartDetect();

            BroadCastWindow.Show();
            BroadCastWindow.Hide();

            Loading.Close();
        }
        private void MainFrm_Closed(object sender, EventArgs e)
        {
            ChatRoomDetector.StopDetect();
            NewDonateDetector.StopDetect();
            TTSMesgeDetector.StopDetect();

            BroadCastWindow.Close();
            this.Close();
            Application.Current.Shutdown();
        }

        private void SetupSetting()
        {
            string path = Directory.GetCurrentDirectory() + "\\Setting.ini";
            if (File.Exists(path))
            {
                Setting.LoadSetting(path);
                Setting.LoadFilter();
                TTS_Enable_Switch.IsChecked = Setting.TTS_Enable;
                TTS_Min_Switch.IsChecked = Setting.TTS_Min_Enable;
                TTS_Max_Switch.IsChecked = Setting.TTS_Max_Enable;
                TTS_Content_Switch.IsChecked = Setting.TTS_Content_Enable;
                TTS_Effect_Sound.IsChecked = Setting.TTS_Effect_Enable;
                TTS_Filter_Switch.IsChecked = Setting.TTS_Filter;
                TTS_Min_Text.Text = Setting.TTS_Min.ToString();
                TTS_Max_Text.Text = Setting.TTS_Max.ToString();
                TTS_Content_Text.Text = Setting.TTS_Content;

                FDM_Enable_Switch.IsChecked = Setting.FDM_Enable;
                FDM_Min_Switch.IsChecked = Setting.FDM_Min_Enable;
                FDM_Max_Switch.IsChecked = Setting.FDM_Max_Enable;
                FDM_Filter_Switch.IsChecked = Setting.FDM_Filter;
                FDM_Min_Text.Text = Setting.FDM_Min.ToString();
                FDM_Max_Text.Text = Setting.FDM_Max.ToString();
                FDM_Content_Text.Text = Setting.FDM_Content;
                FDM_In_Effect_ComboBox.SelectedValue = Setting.FDM_Intro;
                FDM_Effect_ComboBox.SelectedValue = Setting.FDM_Effect;
                FDM_Out_Effect_ComboBox.SelectedValue = Setting.FDM_Outtro;

                Youtube_Enable_Switch.IsChecked = Setting.Youtube_Enable;
                Youtube_Next_Switch.IsChecked = Setting.Youtube_Next_Enable;
                Youtube_Max_Switch.IsChecked = Setting.Youtube_Max_Enable;
                Youtube_Min_Switch.IsChecked = Setting.Youtube_Min_Enable;
                Youtube_Filter_Switch.IsChecked = Setting.Youtube_Filter;
                Youtube_Max_Text.Text = Setting.Youtube_Max.ToString();
                Youtube_Min_Text.Text = Setting.Youtube_Min.ToString();
                
                if(Setting.TTS_Effect_List != "")
                {
                    foreach(string TTS_Effect in Setting.TTS_Effect_List.Split('/'))
                    {
                        TTS_Effect_Sound_Combo.Items.Add(TTS_Effect);
                    }
                    TTS_Effect_Sound_Combo.SelectedIndex = Setting.TTS_Effect;
                }

                if (Setting.TTS_Voice == "TTS_Voice_Heami")
                {
                    TTS_Voice_Heami.IsChecked = true;
                    TTS_Voice_Google.IsChecked = false;
                    TTS_Voice_Randome.IsChecked = false;
                }
                else if (Setting.TTS_Voice == "TTS_Voice_Google")
                {
                    TTS_Voice_Heami.IsChecked = false;
                    TTS_Voice_Google.IsChecked = true;
                    TTS_Voice_Randome.IsChecked = false;
                }
                else if (Setting.TTS_Voice == "TTS_Voice_Randome")
                {
                    TTS_Voice_Heami.IsChecked = false;
                    TTS_Voice_Google.IsChecked = false;
                    TTS_Voice_Randome.IsChecked = true;
                }

                if(Setting.MessageFilter.Count > 0)
                {
                    foreach(string str in Setting.MessageFilter)
                    {
                        Filter_Message_ListView.Items.Add(str);
                    }
                }
                if(Setting.PotsuFilter.Count > 0)
                {
                    foreach(string str in Setting.PotsuFilter)
                    {
                        Filter_Potsu_ListView.Items.Add(str);
                    }
                }
                if(Setting.YoutubeFilter.Count > 0)
                {
                    foreach(string str in Setting.YoutubeFilter)
                    {
                        Filter_Youtube_ListView.Items.Add(str);
                    }
                }
            }
        }
        private void ShowBroadCastButton_Click(object sender, RoutedEventArgs e)
        {
            if (BroadCastWindow.IsVisible) BroadCastWindow.Hide();
            else BroadCastWindow.Show();
        }
        private void PreviewTimer_Tick(object sender, EventArgs e)
        {
            string Content = Utility.FDMTagToContent(FDM_Content_Text.Text, TestDonator); ;
            if (FDMPreviewCount < 1)
            {
                File.WriteAllText(NowDirectory + @"\DonateMessagePreview.html", Utility.GetFDMHTMLSource(Setting.FDM_Intro, Content), Encoding.UTF8);
            }
            else if (FDMPreviewCount == 4)
            {
                File.WriteAllText(NowDirectory + @"\DonateMessagePreview.html", Utility.GetFDMHTMLSource(Setting.FDM_Outtro, Content), Encoding.UTF8);
            }
            else if (FDMPreviewCount > 4)
            {
                FDMPreviewCount = -1;
                File.WriteAllText(NowDirectory + @"\DonateMessagePreview.html", Utility.NULLDocument(), Encoding.UTF8);
                FDM_Preview_WebBrowser.Source = new Uri(string.Format("file:///{0}/DonateMessagePreview.html", NowDirectory));
                ((DispatcherTimer)sender).Stop();
            }
            else
            {
                File.WriteAllText(NowDirectory + @"\DonateMessagePreview.html", Utility.GetFDMHTMLSource(Setting.FDM_Effect, Content), Encoding.UTF8);
            }
            FDMPreviewCount++;
            FDM_Preview_WebBrowser.Source = new Uri(string.Format("file:///{0}/DonateMessagePreview.html", NowDirectory));
        }
        private void OnlyDigits(object sender, TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if(!char.IsDigit(c))
                {
                    e.Handled = true;
                    break;
                }
            }
        }

        private void TTS_Content_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(TTS_Content_Switch.IsChecked == true)
                Setting.TTS_Content = TTS_Content_Text.Text;
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void TTS_Enable_Switch_IsCheckedChanged(object sender, EventArgs e)
        {
            Setting.TTS_Enable = (bool)TTS_Enable_Switch.IsChecked;
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void TTS_Preview_Button_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name == "TTS_Preview_Button" )
            {
                TTSMesgeDetector.TTS_Effect();
                TTSMesgeDetector.TTSSpeach(Utility.TagConverter(TTS_Content_Text.Text, TestDonator));
            }
            else
            {
                TTSMesgeDetector.TTSSpeach(Utility.TagConverter(TTS_Content_Text.Text, TestDonator));
            }
        }
        private void TTS_Voice_Changed(object sender, RoutedEventArgs e)
        {
            string Name = ((RadioButton)sender).Name;
            Setting.TTS_Voice = Name;
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void TTS_Condition_TextChange(object sender, TextChangedEventArgs e)
        {
            string Name = ((TextBox)sender).Name;
            if (Name == "TTS_Min_Text") Setting.TTS_Min = (TTS_Min_Text.Text == "" ? 0 : int.Parse(TTS_Min_Text.Text));
            else if (Name == "TTS_Max_Text") Setting.TTS_Max = (TTS_Max_Text.Text == "" ? 0 : int.Parse(TTS_Max_Text.Text));
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void TTS_Condition_EnableChange(object sender, EventArgs e)
        {
            string Name = ((ToggleSwitch)sender).Name;
            if (Name == "TTS_Min_Switch") Setting.TTS_Min_Enable = (bool)TTS_Min_Switch.IsChecked;
            if (Name == "TTS_Max_Switch") Setting.TTS_Max_Enable = (bool)TTS_Max_Switch.IsChecked;
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void TTS_Content_Switch_IsCheckedChanged(object sender, EventArgs e)
        {
            Setting.TTS_Content_Enable = (bool)TTS_Content_Switch.IsChecked;
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void TTS_Effect_Find_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog GetFile = new OpenFileDialog();
            string TTS_Effect_List_String = null;
            GetFile.Filter = "소리 파일 (*.wav;*.mp3;*.wma;*.wmv) | *.wav;*.mp3;*.wma;*.wmv";
            GetFile.Title = "메세지를 읽기 전에 재생될 소리파일을 선택해 주세요.";
            GetFile.ShowDialog();

            if (GetFile.FileName != "")
            {
                if (File.Exists(GetFile.FileName))
                {
                    TTS_Effect_Sound_Combo.Items.Add(GetFile.FileName);
                    TTS_Effect_Sound_Combo.SelectedIndex = TTS_Effect_Sound_Combo.Items.Count - 1;
                    foreach(string Effect in TTS_Effect_Sound_Combo.Items)
                    {
                        TTS_Effect_List_String += Effect + "/";
                    }
                    TTS_Effect_List_String = TTS_Effect_List_String.Remove(TTS_Effect_List_String.Length-1,1);
                    Setting.TTS_Effect_List = TTS_Effect_List_String;
                    if (this.IsLoaded) Setting.SaveSetting();
                }
                else MessageBox.Show("...? 없는 파일이라는데요?");
            }
        }
        private void TTS_Effect_Sound_IsCheckedChanged(object sender, EventArgs e)
        {
            Setting.TTS_Effect_Enable = (bool)TTS_Effect_Sound.IsChecked;
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void TTS_Effect_Sound_Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Setting.TTS_Effect = TTS_Effect_Sound_Combo.SelectedIndex;
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void TTS_Filter_IsCheckedChanged(object sender, EventArgs e)
        {
            Setting.TTS_Filter = (bool)TTS_Filter_Switch.IsChecked;
            if (this.IsLoaded) Setting.SaveSetting();
        }

        private void FDM_Preview_Expander_Expanded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer PreviewTimer = new DispatcherTimer();
            PreviewTimer.Interval = TimeSpan.FromMilliseconds(1000);
            PreviewTimer.Tick += PreviewTimer_Tick;
            PreviewTimer.Start();
        }
        private void FDM_In_Effect_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Setting.FDM_Intro = ((FrameworkElement)(FDM_In_Effect_ComboBox.Items[FDM_In_Effect_ComboBox.SelectedIndex])).Name.Split('_')[2];
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void FDM_Out_Effect_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Setting.FDM_Outtro = ((FrameworkElement)(FDM_Out_Effect_ComboBox.Items[FDM_Out_Effect_ComboBox.SelectedIndex])).Name.Split('_')[2];
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void FDM_Effect_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Setting.FDM_Effect = ((FrameworkElement)(FDM_Effect_ComboBox.Items[FDM_Effect_ComboBox.SelectedIndex])).Name.Split('_')[2];
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void FDM_Get_Font_Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FontDialog GetFont = new System.Windows.Forms.FontDialog();
            if(GetFont.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("해당 폰트의 이름은 " + GetFont.Font.Name + " 입니다");
            }
        }
        private void FDM_Get_ColorCode_Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog GetColor = new System.Windows.Forms.ColorDialog();
            if(GetColor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MessageBox.Show("해당 컬러의 코드는 " +
                    GetColor.Color.R.ToString("X2") +
                    GetColor.Color.G.ToString("X2") +
                    GetColor.Color.B.ToString("X2")
                    + " 입니다");
            }
        }
        private void FDM_Content_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            Setting.FDM_Content = FDM_Content_Text.Text;
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void FDM_Enable_Switch_IsCheckedChanged(object sender, EventArgs e)
        {
            Setting.FDM_Enable = (bool)FDM_Enable_Switch.IsChecked;
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void FDM_Max_Switch_IsCheckedChanged(object sender, EventArgs e)
        {
            Setting.FDM_Max_Enable = (bool)FDM_Max_Switch.IsChecked;
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void FDM_Min_Switch_IsCheckedChanged(object sender, EventArgs e)
        {
            Setting.FDM_Min_Enable = (bool)FDM_Min_Switch.IsChecked;
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void FDM_Min_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            Setting.FDM_Min = int.Parse(FDM_Min_Text.Text);
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void FDM_Max_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            Setting.FDM_Max = int.Parse(FDM_Max_Text.Text);
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void FDM_Filter_IsCheckedChanged(object sender, EventArgs e)
        {
            Setting.FDM_Filter = (bool)FDM_Filter_Switch.IsChecked;
            if (this.IsLoaded) Setting.SaveSetting();
        }

        private void Youtube_Max_Switch_IsCheckedChanged(object sender, EventArgs e)
        {
            Setting.Youtube_Max_Enable = (bool)Youtube_Max_Switch.IsChecked;
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void Youtube_Min_Switch_IsCheckedChanged(object sender, EventArgs e)
        {
            Setting.Youtube_Min_Enable = (bool)Youtube_Min_Switch.IsChecked;
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void Youtube_Max_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            Setting.Youtube_Max = (Youtube_Max_Text.Text == "" ? 0 : int.Parse(Youtube_Max_Text.Text));
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void Youtube_Min_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            Setting.Youtube_Min = (Youtube_Min_Text.Text == "" ? 0 : int.Parse(Youtube_Min_Text.Text));
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void Youtube_Enable_Switch_IsCheckedChanged(object sender, EventArgs e)
        {
            Setting.Youtube_Enable = (bool)Youtube_Enable_Switch.IsChecked;
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void Youtube_Next_Switch_IsCheckedChanged(object sender, EventArgs e)
        {
            Setting.Youtube_Next_Enable = (bool)Youtube_Next_Switch.IsChecked;
            if (this.IsLoaded) Setting.SaveSetting();
        }
        private void Youtube_Filter_IsCheckedChanged(object sender, EventArgs e)
        {
            Setting.Youtube_Filter = (bool)Youtube_Filter_Switch.IsChecked;
            if (this.IsLoaded) Setting.SaveSetting();
        }

        private void Filter_Message_Button_Click(object sender, RoutedEventArgs e)
        {
            Filter_Message_ListView.Items.Add(Filter_Message_Text.Text);
            Setting.MessageFilter.Add(Filter_Message_Text.Text);
            Setting.SaveFilter();
            Filter_Message_Text.Text = "";
        }
        private void Filter_Potsu_Button_Click(object sender, RoutedEventArgs e)
        {
            Filter_Potsu_ListView.Items.Add(Filter_Potsu_Text.Text);
            Setting.PotsuFilter.Add(Filter_Potsu_Text.Text);
            Setting.SaveFilter();
            Filter_Potsu_Text.Text = "";
        }
        private void Filter_Youtube_Button_Click(object sender, RoutedEventArgs e)
        {
            Filter_Youtube_ListView.Items.Add(Filter_Youtube_Text.Text);
            Setting.YoutubeFilter.Add(Filter_Youtube_Text.Text);
            Setting.SaveFilter();
            Filter_Youtube_Text.Text = "";
        }
        private void Filter_Message_Text_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Filter_Message_Button_Click(Filter_Message_Text, null);
            }
        }
        private void Filter_Potsu_Text_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Filter_Potsu_Button_Click(Filter_Potsu_Text, null);
            }
        }
        private void Filter_Youtube_Text_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Filter_Youtube_Button_Click(Filter_Youtube_Text, null);
            }
        }
        private void Filter_Message_ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Filter_Message_ListView.SelectedIndex > -1)
            {
                Setting.MessageFilter.RemoveAt(Filter_Message_ListView.SelectedIndex);
                Filter_Message_ListView.Items.RemoveAt(Filter_Message_ListView.SelectedIndex);
                Setting.SaveFilter();
            }
        }
        private void Filter_Potsu_ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Filter_Potsu_ListView.SelectedIndex > -1)
            {
                Setting.MessageFilter.RemoveAt(Filter_Potsu_ListView.SelectedIndex);
                Filter_Potsu_ListView.Items.RemoveAt(Filter_Potsu_ListView.SelectedIndex);
                Setting.SaveFilter();
            }
        }
        private void Filter_Youtube_ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Filter_Youtube_ListView.SelectedIndex > -1)
            {
                Setting.MessageFilter.RemoveAt(Filter_Youtube_ListView.SelectedIndex);
                Filter_Youtube_ListView.Items.RemoveAt(Filter_Youtube_ListView.SelectedIndex);
                Setting.SaveFilter();
            }
        }
    }
}
