using System.Numerics;
using System.Windows;

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
        public DelegateCommand CloseCommand => new (() =>{
            Application.Current.MainWindow.Close();
        }); 
        public DelegateCommand MinimizeCommand => new (() =>{
            if (Application.Current.MainWindow.WindowState.ToString() == "Maximized")
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
        }); 
        public DelegateCommand RollupCommand => new (() =>{
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        });
    }
}
