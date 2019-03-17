using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using iMotionsTask.Services;
using iMotionsTaskLib;
using Microsoft.Win32;
using Unity;

namespace iMotionsTask.Components
{
    public class ToolBarViewModel
    {
        public IEyeTrackerReader Reader { get; private set; }
        public IEyeTrackerImage BackGroundImage { get; private set; }

        public RelayCommand LoadImageCommand { get; private set; }
        public RelayCommand LoadDataCommand { get; private set; }
        
        public ToolBarViewModel()
        {
            // Return if in designer
            if (DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject())) { return; }

            Reader = ContainerHelper.Container.Resolve<IEyeTrackerReader>();
            BackGroundImage = ContainerHelper.Container.Resolve<IEyeTrackerImage>();

            LoadImageCommand = new RelayCommand(OnLoadImage);
            LoadDataCommand = new RelayCommand(OnLoadData);

            Reader.ParseFile();
        }

        private void OnLoadImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = BackGroundImage.FilePath;
            if (openFileDialog.ShowDialog() == true)
            {
                BackGroundImage.FilePath = openFileDialog.FileName;
            }
        }

        private void OnLoadData()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = Reader.FilePath;
            if (openFileDialog.ShowDialog() == true)
            {
                Reader.FilePath = openFileDialog.FileName;
                Reader.ParseFile();
            }
                
        }
    }
}
