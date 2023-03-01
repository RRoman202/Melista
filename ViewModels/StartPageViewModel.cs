namespace Melista.ViewModels
{
    public class StartPageViewModel : BindableBase
    {
        private readonly PageService _pageService;
        public StartPageViewModel(PageService pageService)
        {
            
            _pageService = pageService;
        }
    }
}
