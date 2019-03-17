using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iMotionsTaskLib
{
    public class EyeTrackerPlayer : IEyeTrackerPlayer, INotifyPropertyChanged
    {

        public IEyeTrackerData Data { get; private set; }

        public EyeTrackerPlayer(IEyeTrackerData data)
        {
            this.Data = data;

            (data as INotifyPropertyChanged).PropertyChanged += EyeTrackerPlayer_PropertyChanged;
        }

        private void EyeTrackerPlayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Count")
            {
                if (Data.Count <= 1) {
                    OnPropertyChanged("FirstTime");
                    OnPropertyChanged("LastTime");
                    OnPropertyChanged("CurrentTime");
                } else
                {
                    OnPropertyChanged("LastTime");
                }
            }
        }

        public int UpdateDelay { get; set; } = 10;

        private bool playing = false;
        public bool Playing
        {
            get { return playing; }
            set
            {
                if (playing != value)
                {
                    if (value)
                    {
                        // Can not start play if no data
                        if (!Data.Any()) { return; }

                        // Can not play if at the end of the playback
                        long timeLeft = LastTime - beginTime;
                        if (timeLeft == 0) { return; }

                        // Start the playback
                        playing = value;

                        // Used to calculate the current time
                        playingStartRealTime = DateTime.Now;

                        //Console.WriteLine("Playing: " + CurrentTime);

                        // Start task to trigger CurrentTime changed as long as playing
                        Task.Delay(UpdateDelay).ContinueWith(t => TaskCurrentTimeChanged());

                        // Start task to end playback at LastTime
                        Task.Delay((int)timeLeft).ContinueWith(t => TaskPlaybackEnd());
                    }
                    else
                    {
                        beginTime = CurrentTime;
                        playing = value;
                        OnPropertyChanged("CurrentTime");
                    }
                    Console.WriteLine("Playing changed");
                    OnPropertyChanged("Playing");
                }
            }
        }

        private void TaskCurrentTimeChanged()
        {
            if (playing && CurrentTime != LastTime) {
                Task.Delay(UpdateDelay).ContinueWith(t => TaskCurrentTimeChanged());
            }
            //Console.WriteLine("Time Changed: " + CurrentTime);
            OnPropertyChanged("CurrentTime");
        }

        private void TaskPlaybackEnd()
        {
            // return if no longer playing
            if (!Playing) { return; }

            long timeLeft = LastTime - CurrentTime;
            if (timeLeft == 0)
            {
                // Stop the playback
                Playing = false;
            }
            else
            {
                // Not yet done. Create a new EndOfPlayback event
                Task.Delay((int)timeLeft).ContinueWith(t => TaskPlaybackEnd());
            }
        }

        private long backtraceTime = 5000; // Display eye trace 5 sec back as default
        public long BacktraceTime
        {
            get { return backtraceTime; }
            set
            {
                if (backtraceTime != value)
                {
                    backtraceTime = value;
                    OnPropertyChanged("BacktraceTime");
                }
            }
        }

        // If the time between records are larger than the break time they will not be connected
        public long BreakTime { get; set; } = 500; // Default is 0.5 sec

        // First and last time in the data records
        public long FirstTime { get { return (Data.Any()) ? Data.First().t : 0; } }
        public long LastTime { get { return (Data.Any()) ? Data.Last().t : 0; } }

        // beginTime is the time where the playback begins
        private long beginTime = 0;

        private DateTime playingStartRealTime;
        
        public long CurrentTime
        {
            get
            {
                if (!Data.Any()) { return 0; }
                if (Playing)
                {
                    long timePlayed = (long)(DateTime.Now - playingStartRealTime).TotalMilliseconds;
                    return Math.Min(LastTime, beginTime + timePlayed);
                }
                else
                {
                    return beginTime;
                }
            }
            set
            {
                // Can not set time if no data is loaded
                if (!Data.Any()) { return; }

                // Limit the current time to the rage of the data
                value = Math.Max(FirstTime, value);
                value = Math.Min(LastTime, value);

                // Do nothing if the data is unchanged
                if (!Playing && beginTime == value) { return; }

                if (Playing)
                {
                    playingStartRealTime = DateTime.Now;
                }
                beginTime = value;
                OnPropertyChanged("CurrentTime");
            }
        }

        public List<EyeTrackerRecord> GetBacktrace(long time)
        {
            var recordList = new List<EyeTrackerRecord>();
            if (time < FirstTime || time > LastTime) { return recordList; }

            foreach (var r in Data.Enum())
            {
                if (r.t >= time)
                {
                    if (r.t == time) {
                        recordList.Add(r);
                        return recordList;
                    }
                    long delta = r.t - recordList.Last().t;
                    if (delta >= BreakTime) {
                        return recordList;
                    }

                    // Calculate the average weighted value beetween the two nearest points
                    double factorBefore = ((double)(time - recordList.Last().t)) / (double)delta;
                    double factorAfter = 1.0 - factorBefore;
                    EyeTrackerRecord record;
                    record.t = time;
                    record.leftX = (int)Math.Round(factorBefore * recordList.Last().leftX + factorAfter * r.leftX);
                    record.leftY = (int)Math.Round(factorBefore * recordList.Last().leftY + factorAfter * r.leftY);
                    record.rightX = (int)Math.Round(factorBefore * recordList.Last().rightX + factorAfter * r.rightX);
                    record.rightY = (int)Math.Round(factorBefore * recordList.Last().rightY + factorAfter * r.rightY);
                    recordList.Add(record);
                    return recordList;
                }
                recordList.Add(r);
            }


            return recordList;
        }

        public EyeTrackerRecord TimeRecord(long time)
        {
            if (time <= FirstTime || time >= LastTime)
            {
                if (time == FirstTime) { return Data.First(); }
                if (time == LastTime) { return Data.Last(); }

                EyeTrackerRecord record;
                record.t = time;
                record.leftX = -1;
                record.leftY = -1;
                record.rightX = -1;
                record.rightY = -1;
                return record;
            }

            EyeTrackerRecord recordBefore = Data.First();
            EyeTrackerRecord recordAfter;
            foreach(var r in Data.Enum())
            {
                if (r.t >= time)
                {
                    if (r.t == time) { return r; }
                    recordAfter = r;
                    long delta = recordAfter.t - recordBefore.t;
                    if (delta >= BreakTime) { break; }

                    // Calculate the average weighted value beetween the two nearest points
                    double factorBefore = ((double)(time - recordBefore.t)) / (double)delta;
                    double factorAfter = 1.0 - factorBefore;
                    EyeTrackerRecord record;
                    record.t = time;
                    record.leftX = (int)Math.Round(factorBefore * recordBefore.leftX  + factorAfter * recordAfter.leftX);
                    record.leftY = (int)Math.Round(factorBefore * recordBefore.leftY + factorAfter * recordAfter.leftY);
                    record.rightX = (int)Math.Round(factorBefore * recordBefore.rightX + factorAfter * recordAfter.rightX);
                    record.rightY = (int)Math.Round(factorBefore * recordBefore.rightY + factorAfter * recordAfter.rightY);
                    return record;
                }
                recordBefore = r;
            }
            
            recordAfter.t = time;
            recordAfter.leftX = -1;
            recordAfter.leftY = -1;
            recordAfter.rightX = -1;
            recordAfter.rightY = -1;
            return recordAfter;
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
