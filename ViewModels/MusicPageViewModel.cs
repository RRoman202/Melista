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

namespace Melista.ViewModels
{
    public class MusicPageViewModel : BindableBase
    {
        private readonly PageService _pageService;

        private readonly WindowService _windowService;

        public MusicPageViewModel(PageService pageService, WindowService windowService)
        {

            _windowService = windowService;
            _pageService = pageService;
           

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
            WaveOutEvent player = new WaveOutEvent();
            string path = GetPathFromLink(Global.CurrentMedia.Path);
            AudioFileReader audioFile = new AudioFileReader(path);
            player.Init(audioFile);
            player.Play();//aa

        });
        
    }
}
