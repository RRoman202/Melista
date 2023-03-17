using Melista.Models;
using System.Collections.ObjectModel;
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
            Player.Source = new Uri("C:\\Users\\Роман\\Videos\\osu!\\osu! 2022.10.16 - 22.50.55.01.mp4");
            Player.Play();
            
            
        });
    }
}
