using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melista.Models
{
    public class Audio
    {
        public string NameAudio { get; set; }
        public string DescriptionAudio { get; set; }
        public DateTime DateAudio { get; set; }
        public int DurationAudio { get; set; }
        public int ViewsAudio { get; set; }
    }
}
