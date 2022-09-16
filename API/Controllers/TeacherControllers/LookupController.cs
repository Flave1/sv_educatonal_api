using BLL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SMP.BLL.Services.WebRequestServices;
using SMP.Contracts.LookupModels;
using SMP.Contracts.Options;
using SMP.Contracts.Routes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMP.API.Controllers.TeacherControllers
{
    
    public class LookupController : Controller
    {
        private readonly IWebRequestService service;
        private readonly FwsConfigSettings fwsOptions;
        public LookupController(IWebRequestService service, IOptions<FwsConfigSettings> fwsOptions)
        {
            this.service = service;
            this.fwsOptions = fwsOptions.Value;
        }


        [HttpGet("get-countries")]
        public async Task<IActionResult> GetCountries()
        {
            return Ok(await service.GetAsync<APIResponse<List<SelectLookup>>>($"{fwsOptions.FwsBaseUrl}{fwsRoutes.countrySelect}"));
        }
        [HttpGet("get-states")]
        public async Task<IActionResult> GetCountries(string state)
        {
            return Ok(await service.GetAsync<APIResponse<List<SelectLookup>>>($"{fwsOptions.FwsBaseUrl}{fwsRoutes.stateSelect}{state}"));
        }
        [HttpGet("get-cities")]
        public async Task<IActionResult> GetCities(string city)
        {
            return Ok(await service.GetAsync<APIResponse<List<SelectLookup>>>($"{fwsOptions.FwsBaseUrl}{fwsRoutes.citySelect}{city}"));
        }
    }
}
