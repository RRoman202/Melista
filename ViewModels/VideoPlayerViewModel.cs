using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Melista.ViewModels
{
    public class VideoPlayerViewModel : BindableBase
    {
        public Uri Media { get; set; }
        private readonly PageService _pageService;
        public VideoPlayerViewModel(PageService pageService)
        {
            Global.CurrentMedia = "C:\\Users\\Дияз\\Desktop\\Мемы\\Maxwell Frontiers.mp4";
            _pageService = pageService;
        }

        public DelegateCommand LoadNewFile => new(() => Start());

        public void Start()
        {
            Media = new Uri(Global.CurrentMedia, UriKind.Absolute);
        }
    }

}

