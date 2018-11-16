using System;

namespace YF.Utility.Logging {
    public interface ILoggerFactory {
        ILogger CreateLogger(Type type);
    }
}