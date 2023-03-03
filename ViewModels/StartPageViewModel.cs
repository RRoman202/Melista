using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using System.Windows;

namespace Melista.ViewModels
{
    public class StartPageViewModel : BindableBase, IDropTarget
    {
        public ObservableCollection<string> Medias { get; set; }
        private readonly PageService _pageService;
        public StartPageViewModel(PageService pageService)
        {
            
            _pageService = pageService;
            Medias = new ObservableCollection<string>();
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
                    Medias.Add(file);

                }
            }
        }
    }
}
