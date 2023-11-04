using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Logging
{
    public class Log
    {
        private static ILoggerFactory loggerFactory = (ILoggerFactory)new LoggerFactory();
        private static Microsoft.Extensions.Logging.ILogger? _logger;
        public static Microsoft.Extensions.Logging.ILogger Logger => _logger ?? (_logger = loggerFactory.CreateLogger("Logger"));

        public Log()
        {
        }
    }
}
