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
            await context.SaveChangesAsync();
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
            await context.SaveChangesAsync();
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
            await context.SaveChangesAsync();
            logger.Debug(message);
        }

        public async Task Error(string message)
        {
            var log = new Log
            {
                LogType = (int)LogType.Error,
                Message = message
            };
            context.Add(log);
            await context.SaveChangesAsync();
            logger.Error(message);
        }
    }
}
