using System.Numerics;
using System.Windows;
using System.Windows.Threading;

namespace Melista.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private readonly PageService _pageService;

        public Page PageSource { get; set; }
        int NavigateTimer = 0; // Отсчёт таймера для сокрытия интерфейса
        DispatcherTimer timer; // Таймер для сокрытия интерфейса
        public MainViewModel(PageService pageService)
        {
            _pageService = pageService;
            _pageService.onPageChanged += (page) => PageSource = page;
            _pageService.ChangePage(new StartPageView());
            InterfaceVisible = 10;
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
        public int InterfaceVisible { get; set; }
        public DelegateCommand NavigateCommand => new(() => InterfaceisVisible());

        public void InterfaceisVisible()
        {
            InterfaceVisible = 32;
        }
        public DelegateCommand NavigateCommand2 => new(() => InterfaceisVisible2());

        public void InterfaceisVisible2()
        {
            InterfaceVisible = 10;
        }
    }
}
