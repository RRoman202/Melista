using Melista.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json;

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

            if (!File.Exists("theme.json"))
            {
                ResourceDictionary dict = new ResourceDictionary();
                dict.Source = new Uri(@"themes/dark.xaml", UriKind.Relative);

                Application.Current.Resources.MergedDictionaries[0] = dict;
                List<Theme> _theme = new List<Theme>();
                _theme.Add(new Theme()
                {
                    ActiveTheme = "Темная",
                });
                string conv = JsonConvert.SerializeObject(_theme);
                using (StreamWriter sw = new StreamWriter("theme.json", false))
                {
                    sw.Write(conv);
                }
            }
            if (File.Exists("theme.json"))
            {
                string theme = File.ReadAllText("theme.json");
                List<Theme> _theme = JsonConvert.DeserializeObject<List<Theme>>(theme);
                for (int i = 0; i < _theme.Count; i++)
                {
                    theme = _theme[i].ActiveTheme;
                    if (theme == "Светлая")
                    {
                        ResourceDictionary dict = new ResourceDictionary();
                        dict.Source = new Uri(@"themes/light.xaml", UriKind.Relative);

                        Application.Current.Resources.MergedDictionaries[0] = dict;
                    }
                    if (theme == "Темная")
                    {
                        ResourceDictionary dict = new ResourceDictionary();
                        dict.Source = new Uri(@"themes/dark.xaml", UriKind.Relative);

                        Application.Current.Resources.MergedDictionaries[0] = dict;
                    }
                }
            }
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
