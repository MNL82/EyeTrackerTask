using iMotionsTask.Services;
using iMotionsTaskLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Unity;

namespace iMotionsTask.Components
{
    public class MyPoint
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public Visibility V { get; set; } = Visibility.Hidden;
    }


    public class EyeTrackingCanvasViewModel : INotifyPropertyChanged
    {
        public IEyeTrackerImage BackgroundImage { get; private set; }
        public IEyeTrackerPlayer Player { get; private set; }

        public ObservableCollection<MyPoint> Items { get; private set; } = new ObservableCollection<MyPoint>();


        public EyeTrackingCanvasViewModel()
        {
            // Return if in designer
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject())) { return; }
            BackgroundImage = ContainerHelper.Container.Resolve<IEyeTrackerImage>();
            Player = ContainerHelper.Container.Resolve<IEyeTrackerPlayer>();
            (Player as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(Player_PropertyChanged);
        }

        private void Player_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentTime")
            {
                if (!Player.Data.Any())
                {
                    LeftPoint = new MyPoint();
                    RightPoint = new MyPoint();
                    return;
                }

                EyeTrackerRecord record = Player.TimeRecord(Player.CurrentTime);

                if (!record.IsValid())
                {
                    LeftPoint = new MyPoint();
                    RightPoint = new MyPoint();
                    return;
                }

                LeftPoint = new MyPoint()
                {
                    X = record.leftX,
                    Y = record.leftY,
                    V = Visibility.Visible
                };
                RightPoint = new MyPoint()
                {
                    X = record.rightX,
                    Y = record.rightY,
                    V = Visibility.Visible
                };

                Points = Player.GetBacktrace(Player.CurrentTime);
            }
        }

        private List<EyeTrackerRecord> points = new List<EyeTrackerRecord>();
        public List<EyeTrackerRecord> Points
        {
            get { return points; }
            private set
            {
                if (points != value)
                {
                    points = value;
                    OnPropertyChanged("FirstPoint");
                    OnPropertyChanged("Points");
                }
            }
        }

        public Point FirstPoint
        {
            get {
                if (points.Any()) {
                    return new Point((points.First().leftX + points.First().rightX) / 2.0, (points.First().leftY + points.First().rightY) / 2.0);
                } else {
                    return new Point();
                }
            }
        }

        private MyPoint leftPoint = new MyPoint();
        public MyPoint LeftPoint
        {
            get { return leftPoint; }
            set
            {
                if (value != null && leftPoint != value)
                {
                    leftPoint = value;
                    OnPropertyChanged("LeftPoint");
                }
            }
        }

        private MyPoint rightPoint = new MyPoint();
        public MyPoint RightPoint
        {
            get { return rightPoint; }
            set
            {
                if (value != null && rightPoint != value)
                {
                    rightPoint = value;
                    OnPropertyChanged("RightPoint");
                }
            }
        }
        
        //Try using a Canvas instead of an ItemsControl and append to Canvas.Children instead of binding to the ItemsSource of an ItemsControl.
        //You will of course lose the beautiful MVVM binding wizardry, but you're looking for speed, not separation of concerns. The fastest way would be using code-behind. –

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

    public class PointArrayToPointCollection : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var points = value as IEnumerable<EyeTrackerRecord>;
            if (points != null)
            {
                var pc = new PointCollection();
                foreach (var p in points)
                {
                    pc.Add(new Point((p.leftX + p.rightX) / 2.0, (p.leftY + p.rightY) / 2.0));
                }
                return pc;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
