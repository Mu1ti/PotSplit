using System;
using System.Speech.Synthesis;
using System.Threading;
using System.Web;
using System.Media;
using WMPLib;
using System.IO;

namespace PotSplit
{
    class TTSEngine
    {
        private Thread MainThread = null;
        private SpeechSynthesizer HeamiTTS = null;
        private WindowsMediaPlayer GoogleTTS = null;
        private SoundPlayer EffectSound = null;
        private bool IsPlaying = false;

        public TTSEngine()
        {
            GoogleTTS = new WindowsMediaPlayer();
            EffectSound = new SoundPlayer();
            GoogleTTS.PlayStateChange += WMP_EndedDetect;
            HeamiTTS = new SpeechSynthesizer();

            GoogleTTS.URL = "";
            MainThread = new Thread(TTSDetector);
            MainThread.IsBackground = true;
        }

        //************************************************************
        //*                  TTS Thread Functions                    *
        //************************************************************

        public void StartDetect()
        {
            MainThread.Start();
        }
        public void StopDetect()
        {
            MainThread.Abort();
        }
        private void TTSDetector()
        {
            string Content = null;
            bool DoRead = true;

            while (true)
            {
                if(Setting.TTS_Enable == true)
                {
                    if (Donators.TTSWaitingList.Count > 0 && !IsPlaying)
                    {
                        if (Setting.TTS_Min_Enable == true && int.Parse(Donators.TTSWaitingList[0][1]) < Setting.TTS_Min) { DoRead = false; }
                        if (Setting.TTS_Max_Enable == true && int.Parse(Donators.TTSWaitingList[0][1]) > Setting.TTS_Max) { DoRead = false; }
                        if (DoRead)
                        {
                            IsPlaying = true;
                            Content = Setting.TTS_Content;
                            Content = Utility.TagConverter(Content, Donators.TTSWaitingList[0]);
                            TTS_Effect();
                            TTSSpeach(Content);
                            Donators.TTSWaitingList.RemoveAt(0);
                        }
                        else Donators.TTSWaitingList.RemoveAt(0);
                    }
                }
                else Donators.TTSWaitingList.Clear();
                Thread.Sleep(10);
            }
        }
        private void WMP_EndedDetect(int NewState)
        {
            if(NewState == 8)
            {
                IsPlaying = false;
            }
            if(GoogleTTS.currentMedia.duration > 0)
            {
                if(Donators.FDMWaitingList.Count > 0)
                {
                    Donators.FDMWaitingList[0][3] = ((int)GoogleTTS.currentMedia.duration).ToString();
                }
            }
        }
        public void TTS_Effect()
        {
            string[] Effects = Setting.TTS_Effect_List.Split('/');
            int Effect = Setting.TTS_Effect;
            if(Setting.TTS_Effect_Enable == true && File.Exists(Effects[Effect]) )
            {
                EffectSound = new SoundPlayer(Effects[Effect]);
                EffectSound.Play();
            }
            IsPlaying = false;
        }

        //************************************************************
        //*                TTS voice speach functions                *
        //************************************************************

        public void TTSSpeach(string Comment)
        {
            while(true)
            {
                if(!IsPlaying)
                {
                    IsPlaying = true;
                    if (Setting.TTS_Voice == "TTS_Voice_Heami") HeamiSpeach(Comment);
                    else if (Setting.TTS_Voice == "TTS_Voice_Google") GoogleSpeach(Comment);
                    else if (Setting.TTS_Voice == "TTS_Voice_Randome") RandomeSpeach(Comment);

                    break;
                }
                Thread.Sleep(100);
            }

        }
        private void GoogleSpeach(string Text)
        {
            Text = "http://translate.google.com/translate_tts?client=tw-ob&ie=UTF-8&tl=Ko-kr&q=" + HttpUtility.UrlEncode(Text);
            GoogleTTS.URL = Text;
        }
        private void HeamiSpeach(string Text)
        {
            HeamiTTS.Speak(Text);
            IsPlaying = false;
        }
        private void RandomeSpeach(string Text)
        {
            Random r = new Random();
            int SelectVoice = r.Next(0, 2);

            if (SelectVoice == 0) HeamiSpeach(Text);
            else if (SelectVoice == 1) GoogleSpeach(Text);
        }
    }
}
