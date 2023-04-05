using System.Threading.Tasks;

namespace BLL.LoggerService
{
    public interface ILoggerService
    {
        void Information(string message);
        void Warning(string message);
        void Debug(string message);
        void Error(string message, string stackTrace, string innerException, string innerExceptionMessage);
    }
}
