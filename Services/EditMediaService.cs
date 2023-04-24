using Melista.Models;
using System.Threading.Tasks;
using Microsoft.Win32;
using TagLib;
using IWshRuntimeLibrary;
using System.Windows;

namespace Melista.Services
{
    public class EditMediaService
    {
        public async Task<Video> GetTagsVideo(string path)
        {
            TagLib.File Info = TagLib.File.Create(GetPathFromLink(path));

            Video video = new Video();

            
            await Task.Run(() =>
            {
                video = new Video {

                    NameVideo = Info.Tag.Title,
                    DescriptionVideo = Info.Tag.Description,
                    DurationVideo = ((int)Info.Properties.Duration.TotalSeconds),
                    

                };

            });

            return video;
        }



        public string GetPathFromLink(string linkPathName)
        {
            if (System.IO.File.Exists(linkPathName))
            {
                WshShell shell = new WshShell();
                IWshShortcut link = (IWshShortcut)shell.CreateShortcut(linkPathName);
                return link.TargetPath;
            }
            return null;
        }

    }
}
