namespace Melista.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly PageService _pageService;

        public Page PageSource { get; set; }

        public MainViewModel(PageService pageService)
        {
            _pageService = pageService;
            _pageService.onPageChanged += (page) => PageSource = page;
            _pageService.ChangePage(new StartPageView());
        }
    }
}
