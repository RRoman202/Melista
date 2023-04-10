
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

using System.Threading;

using System.Linq;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;
using Melista.Utils;
using System;

namespace Melista.ViewModels
{
    public class StartPageViewModel : BindableBase, IDropTarget
    {
        public ObservableCollection<Video> Medias { get; set; }

        public ObservableCollection<Audio> Audios { get; set; }

        private readonly PageService _pageService;
        private readonly MediaService _mediaService;

        public Visibility ProgVis { get; set; }
        public string ProgVal { get; set; }

        public Video _selectedMedia { get; set; }

        public Audio _selectedAudio { get; set; }

        public Audio SelectedAudio 
        { 
            get { return  _selectedAudio; }
            set
            {
                _selectedAudio = value;
                RaisePropertiesChanged(nameof(SelectedAudio));
                Global.CurrentAudio = SelectedAudio;
            }
        }
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
                ProgVis = Visibility.Visible;
                for(int i = 0; i < 101; i++)
                {
                    ProgVal = i.ToString();
                }
                Medias = await _mediaService.GetMedia();
                Audios = await _mediaService.GetAudios();
                ProgVis = Visibility.Hidden;
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
            int k = 0;
            var dataObject = dropInfo.Data as DataObject;
            if (dataObject != null && dataObject.ContainsFileDropList())
            {
                var files = dataObject.GetFileDropList();
                foreach (var file in files)
                {
                    if(Path.GetExtension(file) == ".mp3" || Path.GetExtension(file) == ".mp4")
                    {
                        bool isVideo = DefineFormatVideo(file);

                        TagLib.File filik = TagLib.File.Create(file);

                        var firstPicture = filik.Tag.Pictures.FirstOrDefault();
                        if (firstPicture == null)
                        {
                            Bitmap btr;
                            if (isVideo)
                            {
                                string kek = System.IO.Path.GetFullPath("aboba.jpeg");
                                var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                                Type shellAppType = Type.GetTypeFromProgID("Shell.Application");
                                dynamic shell = Activator.CreateInstance(shellAppType);
                                string dir = Path.GetDirectoryName(file);
                                string file2 = Path.GetFileName(file);
                                dynamic folder = shell.NameSpace(dir);
                                dynamic folderItem = folder.ParseName(file2);

                                string timee = folder.GetDetailsOf(folderItem, 27).ToString();
                                string[] times = timee.Split(":");

                                ffMpeg.GetVideoThumbnail(file, kek, (int.Parse(times[2]) / 2));
                                btr = new Bitmap(kek);

                            }
                            else
                            {
                                string zaglushka = System.IO.Path.GetFullPath("Resources/Images").Replace(@"\bin\Debug\net7.0-windows\", @"\") + @"\" + "zaglushka.png";
                                btr = new Bitmap(zaglushka);
                            }

                            filik.Tag.Pictures = new TagLib.IPicture[]
                            {
                            new TagLib.Picture(new TagLib.ByteVector((byte[])new ImageConverter().ConvertTo(btr, typeof(byte[]))))
                            };
                            filik.Save();
                        }
                        CreateShortCut(file, RemoveFormatString.RemoveFormat(file), isVideo);
                        k++;
                    }
                }
                if(k == 0)
                {
                    MessageBox.Show("Недопустимый формат файла");

                }
            }
        }
        public DelegateCommand LoadNewFile => new(() => LoadFile());

        public DelegateCommand GoVid => new(() =>
        {
            _pageService.ChangePage(new MediaPage());
        });

        public DelegateCommand ClickMedia => new(() =>
            _pageService.ChangePage(new MusicPage()));
        public DelegateCommand OpenProfile => new(() =>
        {
            _pageService.ChangePage(new ProfileView());
        });
        public void LoadFile() 
        {
            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.Filter = "Файлы mp3; mp4; mkv|*.mp3;*.mp4;*.mkv";
            OpenFile.Multiselect = true;
            if (OpenFile.ShowDialog() == true)
            {
                foreach (string file in OpenFile.FileNames)
                {

                    bool isVideo = DefineFormatVideo(file);

                    TagLib.File filik = TagLib.File.Create(file);

                    var firstPicture = filik.Tag.Pictures.FirstOrDefault();
                    if (firstPicture == null)
                    {
                        Bitmap btr;
                        if (isVideo)
                        {
                            string kek = System.IO.Path.GetFullPath("aboba.jpeg");
                            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                            Type shellAppType = Type.GetTypeFromProgID("Shell.Application");
                            dynamic shell = Activator.CreateInstance(shellAppType);
                            string dir = Path.GetDirectoryName(file);
                            string file2 = Path.GetFileName(file);
                            dynamic folder = shell.NameSpace(dir);
                            dynamic folderItem = folder.ParseName(file2);

                            string timee = folder.GetDetailsOf(folderItem, 27).ToString();
                            string[] times = timee.Split(":");

                            ffMpeg.GetVideoThumbnail(file, kek, (int.Parse(times[2]) / 2));
                            btr = new Bitmap(kek);

                        }
                        else 
                        {
                            string zaglushka = System.IO.Path.GetFullPath("Resources/Images").Replace(@"\bin\Debug\net7.0-windows\", @"\") + @"\" + "zaglushka.png";
                            btr = new Bitmap(zaglushka);
                        }
                        
                        filik.Tag.Pictures = new TagLib.IPicture[]
                        {
                            new TagLib.Picture(new TagLib.ByteVector((byte[])new ImageConverter().ConvertTo(btr, typeof(byte[]))))
                        };
                        filik.Save();
                    }
                    CreateShortCut(file, RemoveFormatString.RemoveFormat(file), isVideo);
                }
            }
        }
        public void CreateShortCut(string Pathh, string shortPath, bool isVideo) {

            WshShell shell = new WshShell();
            string shortcutPath;
            if (isVideo)
            {
                shortcutPath = System.IO.Path.GetFullPath("Resources/ShortCuts/Video").Replace(@"\bin\Debug\net7.0-windows\", @"\") + @"\" + shortPath + ".lnk";
            }
            else 
            { 
                shortcutPath = System.IO.Path.GetFullPath("Resources/ShortCuts/Audio").Replace(@"\bin\Debug\net7.0-windows\", @"\") + @"\" + shortPath + ".lnk";
            }
            

            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = Pathh;
            shortcut.Save();

            Task.Run(async () =>
            {
                if (isVideo)
                {
                    Medias = await _mediaService.GetMedia();
                }
                else 
                {
                    Audios = await _mediaService.GetAudios();
                }
                
            }).WaitAsync(TimeSpan.FromMilliseconds(10))
            .ConfigureAwait(false);
        }

        public bool DefineFormatVideo(string filename)
        {
            string[] strings = filename.Split('.');
            if (strings[strings.Length - 1] == "mp4")
                return true;
            return false;
        }
    }
}
