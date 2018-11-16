using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Castle.Core.Logging;
using log4net;
using log4net.Config;
using log4net.Repository;
using log4net.Repository.Hierarchy;

namespace YF.Utility.Logging {
    public class IQCLog4netFactory : AbstractLoggerFactory {
        private static bool _isFileWatched = false;
        private readonly ILoggerRepository _loggerRepository;
        public IQCLog4netFactory()
        {
            string configFilename = ConfigurationManager.AppSettings["log4net.Config"];
          
            Assembly assembly = Assembly.GetEntryAssembly() ?? GetCallingAssemblyFromStartup();
            _loggerRepository = LogManager.CreateRepository(assembly, typeof(Hierarchy));

            if (!_isFileWatched && !string.IsNullOrWhiteSpace(configFilename)) {
                XmlConfigurator.ConfigureAndWatch(_loggerRepository,GetConfigFile(configFilename));
                _isFileWatched = true;
            }
        }

        public override Castle.Core.Logging.ILogger Create(string name, LoggerLevel level) {
            throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
        }

        public override Castle.Core.Logging.ILogger Create(string name) {
            return new IQCLog4netLogger(LogManager.GetLogger(_loggerRepository.Name,name), this);
        }

        private static Assembly GetCallingAssemblyFromStartup()
        {
            var stackTrace = new System.Diagnostics.StackTrace(2);
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                var frame = stackTrace.GetFrame(i);
                var type = frame.GetMethod()?.DeclaringType;

                if (string.Equals(type?.Name, "Startup", StringComparison.OrdinalIgnoreCase))
                {
                    return type?.Assembly;
                }
            }
            return null;
        }
        
    }
}
