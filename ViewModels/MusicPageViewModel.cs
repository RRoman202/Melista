using Melista.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NAudio.Wave;
using IWshRuntimeLibrary;
using System.Threading;
using System.Windows.Threading;
using System.Numerics;
using NAudio.Utils;

namespace Melista.ViewModels
{
    public class MusicPageViewModel : BindableBase
    {
        private readonly PageService _pageService;

        private readonly WindowService _windowService;

        public WaveOutEvent player { get; set; }

        DispatcherTimer timer = new DispatcherTimer();
        string[] PlayPauseImagePaths;
        public string MediaName { get; set; }
        public string DurText { get; set; } // Текст с отчётом времени {1:02}
        public double Position { get; set; } // Текущая позиция mediapleer(а) в секундах
        public double Duration { get; set; } // Длительность файла mediapleer(а) в секундах
        public string DurText2 { get; set; }
        public bool isPlaying;
        public Uri PlayPauseImage { get; set; }
        public MusicPageViewModel(PageService pageService, WindowService windowService)
        {

            _windowService = windowService;
            _pageService = pageService;
            PlayPauseImagePaths = new string[] { "Resources\\Icons\\play.png", "Resources\\Icons\\pause.png" };
            PlayPauseImage = new Uri(PlayPauseImagePaths[1], UriKind.Relative);
            isPlaying = true;


        }
        public string GetPathFromLink(string linkPathName)
        {
            if (System.IO.File.Exists(linkPathName))
            {
                WshShell shell = new WshShell();
                IWshShortcut link = (IWshShortcut)shell.CreateShortcut(linkPathName);
                return link.TargetPath;
            }
            return null;
        }
        public DelegateCommand MusicLoaded => new(() =>
        {
            player = new WaveOutEvent();
            string path = GetPathFromLink(Global.CurrentAudio.Path);
            AudioFileReader audioFile = new AudioFileReader(path);
            player.Init(audioFile);
            player.Play();//aa
            Duration = audioFile.TotalTime.TotalSeconds;
            DurText2 = String.Format("{0}", audioFile.TotalTime.ToString(@"mm\:ss"));
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        });
        void timer_Tick(object sender, EventArgs e)
        {

            if (player != null)
            {
                DurText = String.Format("{0}", player.GetPositionTimeSpan().ToString(@"mm\:ss"));
                if (isPlaying)
                {
                    Position++;
                }
            }
        }
        public DelegateCommand PlayVideoCommand => new(() =>
        {
            if (!isPlaying)
            {
                player.Play();
                isPlaying = true;
                PlayPauseImage = new Uri(PlayPauseImagePaths[1], UriKind.Relative);
            }
            else if (isPlaying)
            {
                PlayPauseImage = new Uri(PlayPauseImagePaths[0], UriKind.Relative);
                player.Pause();
                isPlaying = false;
            }
        });
        public DelegateCommand Back => new(() =>
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                player.Pause();
                player.Stop();
            });
            _pageService.ChangePage(new StartPageView());
        });

    }
}
