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

namespace Melista.ViewModels
{
    public class MusicPageViewModel : BindableBase
    {
        private readonly PageService _pageService;

        private readonly WindowService _windowService;
        public string Speed { get; set; }

        public WaveOutEvent player { get; set; }

        public MusicPageViewModel(PageService pageService, WindowService windowService)
        {

            _windowService = windowService;
            _pageService = pageService;
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
            player.Play();//aa

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

        public Visibility SettingsVisibility { get; set; }
        public DelegateCommand NavigateCommand2 => new(() => SettingsIsVisibility());

        public void SettingsIsVisibility()
        {
            if(SettingsVisibility == Visibility.Hidden)
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
