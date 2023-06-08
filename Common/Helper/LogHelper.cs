using System.Reflection;
using System.Xml;
using log4net;
using log4net.Config;

[assembly: log4net.Config.XmlConfigurator(ConfigFileExtension = "log4net", Watch = true)]
//[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config.xml", Watch = true)]
namespace Common.Helper
{
    public interface ILogHelper
    {
    }



    public class LogHelper : ILogHelper
    {
        //private static readonly ILog _log4Net = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //private const string DEFAULT_LOGGER_NAME = "Logger";


        private static readonly ILog _log4Net = LogManager.GetLogger(typeof(LogHelper));
        public LogHelper()
        {
            try
            {
                XmlDocument log4netConfig = new XmlDocument();

                using (var fs = File.OpenRead("log4net.config"))
                {
                    log4netConfig.Load(fs);

                    var repo = LogManager.CreateRepository(
                            Assembly.GetEntryAssembly(),
                            typeof(log4net.Repository.Hierarchy.Hierarchy));

                    XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

                    // The first log to be written 
                    _log4Net.Info("Log System Initialized");
                }
            }
            catch (Exception ex)
            {
                _log4Net.Error("Error", ex);
            }
        }


        public static void InfoLog(object message)
        {
            if (_log4Net.IsInfoEnabled)
            {
                _log4Net.Info(message);
            }
        }

        public static void InfoLog(string methodName, Exception ex)
        {
            if (_log4Net.IsInfoEnabled && ex != null)
            {
                _log4Net.Info(methodName, ex);
            }
        }

        // Writes a warning level logging message.
        public static void WarningLog(object message)
        {
            if (_log4Net.IsWarnEnabled)
            {
                _log4Net.Warn(message);
            }
        }

        // Writes a warning level logging message.
        public static void WarningLog(string methodName, System.Exception ex)
        {
            if(_log4Net.IsWarnEnabled && ex != null)
            {
                _log4Net.Warn(methodName, ex);
            }
        }

        // Writes the error.
        public static void ErrorLog(object message)
        {
            if (_log4Net.IsErrorEnabled)
            {
                _log4Net.Error(message);
            }
            //return Task.FromResult(true);
        }

      

        public static void ErrorLog(string methodName, Exception ex)
        {
            if (_log4Net.IsErrorEnabled)
            {
                _log4Net.Error(methodName, ex);
            }
            //return Task.FromResult(true);
        }

        //public static void  ErrorLog(string methodName, string errorMessage, Exception ex)
        //{
        //    if (_log4Net.IsErrorEnabled)
        //    {
        //        _log4Net.Error($"Method:{methodName}, Message{errorMessage}", ex);
        //    }
        //    //return Task.FromResult(true);
        //}



        //public static async Task ErrorLogAsync(string methodName, string errorMessage, string? stackTrace)


        //public static void ErrorLog(string methodName, string errorMessage, string? stackTrace)
        //{
        //    _log4Net.Error(new LogModel(LogLevelEnum.Error, methodName, errorMessage, stackTrace));
        //}


        // Writes the fatal error level logging message..
        public static void FatalLog(object message)
        {
            if (_log4Net.IsFatalEnabled)
            {
                _log4Net.Fatal(message);
            }
        }

        // Writes the fatal error level logging message..
        public static void FatalLog(object message, System.Exception exception)
        {
            if(_log4Net.IsFatalEnabled && exception != null)
            {
                _log4Net.Fatal(message, exception);
            }
        }

        public static void DeleteLog()
        {
            string logDirPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Log");
            if (!Directory.Exists(logDirPath)) return;
            int days = 30;
            foreach (string filePath in Directory.GetFiles(logDirPath))
            {
                DateTime dt;
                DateTime.TryParse(Path.GetFileNameWithoutExtension(filePath).Replace(@"Log\", "").Replace(".", "-"), out dt);
                if (dt.AddDays(days).CompareTo(DateTime.Now) < 0)
                {
                    File.Delete(filePath);
                }
            }
        }
    }
}
