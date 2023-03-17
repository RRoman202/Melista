using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
