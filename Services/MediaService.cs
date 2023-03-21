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
                            var mStream = new MemoryStream();
                            var firstPicture = filik.Tag.Pictures.FirstOrDefault();
                            Debug.WriteLine("FirstFazaNice");
                            if (firstPicture == null)
                            {
                                
                                Debug.WriteLine("Kartinki net");
                                string kek = "C:\\Users\\petro\\Desktop\\aboba.png";
                                var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                                ffMpeg.GetVideoThumbnail(put, kek);
                                Bitmap btr = new Bitmap(kek);
                                filik.Tag.Pictures = new TagLib.IPicture[]
                                {
                                    new TagLib.Picture(new TagLib.ByteVector((byte[])new System.Drawing.ImageConverter().ConvertTo(btr, typeof(byte[]))))
                                };
                                filik.Save();
                            }
                            BitmapImage bm = new BitmapImage();
                            
                            if (firstPicture != null)
                            {
                                byte[] pData = firstPicture.Data.Data;
                                mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
                                var bmp = new Bitmap(mStream, false);
                                mStream.Dispose();
                                bm = ConvertBit(bmp);
                                bm.Freeze();

                            }
                            medias.Add(new Video { NameVideo = RemoveFormatString(f.Name), ImageVideo = bm, Path = f.FullName });
                        }
                        else
                        {
                            System.IO.File.Delete(f.FullName);
                        }
                        Debug.WriteLine("Разщмер медиаса - " + medias.Count);
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

        public BitmapImage ConvertBit(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}
