using Melista.Models;
using System.Windows;

namespace Melista.ViewModels
{
    public class EditMediaWindowViewModel : BindableBase
    {
        private readonly EditMediaService? _editMediaService;

        public Video Video { get; set; }

        public string Title { get; set; }

        public string Year { get; set; }

        public string Description { get; set; }

        public EditMediaWindowViewModel(EditMediaService editMediaService) 
        { 
            _editMediaService = editMediaService;

            GetVideo();
        }

        

        public async void GetVideo() 
        {
            Video = await _editMediaService.GetTagsVideo(Global.CurrentMedia.Path);

            Title = Video.NameVideo;
            Description = Video.DescriptionVideo;
        }
    }
}
