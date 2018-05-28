using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.PubSubEvents;

namespace CheckWordEvent
{
    public class EventAggregatorRepository
    {
        //消息器，共用
        private static IEventAggregator _eventAggregator;
        public static IEventAggregator EventAggregator
        {
            get
            {
                if (_eventAggregator == null)
                {
                    _eventAggregator = new EventAggregator();
                }
                return _eventAggregator;
            }
        }
    }
}
