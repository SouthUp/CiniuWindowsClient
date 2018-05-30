﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MyWordAddIn
{
    /// <summary>
    /// LoadingWait.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingWait : UserControl
    {
        #region Data  
        private readonly DispatcherTimer animationTimer;
        #endregion
        public LoadingWait()
        {
            InitializeComponent();
            try
            {
                animationTimer = new DispatcherTimer(
                DispatcherPriority.ContextIdle, Dispatcher);
                animationTimer.Interval = new TimeSpan(0, 0, 0, 0, 90);
            }
            catch
            { }
        }

        #region Private Methods  
        private void Start()
        {
            try
            {
                animationTimer.Tick += HandleAnimationTick;
                animationTimer.Start();
            }
            catch
            { }
        }

        private void Stop()
        {
            try
            {
                animationTimer.Stop();
                animationTimer.Tick -= HandleAnimationTick;
            }
            catch
            { }
        }

        private void HandleAnimationTick(object sender, EventArgs e)
        {
            try
            {
                SpinnerRotate.Angle = (SpinnerRotate.Angle + 36) % 360;
            }
            catch
            { }
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                const double offset = Math.PI;
                const double step = Math.PI * 2 / 10.0;

                SetPosition(C0, offset, 0.0, step);
                SetPosition(C1, offset, 1.0, step);
                SetPosition(C2, offset, 2.0, step);
                SetPosition(C3, offset, 3.0, step);
                SetPosition(C4, offset, 4.0, step);
                SetPosition(C5, offset, 5.0, step);
                SetPosition(C6, offset, 6.0, step);
                SetPosition(C7, offset, 7.0, step);
                SetPosition(C8, offset, 8.0, step);
            }
            catch
            { }
        }

        private void SetPosition(Ellipse ellipse, double offset,
            double posOffSet, double step)
        {
            ellipse.SetValue(Canvas.LeftProperty, 50.0
                + Math.Sin(offset + posOffSet * step) * 50.0);

            ellipse.SetValue(Canvas.TopProperty, 50
                + Math.Cos(offset + posOffSet * step) * 50.0);
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void HandleVisibleChanged(object sender,
            DependencyPropertyChangedEventArgs e)
        {
            try
            {
                bool isVisible = (bool)e.NewValue;

                if (isVisible)
                    Start();
                else
                    Stop();
            }
            catch
            { }
        }
        #endregion
    }
}
