using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace CheckWordControl.Notify
{
    public class NotifyMessageManager
    {
        /// <summary>
        ///  数据同步锁
        /// </summary>
        private readonly object _syncRoot = new object();
        /// <summary>
        ///  屏幕能够显示通知信息的最大个数
        /// </summary>
        private readonly int popupMaxCount;
        /// <summary>
        ///  通知信息显示位置<see cref="AnimateLocation"/>列表
        /// </summary>
        private List<AnimateLocation> displayLocations;
        /// <summary>
        ///  通知信息<see cref="NotifyMessageViewModel"/>数组
        /// </summary>
        private NotifyMessageViewModel[] displayMessages;
        /// <summary>
        ///  是否为开启显示通知状态
        /// </summary>
        private bool _isStarted;
        /// <summary>
        ///  <see cref="CancellationTokenSource"/>
        /// </summary>
        private CancellationTokenSource _cts;
        /// <summary>
        ///  通知信息<see cref="NotifyMessage"/>队列
        /// </summary>
        protected ConcurrentQueue<NotifyMessage> queueMessages { get; set; }

        private delegate void MethodInvoker();

        #region 单例模式


        private NotifyMessageManager(double popupWidth, double popupHeight)
        {
            queueMessages = new ConcurrentQueue<NotifyMessage>();                      //初始化通知信息队列
            popupMaxCount = Convert.ToInt32(Screen.ScreenHeight / popupHeight) - 1;    //计算屏幕能够显示通知信息的最大个数

            displayMessages = new NotifyMessageViewModel[popupMaxCount];               //初始化通知信息个数为最大个数
            //初始化通知信息显示位置
            displayLocations = new List<AnimateLocation>(popupMaxCount);
            double left = Screen.ScreenWidth - popupWidth;
            double top = Screen.ScreenHeight;
            for (int index = 0; index < popupMaxCount; index++)
            {
                if (index == 0)
                {
                    displayLocations.Add(new AnimateLocation(left, left, top, top - popupHeight));
                }
                else
                {
                    var previousLocation = displayLocations[index - 1];
                    displayLocations.Add(new AnimateLocation(left, left, previousLocation.ToTop, previousLocation.ToTop - popupHeight));
                }
            }

            _isStarted = false;
        }
        private static NotifyMessageManager instance = new NotifyMessageManager(300, 100);
        public static NotifyMessageManager Current
        {
            get
            {
                if (instance == null)
                {
                    return new NotifyMessageManager(300,100);
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        #endregion

        /// <summary>
        ///  异步显示信息
        /// </summary>
        /// <param name="msg">显示的信息<see cref="NotifyMessage"/></param>
        public void EnqueueMessage(NotifyMessage msg)
        {
            queueMessages.Enqueue(msg);
            Start();
        }

        private void Start()
        {
            lock (_syncRoot)
            {
                if (!_isStarted)
                {
                    _cts = new CancellationTokenSource();
                    StartService(_cts.Token);
                    _isStarted = false;
                }
            }
        }

        private void StartService(CancellationToken ct)
        {
            Task.Factory.StartNew(() =>
                {
                    try
                    {
                        //获取在屏幕上显示的下一个位置
                        int nextLocation = FindNextLocation();
                        if (nextLocation != -1)
                        {
                            while (!ct.IsCancellationRequested && queueMessages.Count > 0)
                            {
                                NotifyMessage notifyMsg = null;
                                if (queueMessages.TryDequeue(out notifyMsg))
                                {
                                    NotifyMessageViewModel viewModel = new NotifyMessageViewModel
                                        (notifyMsg,
                                        displayLocations[nextLocation],
                                        () => { displayMessages[nextLocation] = null; }
                                        );
                                    displayMessages[nextLocation] = viewModel;
                                    var dispatcher = Application.Current.Dispatcher;
                                    dispatcher.Invoke(new Action(() =>
                                    {
                                        NotifyMessageView window = new NotifyMessageView();
                                        window.ShowInTaskbar = false;
                                        window.DataContext = viewModel;
                                        window.Show();
                                    }),DispatcherPriority.Background);
                                    Thread.Sleep(1000);
                                }
                            }
                            Stop();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            ); 
        }

        private int FindNextLocation()
        {
            for (int index = 0; index < displayMessages.Count(); index++)
            {
                if (displayMessages[index] == null)
                    return index;
            }
            return -1;
        }

        /// <summary>
        ///  取消任务
        /// </summary>
        private void Stop()
        {
            lock (_syncRoot)
            {
                if (_isStarted)
                {
                    _cts.Cancel();
                    _cts.Dispose();
                    _isStarted = false;
                }
            }
        }
    }
}
