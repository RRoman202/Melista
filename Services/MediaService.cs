using IWshRuntimeLibrary;
using Melista.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

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
                        
                        if (CheckLink(f.FullName) == true)
                        {
                            string put = GetPathFromLink(f.FullName);

                            TagLib.File filik = TagLib.File.Create(put);

                            var firstPicture = filik.Tag.Pictures.FirstOrDefault();
                            BitmapImage bm = new BitmapImage();
                            if (filik.Tag.Pictures.Length >= 1)
                            {
                                var bin = (byte[])(filik.Tag.Pictures[0].Data.Data);
                                bm.BeginInit();
                                bm.StreamSource = new MemoryStream(bin);
                                bm.DecodePixelHeight = 240;
                                bm.EndInit();
                                bm.Freeze();

                            }
                            
                            medias.Add(new Video { NameVideo = RemoveFormatString(f.Name), ImageVideo = bm, Path = f.FullName });
                        }
                        else
                        {
                            System.IO.File.Delete(f.FullName);
                        }
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
        bool CheckLink(string linkPathName)
        {
            if (System.IO.File.Exists(linkPathName))
            {
                WshShell shell = new WshShell();
                IWshShortcut link = (IWshShortcut)shell.CreateShortcut(linkPathName);
                if (System.IO.File.Exists(link.TargetPath))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        string GetPathFromLink(string path) 
        {
            WshShell shell = new WshShell();
            IWshShortcut link = (IWshShortcut)shell.CreateShortcut(path);
            return link.TargetPath;
        }
    }
}
