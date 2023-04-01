using IWshRuntimeLibrary;
using Melista.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;

using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.Threading.Tasks;
using System.Windows.Controls;

using System.Windows.Threading;
using System.Windows.Documents;
using DevExpress.Mvvm.POCO;
using Vlc.DotNet.Core.Interops.Signatures;
using Vlc.DotNet.Core;
using Vlc.DotNet.Wpf;
using FFmpeg.AutoGen;
using System.Drawing.Drawing2D;
using LibVLCSharp.Shared;
using System.Windows.Forms;

namespace Melista.ViewModels
{
    public class MediaPageViewModel : BindableBase
    {
        private readonly PageService _pageService;

        private readonly WindowService _windowService;
        public Uri PlayPauseImage { get; set; }
        TimeSpan PositonToPlayer { get; set; } // Для передачи в MediaElement
        public string MediaName { get; set; }
        public string DurText { get; set; } // Текст с отчётом времени {1:02}
        public double Position { get; set; } // Текущая позиция mediapleer(а) в секундах
        public double Duration { get; set; } // Длительность файла mediapleer(а) в секундах
        int NavigateTimer = 0; // Отсчёт таймера для сокрытия интерфейса
        DispatcherTimer timer; // Таймер для сокрытия интерфейса
        public string DurText2 { get; set; }

        DispatcherTimer timer2 = new DispatcherTimer();
        public bool isPlaying;

        string[] PlayPauseImagePaths;

        LibVLC _libVLC;
        LibVLCSharp.Shared.MediaPlayer _mediaPlayer;

        public MediaPageViewModel(PageService pageService, WindowService windowService)
        {
            string currentDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            var vlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));

            _windowService = windowService;

            timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 1) };
            timer.Tick += Timer_Tick;

            PlayPauseImagePaths = new string[] {"Resources\\Icons\\play.png", "Resources\\Icons\\pause.png"};
            PlayPauseImage = new Uri(PlayPauseImagePaths[1], UriKind.Relative);

            InterfaceVisible = Visibility.Hidden;
            _pageService = pageService;
            isPlaying = true;
            MediaName = Global.CurrentMedia.NameVideo;

            var options = new string[]
            {
                "--no-xlib",
                "--no-osd",
                "--no-video-title-show",
                "--vout=opengl",
                "--avcodec-hw=none"
                
            };
            Player = new VlcMediaPlayer(vlcLibDirectory, options);








            //Player.SourceProvider.CreatePlayer(vlcLibDirectory, options);

            //string path = GetPathFromLink(Global.CurrentMedia.Path);
            //if (path != null)
            //{
            //    Position = 0;
            //    Player.SourceProvider.MediaPlayer.SetMedia(new Uri(path), options);
            //    Player.SourceProvider.MediaPlayer.Play();



            //}

            
            //Player.SourceProvider.MediaPlayer.LengthChanged += MediaPlayer_LengthChanged;
            //Player.SourceProvider.MediaPlayer.EndReached += MediaEnded;
            
            
        }
        public DelegateCommand VideoLoaded => new(() =>
        {
            Core.Initialize();

            _libVLC = new LibVLC();
            Player = new LibVLCSharp.Shared.MediaPlayer(_libVLC);
            

            string path = GetPathFromLink(Global.CurrentMedia.Path);
            if (path != null)
            {
                Position = 0;

                Player.Play(new Media(_libVLC, new Uri(path)));
                
            }
            Player.Opening += MediaOpened;
            Player.LengthChanged += MediaLengthChanged;
            Player.EndReached += MediaEnded;

                
                
                
                Player.SetMedia(new Uri(path), options);
                Player.Play();
                
               


            }

            //Player.SourceProvider.MediaPlayer.Opening += MediaOpened;
            //Player.SourceProvider.MediaPlayer.LengthChanged += MediaPlayer_LengthChanged;
            //Player.SourceProvider.MediaPlayer.EndReached += MediaEnded;

            timer2.Interval = TimeSpan.FromSeconds(1);
            timer2.Tick += timer_Tick2;

            timer2.Start();



        private void MediaPlayer_LengthChanged(object? sender, VlcMediaPlayerLengthChangedEventArgs e)
        {
            
            //Duration = Player.SourceProvider.MediaPlayer.Length;
            //DurText2 = String.Format("{0}", TimeSpan.FromMilliseconds(Duration).ToString(@"mm\:ss"));
            


        });

        private void MediaEnded(object? sender, EventArgs e)
        {
            isPlaying = false;
            PlayPauseImage = new Uri(PlayPauseImagePaths[0], UriKind.Relative);
            Position = Duration;
        }

        private void MediaLengthChanged(object? sender, MediaPlayerLengthChangedEventArgs e)
        {

            Duration = Player.Length;
            DurText2 = String.Format("{0}", TimeSpan.FromMilliseconds(Duration).ToString(@"mm\:ss"));

           
            //DurText = String.Format("{0}", TimeSpan.FromMilliseconds(Position).ToString(@"mm\:ss"));
            //DurText2 = String.Format("{0}", TimeSpan.FromMilliseconds(Duration).ToString(@"mm\:ss"));
            //Player.SourceProvider.MediaPlayer.Time = Global.CurrentMedia.CurrentTime;
            

        }

        private void MediaOpened(object? sender, EventArgs e)
        {

            
            DurText = String.Format("{0}", TimeSpan.FromMilliseconds(Position).ToString(@"mm\:ss"));
            DurText2 = String.Format("{0}", TimeSpan.FromMilliseconds(Duration).ToString(@"mm\:ss"));
            Player.Time = Global.CurrentMedia.CurrentTime;
        }
        void timer_Tick2(object sender, EventArgs e)
        {

            if (Player != null)
            {

                Position = Player.Time;
                DurText = String.Format("{0}", TimeSpan.FromMilliseconds(Position).ToString(@"mm\:ss"));

                if (isPlaying)
                {
                    Position = Player.Time;
                }
            }
        }
        public LibVLCSharp.Shared.MediaPlayer Player { get; set; }
        
        
        public Visibility InterfaceVisible { get; set; }
        

        public DelegateCommand PlayVideoCommand => new(() =>
        {
            if (!isPlaying)
            {
                if (Position == Duration)
                {
                    Player.Time = 0;
                }
                Player.Play();
                isPlaying = true;
                PlayPauseImage = new Uri(PlayPauseImagePaths[1], UriKind.Relative);
            }
            else if (isPlaying)
            {
                PlayPauseImage = new Uri(PlayPauseImagePaths[0], UriKind.Relative);
                Player.Pause();
                isPlaying = false;
            }

            //isPlaying = false;
            //PlayPauseImage = new Uri(PlayPauseImagePaths[0], UriKind.Relative);
            //Position = Duration;
        }
        void timer_Tick2(object sender, EventArgs e)
        {
            
            //if (Player.SourceProvider.MediaPlayer != null)
            //{
                
            //    Position = Player.SourceProvider.MediaPlayer.Time;
            //    DurText = String.Format("{0}", TimeSpan.FromMilliseconds(Position).ToString(@"mm\:ss"));
                
            //    if (isPlaying)
            //    {
            //        Position = Player.SourceProvider.MediaPlayer.Time;

            //    }
            //}
        }
        public VlcMediaPlayer Player { get; set; }
        
        
        public Visibility InterfaceVisible { get; set; }
        public DelegateCommand Back => new(() =>
        {
            //Player.SourceProvider.MediaPlayer.Pause();
            //Task.Run(async () =>
            //{
            //    if (Player.SourceProvider.MediaPlayer != null)
            //    {
            //        Player.SourceProvider.MediaPlayer.Stop();
            //    }
            //}).WaitAsync(TimeSpan.FromMilliseconds(10))
            //.ConfigureAwait(false);
            //Player = new Vlc.DotNet.Wpf.VlcControl();
            //_pageService.ChangePage(new StartPageView());
        });

        public DelegateCommand PlayVideoCommand => new(() =>
        {
            //if (!isPlaying)
            //{
            //    if (Position == Duration)
            //    {
            //        Player.SourceProvider.MediaPlayer.Time = 0;
            //    }
            //    Player.SourceProvider.MediaPlayer.Play();
            //    isPlaying = true;
            //    PlayPauseImage = new Uri(PlayPauseImagePaths[1], UriKind.Relative);
            //}
            //else if (isPlaying)
            //{
            //    PlayPauseImage = new Uri(PlayPauseImagePaths[0], UriKind.Relative);
            //    Player.SourceProvider.MediaPlayer.Pause();
            //    isPlaying = false;
            //}

        });

        public DelegateCommand FastForward => new(() =>
        {

            Player.Time += 10000;
            Position = Player.Time;


            //Player.SourceProvider.MediaPlayer.Time += 10000;
            //Position = Player.SourceProvider.MediaPlayer.Time;
            


        });

        public DelegateCommand Rewind => new(() =>
        {

            if (Player.Time >= 10000)
            {
                Player.Time -= 10000;
                Position = Player.Time;
            }
            else
            {
                Player.Time = 0;
                Position = Player.Time;
            }

            //if(Player.SourceProvider.MediaPlayer.Time >= 10000)
            //{
            //    Player.SourceProvider.MediaPlayer.Time -= 10000;
            //    Position = Player.SourceProvider.MediaPlayer.Time;
            //}
            //else
            //{
            //    Player.SourceProvider.MediaPlayer.Time = 0;
            //    Position = Player.SourceProvider.MediaPlayer.Time;
            //}


        });

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

        public DelegateCommand NavigateCommand => new(() => InterfaceisVisible());

        public void InterfaceisVisible()
        {
            InterfaceVisible = Visibility.Visible;
            NavigateTimer = 2;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            if (NavigateTimer == 0 && !thumbIsDraging)
            {
                InterfaceVisible = Visibility.Hidden;
            }

            else
            {
                NavigateTimer--;
            }
        }

        bool thumbIsDraging = false;
        public DelegateCommand SliderDragStartedCommand => new(() =>
        {

            thumbIsDraging = true;
            Player.Pause();
            timer2.Stop();

            //thumbIsDraging = true;
            //Player.SourceProvider.MediaPlayer.Pause();
            //timer2.Stop();

        });

        public DelegateCommand SliderDragCompletedCommand => new(() =>
        {

            Player.Time = (long)Position;
            thumbIsDraging = false;
            if (isPlaying)
            {
                Player.Play();
                timer2.Start();
            }

            //Player.SourceProvider.MediaPlayer.Time = (long)Position;
            //thumbIsDraging = false;
            //if (isPlaying)
            //{
            //    Player.SourceProvider.MediaPlayer.Play();
            //    timer2.Start();
            //}

        });
        public DelegateCommand SliderValueChangedCommand => new(() =>
        {
            DurText = String.Format("{0}", TimeSpan.FromMilliseconds(Position).ToString(@"mm\:ss"));
            
        });
        
        public DelegateCommand FullScreen => new(() =>
        {

            Global.CurrentMedia.CurrentTime = Player.Time;
            Player.Pause();
            Task.Run(async () =>
            {
                if (Player != null)
                {
                    Player.Stop();
                }

            }).WaitAsync(TimeSpan.FromMilliseconds(10))
            .ConfigureAwait(false);


            _pageService.ChangePage(new FullScreenPage());


            //Global.CurrentMedia.CurrentTime = Player.SourceProvider.MediaPlayer.Time;
            //Player.SourceProvider.MediaPlayer.Pause();
            //Task.Run(async () =>
            //{
            //    if (Player.SourceProvider.MediaPlayer != null)
            //    {
            //        Player.SourceProvider.MediaPlayer.Stop();
            //    }
                    
            //}).WaitAsync(TimeSpan.FromMilliseconds(10))
            //.ConfigureAwait(false);
            //Player = new Vlc.DotNet.Wpf.VlcControl();

            //_pageService.ChangePage(new FullScreenPage());
            


        });

        public DelegateCommand MiniScreenCommand => new(() =>
        {

            Global.CurrentMedia.CurrentTime = Player.Time;
            Player.Pause();
            Task.Run(async () =>
            {
                if (Player != null)
                {
                    Player.Stop();
                }
            }).WaitAsync(TimeSpan.FromMilliseconds(10))
            .ConfigureAwait(false);


            _pageService.ChangePage(new MediaPage());

            //Global.CurrentMedia.CurrentTime = Player.SourceProvider.MediaPlayer.Time;
            //Player.SourceProvider.MediaPlayer.Pause();
            //Task.Run(async () =>
            //{
            //    if (Player.SourceProvider.MediaPlayer != null)
            //    {
            //        Player.SourceProvider.MediaPlayer.Stop();
            //    }
            //}).WaitAsync(TimeSpan.FromMilliseconds(10))
            //.ConfigureAwait(false);
            //Player = new Vlc.DotNet.Wpf.VlcControl();

            //_pageService.ChangePage(new MediaPage());

        });

        public DelegateCommand OpenEditMediaWindow => new(() =>
        {
            _windowService.Show<EditMediaWindow>(new EditMediaWindowViewModel());
        });
        
        public DelegateCommand Back => new(() =>
        {

            ThreadPool.QueueUserWorkItem(_ =>
            {
                Player.Pause();
                Player.Stop();
            });
            

            _pageService.ChangePage(new StartPageView());
        });
    }
}
