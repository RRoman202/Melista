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
using LibVLCSharp.Shared;
using Melista.Services;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Melista.ViewModels
{
    public class MusicPageViewModel : BindableBase
    {
        private readonly PageService _pageService;

        private readonly WindowService _windowService;
        public string Speed { get; set; }

        public WaveOutEvent player { get; set; }
        public float VolumePosition { get; set; }
        DispatcherTimer timer = new DispatcherTimer();
        string[] PlayPauseImagePaths;
        public string MediaName { get; set; }
        public string DurText { get; set; } // Текст с отчётом времени {1:02}
        public double Position { get; set; } // Текущая позиция mediapleer(а) в секундах
        public double Duration { get; set; } // Длительность файла mediapleer(а) в секундах
        public string DurText2 { get; set; }
        public bool isPlaying { get; set; } = false;
        public Uri PlayPauseImage { get; set; }
        public MusicPageViewModel(PageService pageService, WindowService windowService)
        {

            _windowService = windowService;
            _pageService = pageService;
            PlayPauseImagePaths = new string[] { "Resources\\Icons\\play.png", "Resources\\Icons\\pause.png" };
            PlayPauseImage = new Uri(PlayPauseImagePaths[1], UriKind.Relative);
            

            SettingsVisibility = Visibility.Hidden;
            Speed = "1x";

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
            
            
            
            Duration = audioFile.TotalTime.TotalSeconds;
            DurText2 = String.Format("{0}", audioFile.TotalTime.ToString(@"mm\:ss"));
            VolumePosition = player.Volume;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();


        });
        public DelegateCommand SliderVolumeChanged => new(() =>
        {

            player.Volume = VolumePosition;
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
        bool thumbIsDraging = false;
        public DelegateCommand SliderDragStartedCommand => new(() =>
        {
            thumbIsDraging = true;
            player.Pause();
            timer.Stop();
        });

        public DelegateCommand SliderDragCompletedCommand => new(() =>
        {
            player = new WaveOutEvent();
            string path = GetPathFromLink(Global.CurrentAudio.Path);
            AudioFileReader audioFile = new AudioFileReader(path);
            audioFile.Position = (long)Position;
            player.Init(audioFile);

            thumbIsDraging = false;
            if (isPlaying)
            {
                player.Play();
                timer.Start();
            }
        });
        public DelegateCommand SliderValueChangedCommand => new(() =>
        {
            if (thumbIsDraging)
                DurText = String.Format("{0}", TimeSpan.FromSeconds(Position).ToString(@"mm\:ss"));
        });

        public Visibility SettingsVisibility { get; set; }
        public DelegateCommand NavigateCommand2 => new(() => SettingsIsVisibility());

        public void SettingsIsVisibility()
        {
            if (SettingsVisibility == Visibility.Hidden)
            {
                SettingsVisibility = Visibility.Visible;
            }
            else
            {
                SettingsVisibility = Visibility.Hidden;
            }
        }

    }
}