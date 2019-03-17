using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iMotionsTaskLib
{
    public interface IEyeTrackerPlayer
    {
        IEyeTrackerData Data { get; }

        bool Playing { get; set; }

        long FirstTime { get; }
        long LastTime { get; }
        long CurrentTime { get; set; }

        EyeTrackerRecord TimeRecord(long time);

        List<EyeTrackerRecord> GetBacktrace(long time);
    }
}
