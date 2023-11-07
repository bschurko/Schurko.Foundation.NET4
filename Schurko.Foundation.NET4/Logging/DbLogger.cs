using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Schurko.Foundation.Data;
using System.Diagnostics;


namespace Schurko.Foundation.Logging
{
    public class DbLogger : IDbLogger
    {
        private string _connectionString;
        public DbLogger(IConnectionString connectionString)
        {
            _connectionString = connectionString.Value;
        }

        public void LogDebug(string message, string messageTemplate = null)
        {
            ILogMessage log = new LogMessage(message, messageTemplate, LogLevel.Debug.ToString(), DateTime.Now, null, null);
            Task<bool> result = Task.Run(async () => await LogAsync(log));
        }

        public void LogError(string message, Exception exception)
        {
            ILogMessage log = new LogMessage(message, null, LogLevel.Error.ToString(), DateTime.Now, exception.ToString(), null);
            Task<bool> result = Task.Run(async () => await LogAsync(log));
        }

        public void LogException(string message)
        {
            ILogMessage log = new LogMessage(message, null, LogLevel.Exception.ToString(), DateTime.Now, null, null);
            Task<bool> result = Task.Run(async () => await LogAsync(log));
        }

        public void LogException(string message, Exception exception)
        {
            ILogMessage log = new LogMessage(message, null, LogLevel.Exception.ToString(), DateTime.Now, exception.ToString(), null);
            Task<bool> result = Task.Run(async () => await LogAsync(log));
        }

        public void LogFatal(string message)
        {
            ILogMessage log = new LogMessage(message, null, LogLevel.Fatal.ToString(), DateTime.Now, null, null);
            Task<bool> result = Task.Run(async () => await LogAsync(log));
        }

        public void LogFatal(string message, Exception exception, string messageTemplate = null)
        {
            ILogMessage log = new LogMessage(message, messageTemplate, LogLevel.Fatal.ToString(), DateTime.Now, exception.ToString(), null);
            Task<bool> result = Task.Run(async () => await LogAsync(log));
        }

        public void LogInfo(string message, string messageTemplate = null)
        {
            ILogMessage log = new LogMessage(message, messageTemplate, LogLevel.Info.ToString(), DateTime.Now, null, null);
            Task<bool> result = Task.Run(async () => await LogAsync(log));
        }
         
        public void LogVerbose(string message, string messageTemplate, string properties)
        {
            ILogMessage log = new LogMessage(message, messageTemplate, LogLevel.Verbose.ToString(), DateTime.Now, null, properties);
            Task<bool> result = Task.Run(async () => await LogAsync(log));
        }

        public async void LogWarn(string message)
        {
            ILogMessage log = new LogMessage(message, null, LogLevel.Warn.ToString(), DateTime.Now, null, null);
            Task<bool> result = Task.Run(async () => await LogAsync(log));
        }

        private async Task<bool> LogAsync(ILogMessage message)
        {
            try
            {
                const string StoredProcName = @"spCreateLog";
                 
                var p = new DynamicParameters();

                p.Add("Message", message.Message);
                p.Add("MessageTemplate", message.MessageTemplate);
                p.Add("Level", message.Level);
                p.Add("TimeStamp", message.TimeStamp);
                p.Add("Exception", message.Exception);
                p.Add("Properties", message.Properties);

                await StoredProcWithParamsAsync<LogMessage>(StoredProcName, p).ConfigureAwait(false);
                 
                return true;
            }
            catch (Exception ex)
            {
                Trace.TraceError("DbLogger Exception", ex);
                return false;
            }
        }


        private async Task<List<T>> StoredProcWithParamsAsync<T>(string procname, object parms)
        {
            
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                var task = await connection.QueryAsync<T>(procname, (object)parms, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

                return task.ToList();
            }
        }

        public enum LogLevel
        {
            Warn = 0,
            Error = 1,
            Info = 2,
            Debug = 3,
            Fatal = 4,
            Exception = 5,
            Verbose = 6
        }
    }



    public interface IDbLogger
    {
        void LogWarn(string message);
        void LogError(string message, Exception exception);
        void LogInfo(string message, string messageTemplate = null);
        void LogDebug(string message, string messageTemplate = null);
        void LogVerbose(string message, string messageTemplate, string properties);
        void LogException(string message);
        void LogFatal(string message);
        void LogException(string message, Exception exception);
        void LogFatal(string message, Exception exception, string messageTemplate = null);
     
    }



    public class LogMessage : ILogMessage
    {
        public LogMessage() { }

        public LogMessage(string message) 
        {
            this.Message = message;
        }

        public LogMessage(string message, string messageTemplate, string level, DateTime timeStamp, string exception, string properties)
        {
            Message = message;
            MessageTemplate = messageTemplate;
            Level = level;
            TimeStamp = timeStamp;
            Exception = exception;
            Properties = properties;
        }
        public LogMessage(int id, string message, string messageTemplate, string level, DateTime timeStamp, string exception, string properties)
        {
            Id = id;
            Message = message;
            MessageTemplate = messageTemplate;
            Level = level;
            TimeStamp = timeStamp;
            Exception = exception;
            Properties = properties;
        }

        public int Id { get; set; }
        public string Message { get; set; }
        public string MessageTemplate { get; set; }
        public string Level { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Exception { get; set; }
        public string Properties { get; set; }
    }

    public interface ILogMessage
    {
        string Exception { get; set; }
        int Id { get; set; }
        string Level { get; set; }
        string Message { get; set; }
        string MessageTemplate { get; set; }
        string Properties { get; set; }
        DateTime TimeStamp { get; set; }
    }
}

 