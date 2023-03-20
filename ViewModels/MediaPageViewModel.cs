using IWshRuntimeLibrary;
using Melista.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Melista.ViewModels
{
    public class MediaPageViewModel : BindableBase
    {
        private readonly PageService _pageService;
        public string MediaName { get; set; }

        public string MediaDur { get; set; }

        public string MaxDur { get; set; }

        public double SliderVal { get; set; }

        public TimeSpan TotalTime { get; set; }

        public MediaPageViewModel(PageService pageService)
        {
            
            _pageService = pageService;
            play = true;
            MediaName = Global.CurrentMedia.NameVideo;
            Player = new MediaElement()
            {
                LoadedBehavior = MediaState.Manual,
            };
            string path = GetPathFromLink(Global.CurrentMedia.Path);
            if (path != null)
            {


                Player.Source = new Uri(path);
                Player.Play();



                //TotalTime = Player.NaturalDuration.TimeSpan;

                //MessageBox.Show(TotalTime.ToString());
                //DispatcherTimer timerVideoTime = new DispatcherTimer();
                //timerVideoTime.Interval = TimeSpan.FromSeconds(1);
                //timerVideoTime.Tick += new EventHandler(timer_Tick);
                //timerVideoTime.Start();

            }
            
        }
        
        void timer_Tick(object sender, EventArgs e)
        {
            // Check if the movie finished calculate it's total time
            if (Player.NaturalDuration.TimeSpan.TotalSeconds > 0)
            {
                if (TotalTime.TotalSeconds > 0)
                {
                    // Updating time slider
                    SliderVal = Player.Position.TotalSeconds /
                                       TotalTime.TotalSeconds;
                    MediaDur = SliderVal.ToString();
                }
            }
        }
        public MediaElement Player { get; set; }
        public DelegateCommand Back => new(() =>
        {
            _pageService.ChangePage(new StartPageView());
        });
        public bool play { get; set; }
        public DelegateCommand PlayVideoCommand => new(() =>
        {
            if (!play)
            { 
                Player.Play();
                play = true;
            }
            else if(play)
            {
                Player.Pause();
                play = false;
            }
        });
        public DelegateCommand FastForward => new(() =>
        {
            MaxDur = Player.NaturalDuration.TimeSpan.TotalSeconds.ToString();
            TotalTime = Player.NaturalDuration.TimeSpan;
            Player.Position += TimeSpan.FromSeconds(10);
            SliderVal = Player.Position.TotalSeconds;
            MediaDur = SliderVal.ToString();
            
        });
        public DelegateCommand Rewind => new(() =>
        {
            Player.Position -= TimeSpan.FromSeconds(10);
        });

        public string GetPathFromLink(string linkPathName)
        {
            if (System.IO.File.Exists(linkPathName))
            {
                WshShell shell = new WshShell();
                IWshShortcut link = (IWshShortcut)shell.CreateShortcut(linkPathName);
                return link.TargetPath;
            }
            else
            {
                return null;
            }
        }
    }
}
