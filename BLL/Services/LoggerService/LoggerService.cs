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

        public async Task Information(string message)
        {
            var log = new Log
            {
                LogType = (int)LogType.Info,
                Message = message
            };
            context.Add(log);
            await context.SaveChangesNoClientAsync();
            logger.Info(message);
        }

        public async Task Warning(string message)
        {
            var log = new Log
            {
                LogType = (int)LogType.Warning,
                Message = message
            };
            context.Add(log);
            await context.SaveChangesNoClientAsync();
            logger.Warn(message);
        }

        public async Task Debug(string message)
        {
            var log = new Log
            {
                LogType = (int)LogType.Debug,
                Message = message
            };
            context.Add(log);
            await context.SaveChangesNoClientAsync();
            logger.Debug(message);
        }

        public async Task Error(string message, string stackTrace, string innerException, string innerExceptionMessage)
        {
            var log = new Log
            {
                LogType = (int)LogType.Error,
                Message = message,
                StackTrace = stackTrace,
                InnerException = innerException,
                InnerExceptionMessage = innerExceptionMessage
            };
            context.Add(log);
            await context.SaveChangesNoClientAsync();
            logger.Error(message);
        }
    }
}
