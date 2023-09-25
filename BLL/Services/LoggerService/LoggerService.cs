using DAL;
using NLog;
using SMP.BLL.Constants;
using SMP.DAL.Models.Logger;
using System.Threading.Tasks;

namespace BLL.LoggerService
{

    public class LoggerService : ILoggerService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly DataContext context;

        public LoggerService(DataContext context)
        {
            this.context = context;
        }

        public void Information(string message)
        {
            var log = new Log
            {
                LogType = (int)LogType.Info,
                Message = message
            };
            logger.Info(message);
            context.Add(log);
            context.SaveChangesNoClientAsync().Wait();
        }

        public void Warning(string message)
        {
            var log = new Log
            {
                LogType = (int)LogType.Warning,
                Message = message
            };
            logger.Warn(message);
            context.Add(log);
            context.SaveChangesNoClientAsync().Wait();
        }

        public void Debug(string message)
        {
            var log = new Log
            {
                LogType = (int)LogType.Debug,
                Message = message
            };
            logger.Debug(message);
            context.Add(log);
            context.SaveChangesNoClientAsync().Wait();
        }

        public void Error(string message, string stackTrace, string innerException, string innerExceptionMessage)
        {
            var log = new Log
            {
                LogType = (int)LogType.Error,
                Message = message,
                StackTrace = stackTrace,
                InnerException = innerException,
                InnerExceptionMessage = innerExceptionMessage
            };
            logger.Error($"Message: {message} || StackTrace: {message} || InnerException: {innerException} || InnerExceptionMessage: {innerExceptionMessage}");
            context.Add(log);
            context.SaveChangesNoClientAsync().Wait();
        }
    }
}
