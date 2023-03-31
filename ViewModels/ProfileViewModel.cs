using BlurryControls.Properties;
using Melista.Models;
using Melista.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;

namespace Melista.ViewModels
{
    public class ProfileViewModel : BindableBase
    {
        private readonly PageService _pageService;

        public List<string> Themes { get; set; } = new() { "Светлая", "Темная" };
        public string SelectedTheme
        {
            get { return GetValue<string>(); }
            set { SetValue(value, changedCallback: UpdateTheme); }
        }
        public ProfileViewModel(PageService pageService)
        {
            _pageService = pageService;
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
                        dict.Source = new Uri(@"style/light.xaml", UriKind.Relative);

                        Application.Current.Resources.MergedDictionaries[0] = dict;
                        SelectedTheme = "Светлая";
                    }
                    if (theme == "Темная")
                    {
                        ResourceDictionary dict = new ResourceDictionary();
                        dict.Source = new Uri(@"style/dark.xaml", UriKind.Relative);

                        Application.Current.Resources.MergedDictionaries[0] = dict;
                        SelectedTheme = "Темная";
                    }
                }
            }
        }
        private async void UpdateTheme()
        { 
            if (SelectedTheme == "Светлая")
            {
                ResourceDictionary dict = new ResourceDictionary();
                dict.Source = new Uri(@"style/light.xaml", UriKind.Relative);

                Application.Current.Resources.MergedDictionaries[0] = dict;
                List<Theme> _theme = new List<Theme>();
                _theme.Add(new Theme()
                {
                    ActiveTheme = "Светлая",
                });
                string conv = JsonConvert.SerializeObject(_theme);
                using (StreamWriter sw = new StreamWriter("theme.json", false))
                {
                    sw.Write(conv);
                }
            }
            if (SelectedTheme == "Темная")
            {
                ResourceDictionary dict = new ResourceDictionary();
                dict.Source = new Uri(@"style/dark.xaml", UriKind.Relative);

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
        }

        public DelegateCommand GoToStartPage => new(() =>
        {
            _pageService.ChangePage(new StartPageView());
        });
    }
}
