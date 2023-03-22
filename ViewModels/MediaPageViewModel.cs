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

namespace Melista.ViewModels
{
    public class MediaPageViewModel : BindableBase
    {
        private readonly PageService _pageService;

        DispatcherTimer timer; // Таймер для сокрытия интерфейса
        TimeSpan PositonToPlayer { get; set; } // Для передачи в MediaElement
        public string MediaName { get; set; }
        public string DurText { get; set; } // Текст с отчётом времени {1:02}
        public double Position { get; set; } // Текущая позиция mediapleer(а) в секундах
        public double Duration { get; set; } // Длительность файла mediapleer(а) в секундах
        public bool isPlaying { get; set; }

        public MediaPageViewModel(PageService pageService)
        {
            timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 1) };
            timer.Tick += Timer_Tick;

            InterfaceVisible = Visibility.Hidden;
            _pageService = pageService;
            isPlaying = true;
            MediaName = Global.CurrentMedia.NameVideo;

            Player = new MediaElement()
            {
                LoadedBehavior = MediaState.Manual,
            };
            Player.MediaOpened += new RoutedEventHandler(MediaOpened());
            string path = GetPathFromLink(Global.CurrentMedia.Path);
            if (path != null)
            {
                Position = 0;
                Player.Source = new Uri(path);
                Player.Play();

            }

            DispatcherTimer timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromSeconds(1);
            timer2.Tick += timer_Tick2;
            timer2.Start();
        }

        public void MediaOpened()
        {
            MessageBox.Show("");
        }
        
        void timer_Tick2(object sender, EventArgs e)
        {
            if (Player.Source != null)
            {
                if (Player.NaturalDuration.HasTimeSpan)
                {
                    Duration = Player.NaturalDuration.TimeSpan.TotalSeconds;
                    DurText = String.Format("{0} / {1}", Player.Position.ToString(@"mm\:ss"), Player.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
                    if (isPlaying)
                    {
                        Position++;
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
                Player.Play();
                isPlaying = true;
            }
            else if(isPlaying)
            {
                Player.Pause();
                isPlaying = false;
            }
        });

        public DelegateCommand FastForward => new(() =>
        {
            Player.Position += TimeSpan.FromSeconds(10);
            Position = Player.Position.Seconds;
            DurText = String.Format("{0} / {1}", Player.Position.ToString(@"mm\:ss"), Player.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
        });

        public DelegateCommand Rewind => new(() =>
        {
            Player.Position -= TimeSpan.FromSeconds(10);
            Position = Player.Position.Seconds;
            DurText = String.Format("{0} / {1}", Player.Position.ToString(@"mm\:ss"), Player.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
        });

        public string GetPathFromLink(string linkPathName)
        {
            if (System.IO.File.Exists(linkPathName))
            {
                WshShell shell = new WshShell();
                IWshShortcut link = (IWshShortcut)shell.CreateShortcut(linkPathName);
                return link.TargetPath;
            }
            else
            {
                MessageBox.Show("Файл не найден");
            }
            return "";
        }

        public DelegateCommand NavigateCommand => new(() => InterfaceisVisible());

        int NavigateTimer = 0;
        public void InterfaceisVisible()
        {
            InterfaceVisible = Visibility.Visible;
            NavigateTimer = 2;
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            if (NavigateTimer == 0)
            {
                InterfaceVisible = Visibility.Hidden;
            }

            else
            {
                NavigateTimer--;
            }
        }
    }
}
