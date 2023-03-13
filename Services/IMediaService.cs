using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melista.Services
{
    public interface IMediaService
    {
        void Play();
        void Pause();
        void Stop();
        void Rewind();
        void FastForward();
    }
}
