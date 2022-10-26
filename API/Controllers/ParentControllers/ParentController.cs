using BLL.Filter;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.ParentServices;
using SMP.Contracts.Parents;
using System.Threading.Tasks;

namespace SMP.API.Controllers
{
    public class ParentController : Controller
    {
        private readonly IParentService service;
        public ParentController(IParentService sertvice)
        {
            this.service = sertvice;
        }
        [HttpPost("/create/parent")]
        public async Task<IActionResult> CreateParentAsync([FromForm] Parents Parents)
        {
            var response = await service.AddParentAsync(Parents);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }
        [HttpPost("/update/parent")]
        public async Task<IActionResult> UpdateParentAsync([FromForm] UpdateParent Parents)
        {
            var response = await service.UpdateParent(Parents);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);
        }

        [HttpGet("getall/parents")]
        public async Task<IActionResult> GetAllParentAssync(PaginationFilter filter)
        {
            var response = await service.GetAllParentsAsync(filter);
            return Ok(response);
        }

        [HttpGet("getstudents/parents")]
        public async Task<IActionResult> GetAllStudentsByParentsId(Parents Parents)
        {
            var response = await service.GetAllStudentsByParentId(Parents);
            if (response.IsSuccessful)
                return Ok(response);
            return BadRequest(response);

        }
    }
}
