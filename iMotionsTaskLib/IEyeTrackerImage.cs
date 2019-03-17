using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iMotionsTaskLib
{
    public interface IEyeTrackerImage
    {
        string FilePath { get; set; }

        int Height { get; }
        int Width { get; }
    }
}
