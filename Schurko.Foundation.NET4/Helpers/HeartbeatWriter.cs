using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Schurko.Foundation.Identity.Impersonation;
using Schurko.Foundation.Utilities;

namespace Schurko.Foundation.Helpers
{
    public interface IHeartbeatWriter
    {
        void Start();
        void Stop();
    }

    public class HeartbeatWriter : IHeartbeatWriter
    {
        private readonly System.Timers.Timer _timer;
        private readonly HeartbeatMode _heartbeatMode = HeartbeatMode.Disabled; // Default.
        private readonly IHeartbeatWriterProvider _writer;

        private ILoggerFactory loggerFactory = new LoggerFactory();
        private ILogger? _logger;
        private ILogger Logger => _logger ?? (_logger = loggerFactory.CreateLogger("IHeartbeatWriter"));


        protected bool IsDisabled
        {
            get { return _heartbeatMode == HeartbeatMode.Disabled; }
        }

        public HeartbeatWriter(HeartbeatMode mode = HeartbeatMode.Disabled, TimeSpan? interval = null)
        {
            _heartbeatMode = mode;

            if (IsDisabled) return;

            TimeSpan heartbeatInterval = interval ?? new TimeSpan(0, 0, 30);

            _timer = new System.Timers.Timer
            {
                AutoReset = true,
                Interval = heartbeatInterval.TotalMilliseconds,
                Enabled = true
            };

            _writer = HeartbeatWriterProviderFactory.GetHeartbeatWriterProvider(mode);

            _timer.BeginInit();

            _timer.Elapsed += (sender, args) => _writer.InvokeSignal();

            _timer.EndInit();
        }

        public void Start()
        {
            if (IsDisabled) return;

            _timer.Start();
        }

        public void Stop()
        {
            if (IsDisabled) return;

            _timer.Stop();
        }

    }

    public enum HeartbeatMode
    {
        Disabled = 0,
        LogWriter = 1,
        FileWriter = 2
    }

    public class HeartbeatWriterProviderFactory
    {
        private static ILoggerFactory loggerFactory = new LoggerFactory();
        private static ILogger? _logger;
        private static ILogger Logger => _logger ?? (_logger = loggerFactory.CreateLogger("IHeartbeatWriter"));


        public static IHeartbeatWriterProvider GetHeartbeatWriterProvider(HeartbeatMode mode)
        {
            switch (mode)
            {
                case HeartbeatMode.LogWriter:
                    return new HeartbeatLogWriterProvider();
                    break;
                case HeartbeatMode.FileWriter:
                    return new HeartbeatFileWriterProvider();
                    break;
                case HeartbeatMode.Disabled:
                    Logger.LogInformation(string.Format(
                            "Heartbeat Service is DISABLED for the machine: [{0}]",
                            Environment.MachineName));
                    return null;
                    break;
                default:
                    Logger.LogInformation(string.Format(
                            "encountered an unknown Heartbeat Service for the machine: [{0}]",
                            Environment.MachineName));
                    return null;
                    break;
            }
        }
    }

    public interface IHeartbeatWriterProvider
    {
        void InvokeSignal(string message = null);
    }

    public class HeartbeatFileWriterProvider : IHeartbeatWriterProvider
    {
        private readonly ICredentialProvider _credentialProvider;

        private ILoggerFactory loggerFactory = new LoggerFactory();
        private ILogger? _logger;
        private ILogger Logger => _logger ?? (_logger = loggerFactory.CreateLogger("IHeartbeatWriter"));

        public HeartbeatFileWriterProvider()
        {
            _credentialProvider = null;

            using (GetImpersonation())
            {
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                FileUtil.CreateDirectory("heartbeat");
            }
        }

        private SecurityImpersonation GetImpersonation()
        {
            return _credentialProvider != null ? new SecurityImpersonation(_credentialProvider) : null;
        }

        public void InvokeSignal(string message = null)
        {
            try
            {
                using (GetImpersonation())
                {
                    File.WriteAllText("heartbeat/heartbeat.info",
                        message ?? DateTime.Now.ToString(CultureInfo.InvariantCulture));
                }

                const string logMessageFormat = "Heartbeat Signal at [{0}] from [{1}]";
                string logMessage = string.Format(logMessageFormat, message ?? DateTime.Now.ToString(CultureInfo.InvariantCulture), Environment.MachineName);

                Trace.WriteLine(logMessage);
                Debug.WriteLine(logMessage);

                Logger.LogInformation(logMessage);
            }
            catch (Exception ex)
            {
                const string errorMsg = "HeartbeatFileWriterProvider Exception";
                if (_logger != null) Logger.LogError(errorMsg, ex);
                throw;
            }
        }
    }

    public class HeartbeatLogWriterProvider : IHeartbeatWriterProvider
    {
        private ILoggerFactory loggerFactory = new LoggerFactory();
        private ILogger? _logger;
        private ILogger Logger => _logger ?? (_logger = loggerFactory.CreateLogger("IHeartbeatWriter"));


        public HeartbeatLogWriterProvider()
        {

        }

        public void InvokeSignal(string message = null)
        {
            try
            {
                const string logMessageFormat = " Heartbeat Signal at [{0}] from [{1}]";
                string logMessage = string.Format(logMessageFormat, message ?? DateTime.Now.ToString(CultureInfo.InvariantCulture), Environment.MachineName);

                Trace.WriteLine(logMessage);
                Debug.WriteLine(logMessage);

                Logger.LogInformation(logMessage);
            }
            catch (Exception ex)
            {
                const string errorMsg = "HeartbeatLogWriterProvider Exception";
                Logger.LogError(errorMsg, ex);
                throw;
            }
        }
    }
}
