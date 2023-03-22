
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

namespace Melista.ViewModels
{
    public class StartPageViewModel : BindableBase, IDropTarget
    {
        public ObservableCollection<Video> Medias { get; set; }

        private readonly PageService _pageService;
        private readonly MediaService _mediaService;

        public Visibility ProgVis { get; set; }
        public string ProgVal { get; set; }

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
                ProgVis = Visibility.Visible;
                for(int i = 0; i < 101; i++)
                {
                    ProgVal = i.ToString();
                }
                Medias = await _mediaService.GetMedia();
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
                        Medias.Add(new Video { NameVideo = RemoveFormatString(file) });
                        CreateShortCut(file, RemoveFormatString(file));
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
                    CreateShortCut(file, RemoveFormatString(file));
                    Medias.Add(new Video { NameVideo = RemoveFormatString(file) });
                }
                
            }
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

            string shortcutPath = Path.GetFullPath("Resources/ShortCuts").Replace(@"\bin\Debug\net7.0-windows\", @"\") + @"\" + shortPath + ".lnk";

            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

            shortcut.Description = "Ярлык для текстового редактора";

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
