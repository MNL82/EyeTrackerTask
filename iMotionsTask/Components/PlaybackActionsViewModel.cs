using iMotionsTask.Services;
using iMotionsTaskLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace iMotionsTask.Components
{
    public class PlaybackActionsViewModel : INotifyPropertyChanged
    {
        public IEyeTrackerPlayer Player { get; private set; }
        public RelayCommand PlayCommand { get; private set; }

        public PlaybackActionsViewModel()
        {
            PlayCommand = new RelayCommand(OnPlay);

            // Return if in designer
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject())) { return; }

            Player = ContainerHelper.Container.Resolve<IEyeTrackerPlayer>();
            (Player as INotifyPropertyChanged).PropertyChanged += PlaybackActionsViewModel_PropertyChanged;
            
        }

        private string timeString;
        public string TimeString
        {
            get { return timeString; }
            private set
            {
                if (timeString != value)
                {
                    timeString = value;
                    OnPropertyChanged("TimeString");
                }
            }
        }

        private void PlaybackActionsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentTime")
            {
                TimeString = TimeSpan.FromMilliseconds(Player.CurrentTime).ToString(@"mm\:ss\.fff");
            }
        }

        public void OnPlay()
        {
            Player.Playing = !Player.Playing;
        }

        public bool CanPlay()
        {
            return Player.Data.Any();
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
