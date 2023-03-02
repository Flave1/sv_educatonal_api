using System.Threading.Tasks;

namespace BLL.LoggerService
{
    public interface ILoggerService
    {
        Task Information(string message);
        Task Warning(string message);
        Task Debug(string message);
        Task Error(string message);
    }
}
