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

        public MediaPageViewModel(PageService pageService, WindowService windowService)
        {

            _windowService = windowService;

            timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 1) };
            timer.Tick += Timer_Tick;

            PlayPauseImagePaths = new string[] {"Resources\\Icons\\play.png", "Resources\\Icons\\pause.png"};
            PlayPauseImage = new Uri(PlayPauseImagePaths[1], UriKind.Relative);

            InterfaceVisible = Visibility.Hidden;
            _pageService = pageService;
            isPlaying = true;
            MediaName = Global.CurrentMedia.NameVideo;

            Player = new MediaElement()
            {
                LoadedBehavior = MediaState.Manual,
            };
            Player.MediaOpened += MediaOpened;
            Player.MediaEnded += MediaEnded;

            string path = GetPathFromLink(Global.CurrentMedia.Path);
            if (path != null)
            {
                Position = 0;
                Player.Source = new Uri(path);
                Player.Play();
            }

            timer2.Interval = TimeSpan.FromSeconds(0.05);
            timer2.Tick += timer_Tick2;
            timer2.Start();
        }
        
        public void MediaOpened(object sender, RoutedEventArgs e)
        {
            Duration = Player.NaturalDuration.TimeSpan.TotalSeconds;
            DurText2 = String.Format("{0}", Player.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            Player.Position = Global.CurrentMedia.CurrentTime;
        }

        public void MediaEnded(object sender, RoutedEventArgs e)
        {
            isPlaying = false;
            PlayPauseImage = new Uri(PlayPauseImagePaths[0], UriKind.Relative);
        }
        void timer_Tick2(object sender, EventArgs e)
        {
            if (Player.Source != null)
            {
                if (Player.NaturalDuration.HasTimeSpan)
                {
                    Position = Player.Position.TotalSeconds;
                    DurText = String.Format("{0}", Player.Position.ToString(@"mm\:ss"));
                    if (isPlaying)
                    {
                        Position = Player.Position.TotalSeconds;
                        
                    }
                }
            }
        }
        public MediaElement Player { get; set; }
        public Visibility InterfaceVisible { get; set; }
        public DelegateCommand Back => new(() =>
        {
            _pageService.ChangePage(new StartPageView());
        });

        public DelegateCommand PlayVideoCommand => new(() =>
        {
            if (!isPlaying)
            { 
                if(Position == Duration)
                {
                    Player.Position = TimeSpan.FromSeconds(0);
                }
                Player.Play();
                isPlaying = true;
                PlayPauseImage = new Uri(PlayPauseImagePaths[1], UriKind.Relative);
            }
            else if(isPlaying)
            {
                PlayPauseImage = new Uri(PlayPauseImagePaths[0], UriKind.Relative);
                Player.Pause();
                isPlaying = false;
            }
        });

        public DelegateCommand FastForward => new(() =>
        {
            Player.Position += TimeSpan.FromSeconds(10);
            Position = Player.Position.TotalSeconds;
            DurText = String.Format("{0}", Player.Position.ToString(@"mm\:ss"));
            DurText2 = String.Format("{0}", Player.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));

        });

        public DelegateCommand Rewind => new(() =>
        {
            Player.Position -= TimeSpan.FromSeconds(10);
            Position = Player.Position.TotalSeconds;
            DurText = String.Format("{0}", Player.Position.ToString(@"mm\:ss"));
            DurText2 = String.Format("{0}", Player.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));

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
        });

        public DelegateCommand SliderDragCompletedCommand => new(() =>
        {
            Player.Position = TimeSpan.FromSeconds(Position);
            thumbIsDraging = false;
            if (isPlaying)
            {
                Player.Play();
                timer2.Start();
            }
        });
        public DelegateCommand SliderValueChangedCommand => new(() =>
        {
            DurText = String.Format("{0}", TimeSpan.FromSeconds(Position).ToString(@"mm\:ss"));
            if (!thumbIsDraging)
            {
                Player.Position = TimeSpan.FromSeconds(Position);
            }
        });
        public DelegateCommand FullScreen => new(() =>
        {
            Global.CurrentMedia.CurrentTime = TimeSpan.FromSeconds(Player.Position.TotalSeconds);
            _pageService.ChangePage(new FullScreenPage());
            
        });

        public DelegateCommand MiniScreenCommand => new(() =>
        {
            Global.CurrentMedia.CurrentTime = TimeSpan.FromSeconds(Player.Position.TotalSeconds);
            _pageService.ChangePage(new MediaPage());
            
        });

        public DelegateCommand OpenEditMediaWindow => new(() =>
        {
            _windowService.Show<EditMediaWindow>(new EditMediaWindowViewModel());
        });
           
    }
}
