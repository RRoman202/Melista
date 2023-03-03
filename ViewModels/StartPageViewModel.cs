using Microsoft.Win32;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace Melista.ViewModels
{
    public class StartPageViewModel : BindableBase
    {
        private readonly PageService _pageService;
        public StartPageViewModel(PageService pageService)
        {
            _pageService = pageService;
        }
        public AsyncCommand LoadNewFileCommand => new(async () =>
        {
            await Task.Run(async () =>
            {
                OpenFileDialog OpenFile = new OpenFileDialog();
                OpenFile.Filter = "Файлы mp3; mp4|*.mp3;*.mp4";
                if (OpenFile.ShowDialog() == true)
                {
                    Process.Start(new ProcessStartInfo() { FileName = OpenFile.FileName, UseShellExecute = true });
                }
            });
        }, bool () => true);
        
       
    }
}
