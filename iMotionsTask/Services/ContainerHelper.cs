using iMotionsTaskLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;

namespace iMotionsTask.Services
{
    class ContainerHelper
    {
        private static IUnityContainer _container;

        static ContainerHelper()
        {
            _container = new UnityContainer();
            _container.RegisterType<IEyeTrackerData, EyeTrackerData>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IEyeTrackerReader, EyeTrackerReader>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IEyeTrackerImage, EyeTrackerImage>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IEyeTrackerPlayer, EyeTrackerPlayer>(new ContainerControlledLifetimeManager());
        }

        public static IUnityContainer Container
        {
            get { return _container; }
        }
    }
}
