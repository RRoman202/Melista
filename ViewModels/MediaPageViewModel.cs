using IWshRuntimeLibrary;
using Melista.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Melista.ViewModels
{
    public class MediaPageViewModel : BindableBase
    {
        private readonly PageService _pageService;
        DispatcherTimer timer;
        public MediaPageViewModel(PageService pageService)
        {
            timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 1) }; // 1 секунда
            timer.Tick += Timer_Tick;

            _pageService = pageService;
            InterfaceVisible = Visibility.Hidden;
            Player = new MediaElement()
            {
                LoadedBehavior = MediaState.Manual,
            };
            string path = GetPathFromLink(Global.CurrentMedia.Path);
            if (path != null)
            {
                Player.Source = new Uri(path);
                Player.Play();
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
            string path = GetPathFromLink(Global.CurrentMedia.Path);
            if(path != null)
            {
                Player.Source = new Uri(path);
                Player.Play();
            }
            else
            {
                MessageBox.Show("Файл недопустим");
            }
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
                return null;
            }
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
