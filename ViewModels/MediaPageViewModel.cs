using IWshRuntimeLibrary;
using Melista.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace Melista.ViewModels
{
    public class MediaPageViewModel : BindableBase
    {
        private readonly PageService _pageService;

        public MediaPageViewModel(PageService pageService)
        {
            _pageService = pageService;
            Player = new MediaElement()
            {
                LoadedBehavior = MediaState.Manual,
            };
        }
        public MediaElement Player { get; set; }
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
    }
}
