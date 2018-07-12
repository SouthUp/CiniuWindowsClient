using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WPFClientCheckWordUtil.Log
{
    /// <summary>
    /// 日志文件管理
    /// </summary>
    public static class TextLog
    {
        /// <summary>
        /// 两次写错误日志文件间隔时间，单位为秒，默认为1毫秒。
        /// </summary>
        public static double SleepSecondsBetweenWriteErrorLog = 0.001;
        /// <summary>
        /// 两次写正常日志文件间隔时间，单位为秒，默认为1秒。
        /// </summary>
        public static double SleepSecondsBetweenWriteNormalLog = 1;

        /// <summary>
        /// 多长时间检查一次日志文件的大小，单位为分钟，默认为1。
        /// </summary>
        public static int MinutesBetweenCheckFileSize = 10;
        /// <summary>
        /// 日志文件的最大值，单位为MB，默认为10。
        /// </summary>
        public static int MaxFileLengthOfMB = 10;

        static string errorFilePath;
        private static string errorLogDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CiNiu\\ClientErrorLog\\";
        /// <summary>
        ///  错误日志默认位置
        /// </summary>
        public static string ErrorLogDir
        {
            get
            {
                return errorLogDir;
            }
            set
            {
                errorLogDir = value;
                Ensure(errorLogDir);
                errorFilePath = Path.Combine(value, DateTime.Now.ToString("yyyyMMdd") + ".Error.txt");
            }
        }

        static string normalFilePath;
        private static string normalLogDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CiNiu\\ClientNormalLog\\";
        public static string NormalLogDir
        {
            get
            {
                return normalLogDir;
            }
            set
            {
                normalLogDir = value;
                Ensure(value);
                normalFilePath = Path.Combine(value, DateTime.Now.ToString("yyyyMMdd") + ".Normal.txt");
            }
        }

        static TextLog()
        {
            try
            {
                ErrorLogDir = errorLogDir;
                NormalLogDir = normalLogDir;

                if (writeErrorLogThread == null)
                {
                    writeErrorLogThread = new Thread(innerWriteErrorLog) { IsBackground = true, Name = "Error日志保存线程-常驻" };
                    writeErrorLogThread.Start();
                }
                if (writeNormalLogThread == null)
                {
                    writeNormalLogThread = new Thread(InnerWriteNormalLog) { IsBackground = true, Name = "Normal日志保存线程-常驻" };
                    writeNormalLogThread.Start();
                }
                MonitorFileSize();
            }
            catch (Exception)
            { }
        }

        public static void Dispose()
        {
            try
            {
                if (MonitorFileSizeThread != null)
                    MonitorFileSizeThread.Abort();

                if (writeNormalLogThread != null)
                    writeNormalLogThread.Abort();

                if (writeErrorLogThread != null)
                    writeErrorLogThread.Abort();
            }
            catch
            { }
        }

        #region Normal

        public static void SaveNormal(string message)
        {
            var detail = GetDetail(message);

            lock (normalLogListLocker)
                normalLogList.Add(detail);
        }

        static Thread writeNormalLogThread;
        private static List<string> normalLogList = new List<string>();
        static object normalLogListLocker = new object();
        private static void InnerWriteNormalLog()
        {
            InnerWriteLog(normalLogList, normalLogListLocker, ref SleepSecondsBetweenWriteNormalLog, ref normalFilePath);
        }

        #endregion

        #region Error

        public static void SaveError(string message)
        {
            var detail = GetDetail(message);

            lock (errorLogLock)
                errorLogList.Add(detail);
        }

        private static Thread writeErrorLogThread;
        private static object errorLogLock = new object();
        private static List<string> errorLogList = new List<string>();
        private static void innerWriteErrorLog()
        {
            InnerWriteLog(errorLogList, errorLogLock, ref SleepSecondsBetweenWriteErrorLog, ref errorFilePath);
        }
        #endregion

        #region 公共

        private static void Ensure(string dir)
        {
            try
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
            catch
            { }
        }

        static Thread MonitorFileSizeThread;
        /// <summary>
        /// 监控文件大小，超过了配置的文件大小后，创建新文件。
        /// </summary>
        private static void MonitorFileSize()
        {
            MonitorFileSizeThread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        if (Directory.Exists(ErrorLogDir))
                        {
                            foreach (string d in Directory.GetFileSystemEntries(ErrorLogDir))
                            {
                                if (File.Exists(d))
                                {
                                    FileInfo fi = new FileInfo(d);
                                    if (fi.LastWriteTime < DateTime.Now.AddDays(-7))
                                    {
                                        if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                                            fi.Attributes = FileAttributes.Normal;
                                        File.Delete(d);//直接删除其中的文件 
                                    }
                                }
                            }
                        }
                    }
                    catch
                    { }
                    try
                    {
                        var maxB = MaxFileLengthOfMB * 1024 * 1024;

                        FileInfo fileInfo;
                        if (errorFilePath != null)
                        {
                            fileInfo = new FileInfo(errorFilePath);
                            if (fileInfo.Exists && fileInfo.Length > maxB)
                            {
                                errorFilePath = Path.Combine(ErrorLogDir, DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".Error.txt");
                            }
                        }

                        if (normalFilePath != null)
                        {
                            fileInfo = new FileInfo(normalFilePath);
                            if (fileInfo.Exists && fileInfo.Length > maxB)
                                normalFilePath = Path.Combine(NormalLogDir, DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".Normal.txt");
                        }

                        Thread.Sleep(TimeSpan.FromMinutes(MinutesBetweenCheckFileSize));
                    }
                    catch
                    { }
                }
            }) { IsBackground = true, Name = "日志文件大小监控线程-常驻" };
            MonitorFileSizeThread.Start();
        }

        private static void InnerWriteLog(List<string> list, object locker, ref double seconds, ref string saveTo)
        {
            while (true)
            {
                try
                {
                    var count = list.Count;
                    if (count > 0)
                    {
                        lock (locker)
                        {
                            // 将当前队列中的项一次全部写入日志文件。
                            var multiLinesLogInfo = list.Take(count).Aggregate("", (s, e) => s + e + Environment.NewLine, r => r);
                            Write(multiLinesLogInfo, saveTo);

                            list.RemoveRange(0, count);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(DateTime.Now.ToString() + " " + ex.Message);
                }

                Thread.Sleep(TimeSpan.FromSeconds(seconds));
            }
        }

        private static string GetDetail(string message)
        {
            try
            {
                var stackTrace = new StackTrace(true);
                var indexOfStack = 2;

                var errorLogInfo = DateTime.Now.ToString("yyyy/MM/dd HH':'mm':'ss'.'fff") + " 类：" + stackTrace.GetFrame(indexOfStack).GetMethod().DeclaringType.FullName;
                errorLogInfo += " 函数：" + stackTrace.GetFrame(indexOfStack).GetMethod().Name;
                errorLogInfo += " 信息：" + message;

                return errorLogInfo;
            }
            catch
            {
                return message;
            }
        }
        private static void Write(string message, string toFile)
        {
            try
            {
                using (var s = new StreamWriter(toFile, true, Encoding.UTF8))
                {
                    s.Write(message);
                }
            }
            catch
            { }
        }

        #endregion
    }
}
