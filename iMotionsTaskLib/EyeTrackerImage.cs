using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace iMotionsTaskLib
{
    public class EyeTrackerImage : IEyeTrackerImage, INotifyPropertyChanged
    {
        public EyeTrackerImage()
        {
            ValidataImage(FilePath);
        }

        private int height = 1024;
        public int Height
        {
            get { return height; }
            set
            {
                if (height != value)
                {
                    height = value;
                    OnPropertyChanged("ImageHeight");
                }
            }
        }

        private int width = 1280;
        public int Width
        {
            get { return width; }
            set
            {
                if (width != value)
                {
                    width = value;
                    OnPropertyChanged("ImageWidth");
                }
            }
        }

        //private string filePath = "";
        private string filePath = "B1M12_EccoAW10C.jpg";//@"C:\dev\iMotionsTask\iMotionsTask\B1M12_EccoAW10C.jpg";
        public string FilePath
        {
            get { return filePath; }
            set {
                if (filePath != value)
                {
                    ValidataImage(value);
                }
            }
        }

        private bool ValidataImage(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath)) { return false; }

            if (!File.Exists(imagePath)) { return false; }
            
            Image image = Image.FromFile(imagePath);
            if (image == null) { return false; }
            
            filePath = imagePath;
            OnPropertyChanged("FilePath");
            Height = image.Height;
            Width = image.Width;

            return true;
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
