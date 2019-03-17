using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iMotionsTaskLib
{
    public struct EyeTrackerRecord
    {
        public long t;
        public int leftX;
        public int leftY;
        public int rightX;
        public int rightY;

        public bool IsValid()
        {
            return leftX >= 0 &&
                   leftY >= 0 &&
                   rightX >= 0 &&
                   rightY >= 0;
        }
    }
}
