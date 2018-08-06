using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.UnityExtensions;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using Microsoft.Practices.Prism.PubSubEvents;
using CheckWordEvent;

namespace WordAndImgOperationApp
{
    class MetroBootstrapper : UnityBootstrapper
    {
        #region Private Properties
        private IShell Shell { get; set; }
        #endregion
        protected override void ConfigureContainer()
        {
            Container.RegisterType<IShell, MainWindow>(new ContainerControlledLifetimeManager());
            base.ConfigureContainer();
        }

        protected override DependencyObject CreateShell()
        {
            Shell = Container.Resolve<IShell>();
            return Shell as DependencyObject;
        }

        protected override void InitializeModules()
        {
            Shell.Show();
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            loginWindow.Activate();
        }
    }
}