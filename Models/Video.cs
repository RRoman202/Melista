using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Melista.Models
{
    public class Video
    {
        public string NameVideo { get; set; }
        public string DescriptionVideo { get; set; }
        public DateTime DateVideo { get; set; }
        public int DurationVideo { get; set; }
        public int ViewsVideo { get; set; }
        public string Path { get; set; }
        public BitmapImage ImageVideo { get; internal set; }
        public TimeSpan CurrentTime { get; set; }
    }
}
