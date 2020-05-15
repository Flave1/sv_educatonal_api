namespace App.LogHandler.Service
{
    public interface ILoggerService
    {
        void Information(string message);
        void Warning(string message);
        void Debug(string message);
        void Error(string message);
    }
}
