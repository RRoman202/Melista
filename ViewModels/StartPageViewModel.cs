
﻿using Microsoft.Win32;
using System.Diagnostics;
using System.Threading.Tasks;

﻿using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;

using System.Windows;
using static System.Net.Mime.MediaTypeNames;
using Melista.Models;

namespace Melista.ViewModels
{
    public class StartPageViewModel : BindableBase, IDropTarget
    {
        public ObservableCollection<Video> Medias { get; set; }
        private readonly PageService _pageService;
        public StartPageViewModel(PageService pageService)
        {
            _pageService = pageService;
            Medias = new ObservableCollection<Video>();
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

                }
            }
        }

        public DelegateCommand LoadNewFile => new(() => LoadFile());

        public void LoadFile() 
        {
            OpenFileDialog OpenFile = new OpenFileDialog();
            OpenFile.Filter = "Файлы mp3; mp4|*.mp3;*.mp4";
            OpenFile.Multiselect = true;
            if (OpenFile.ShowDialog() == true)
            {
                foreach (string file in OpenFile.FileNames)
                { 
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
    }
}
