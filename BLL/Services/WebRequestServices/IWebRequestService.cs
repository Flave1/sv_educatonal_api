using System.Threading.Tasks;

namespace SMP.BLL.Services.WebRequestServices
{
    public interface IWebRequestService
    {
        Task<T> PostAsync<T, Y>(string url, Y data) where Y : class;
        Task<T> GetAsync<T>(string url);
    }
}