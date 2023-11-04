
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Logging
{
    public class Log
    {
        private static readonly ILogger Instance = new Logger();

        public static ILogger Logger
        {
            get { return Instance; }
        }
    }

    public class Logger : ILogger
    {
        public Logger()
        {
        }

        public void LogError(string message, Exception exception)
        {
            Trace.TraceError(message, exception);
        }

     
        public void LogError(string message, params object[] list)
        {
            Trace.TraceError(message, list);
        }

        public void LogError(string message)
        {
            Trace.TraceError(message);
        }

        public void LogFatal(string message)
        {
            Trace.TraceError(message);
        }

        public void LogInfo(string message)
        {
            Trace.TraceInformation(message);
        }

        public void Log(string message)
        {
            Trace.WriteLine(message);
        }

    

        public void LogWarn(string message)
        {
            Trace.TraceWarning(message);
        }
    }

    public interface ILogger
    {
        void LogInfo(string message);
        void LogWarn(string message);
        void LogError(string message, Exception exception);
        void LogError(string message, params object[] list);
        void LogFatal(string message);
        void Log(string message);
        

    }
}
