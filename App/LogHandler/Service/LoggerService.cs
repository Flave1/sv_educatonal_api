using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;  
namespace App.LogHandler.Service
{
    public class LoggerService : ILoggerService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        public LoggerService()
        {

        }

        public void Information(string message)
        {
            logger.Info(message);
        }

        public void Warning(string message)
        {
            logger.Warn(message);
        }

        public void Debug(string message)
        {
            logger.Debug(message);
        }

        public void Error(string message)
        {
            logger.Error(message);
        }
    }
}
