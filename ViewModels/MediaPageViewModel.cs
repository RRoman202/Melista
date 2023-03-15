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
    }
}
