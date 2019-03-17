using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iMotionsTaskLib
{
    public interface IEyeTrackerReader
    {
        string FilePath { get; set; }
        bool ParseFile();
    }
}
