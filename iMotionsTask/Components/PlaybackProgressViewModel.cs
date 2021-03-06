﻿using iMotionsTask.Services;
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
    public class PlaybackProgressViewModel : INotifyPropertyChanged
    {
        public IEyeTrackerPlayer Player { get; set; }

        public PlaybackProgressViewModel()
        {
            // Return if in designer
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject())) { return; }

            Player = ContainerHelper.Container.Resolve<IEyeTrackerPlayer>();
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
