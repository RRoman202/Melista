﻿using Melista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melista
{
    public static class Global
    {
        public static Video CurrentMedia { get; set; } = new Video();
        public static List<string> MediaList { get; set; } = new List<string>();
        public static Audio CurrentAudio { get; set; } = new Audio();
    }
}
