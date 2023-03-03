using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melista.Models
{
    public class Playlist
    {
        public string NamePlaylist { get; set; }
        public string DescriptionPlaylist { get; set; }
        public List<Audio> AudioPlaylist { get; set; }
        public List<Video> VideoPlaylist { get; set; }
        public DateTime DatePlaylist { get; set; }
        public int ViewsPlaylist { get; set; }
    }
}
