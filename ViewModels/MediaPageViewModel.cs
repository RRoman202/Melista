using Melista.Models;
using System.Collections.ObjectModel;

namespace Melista.ViewModels
{
    public class MediaPageViewModel : BindableBase
    {
        private readonly PageService _pageService;
        public MediaPageViewModel(PageService pageService)
        {
            _pageService = pageService;
        }
        public DelegateCommand Back => new(() =>
        {
            _pageService.ChangePage(new StartPageView());
        });
    }
}
