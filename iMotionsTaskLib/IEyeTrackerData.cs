using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iMotionsTaskLib
{
    public interface IEyeTrackerData
    {
        void Add(EyeTrackerRecord record);

        int Count { get; }

        bool Any();
        EyeTrackerRecord First();
        EyeTrackerRecord Last();

        IEnumerable<EyeTrackerRecord> Enum();
    }
}
