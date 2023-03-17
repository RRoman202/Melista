using Melista.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melista.Services
{
    public class MediaService
    {
        public async Task<ObservableCollection<Video>> GetMedia()
        {
            ObservableCollection<Video> medias = new();
            await Task.Run(async () =>
            {
                try
                {
                    var dir = new DirectoryInfo(Path.GetFullPath("Resources/ShortCuts").Replace(@"\bin\Debug\net7.0-windows\", @"\") + @"\");
                    FileInfo[] files = dir.GetFiles();
                    foreach(FileInfo f in files)
                    {
                        medias.Add(new Video { NameVideo = RemoveFormatString(f.Name), Path=f.FullName});
                        
                    }
                }
                catch { }
            });
            return medias;
        }
        public string RemoveFormatString(string stringForRemove)
        {

            if (stringForRemove.Contains('\\'))
            {
                string[] strings = stringForRemove.Split('\\');
                stringForRemove = strings[strings.Length - 1];
            }
            string[] strings_1 = stringForRemove.Split('.');
            return strings_1[0];
        }
    }
}
