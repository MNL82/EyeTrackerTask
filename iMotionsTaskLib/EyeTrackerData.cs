using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iMotionsTaskLib
{
    public class EyeTrackerData : IEyeTrackerData, INotifyPropertyChanged
    {
        private List<EyeTrackerRecord> recordList = new List<EyeTrackerRecord>();

        public int Count { get { return recordList.Count; } }

        public bool Any() { return recordList.Any(); }
        public EyeTrackerRecord First() { return recordList.Any() ? recordList.First() : new EyeTrackerRecord(); }
        public EyeTrackerRecord Last() { return recordList.Any() ? recordList.Last() : new EyeTrackerRecord(); }

        public void Add(EyeTrackerRecord record)
        {
            recordList.Add(record);
            OnPropertyChanged("Count");
        }

        public void Clear()
        {
            if (recordList.Any())
            {
                recordList.Clear();
                OnPropertyChanged("Count");
            }
        }

        public EyeTrackerRecord this[int i]
        {
            get { return recordList[i]; }
        }

        public IEnumerable<EyeTrackerRecord> Enum()
        {
            return recordList;
        }

        // Declare the event
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
