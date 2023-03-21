
﻿using Microsoft.Win32;
using System.Diagnostics;
using System.Threading.Tasks;

﻿using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;

using System.Windows;
using static System.Net.Mime.MediaTypeNames;
using Melista.Models;

using System.IO;

using IWshRuntimeLibrary;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.Linq;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Melista.ViewModels
{
    public class StartPageViewModel : BindableBase, IDropTarget
    {
        public ObservableCollection<Video> Medias { get; set; }

        private readonly PageService _pageService;
        private readonly MediaService _mediaService;
        public Video _selectedMedia { get; set; }
        public Video SelectedMedia
        {
            get { return _selectedMedia; }
            set {
                _selectedMedia = value;
                RaisePropertiesChanged(nameof(SelectedMedia));
                Global.CurrentMedia = SelectedMedia;

            }
        }

        public StartPageViewModel(PageService pageService, MediaService mediaService)
        {
            _pageService = pageService;
            _mediaService = mediaService;
            Task.Run(async () =>
            {
                
                Medias = await _mediaService.GetMedia();
               
            }).WaitAsync(TimeSpan.FromMilliseconds(10))
            .ConfigureAwait(false);
            
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;

            var dataObject = dropInfo.Data as IDataObject;

            dropInfo.Effects = dataObject != null && dataObject.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.Move;
        }

        public void Drop(IDropInfo dropInfo)
        {
            var dataObject = dropInfo.Data as DataObject;
            if (dataObject != null && dataObject.ContainsFileDropList())
            {
                var files = dataObject.GetFileDropList();
                foreach (var file in files)
                {
                    Medias.Add(new Video { NameVideo = RemoveFormatString(file) });
                    CreateShortCut(file, RemoveFormatString(file));
                    Process.Start(new ProcessStartInfo() { FileName = System.IO.Path.GetFullPath("Resources/ShortCuts").Replace(@"\bin\Debug\net7.0-windows\", @"\") + "\\" + RemoveFormatString(file), UseShellExecute = true });
                }

            }
        }
        public DelegateCommand LoadNewFile => new(() => LoadFile());

        public DelegateCommand GoVid => new(() =>
        {
            _pageService.ChangePage(new MediaPage());
        });

        public DelegateCommand ClickMedia => new(() =>
            _pageService.ChangePage(new MediaPage()));

        public void LoadFile() 
        {
            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.Filter = "Файлы mp3; mp4|*.mp3;*.mp4";
            OpenFile.Multiselect = true;
            if (OpenFile.ShowDialog() == true)
            {
                foreach (string file in OpenFile.FileNames)
                {
                    TagLib.File filik = TagLib.File.Create(file);
                    var mStream = new MemoryStream();


                    var firstPicture = filik.Tag.Pictures.FirstOrDefault();
                    if (firstPicture == null)
                    {
                        string kek = "C:\\Users\\petro\\Desktop\\aboba.png";
                        var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                        ffMpeg.GetVideoThumbnail(file, kek);
                        Bitmap btr = new Bitmap(kek);
                        filik.Tag.Pictures = new TagLib.IPicture[]
                        {
                            new TagLib.Picture(new TagLib.ByteVector((byte[])new System.Drawing.ImageConverter().ConvertTo(btr, typeof(byte[]))))
                        };
                        filik.Save();
                    }
                    BitmapImage bm = new BitmapImage();
                    if (filik.Tag.Pictures.Length >= 1)
                    {
                        var bin = (byte[])(filik.Tag.Pictures[0].Data.Data);
                        mStream.Write(bin, 0, Convert.ToInt32(bin.Length));
                        var bmp = new Bitmap(mStream, false);
                        mStream.Dispose();
                        bm = ConvertBit(bmp);
                    }
                    CreateShortCut(file, RemoveFormatString(file));
                    Medias.Add(new Video { NameVideo = RemoveFormatString(file), ImageVideo = bm});
                }
                
            }
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
        public void CreateShortCut(string Pathh, string shortPath) {

            WshShell shell = new WshShell();

            string shortcutPath = System.IO.Path.GetFullPath("Resources/ShortCuts").Replace(@"\bin\Debug\net7.0-windows\", @"\") + @"\" + shortPath + ".lnk";

            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = Pathh;

            shortcut.Save();

            Task.Run(async () =>
            {

                Medias = await _mediaService.GetMedia();

            }).WaitAsync(TimeSpan.FromMilliseconds(10))
            .ConfigureAwait(false);

        }

    }
}
