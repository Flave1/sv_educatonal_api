using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.BLL.Services.WebRequestServices
{
    public interface IWebRequestService
    {
        Task<T> PostAsync<T, Y>(string url, Y data, Dictionary<string, string> credentials = null) where Y : class;
        Task<T> GetAsync<T>(string url, Dictionary<string, string> credentials = null);
    }
}