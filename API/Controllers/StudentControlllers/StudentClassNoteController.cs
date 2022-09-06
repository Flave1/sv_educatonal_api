using BLL.MiddleWares; 
using Contracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 
using SMP.BLL.Services.NoteServices;
using SMP.Contracts.Notes;
using System;
using System.Threading.Tasks;

namespace SMP.API.Controllers
{
    [PortalAuthorize]
    [Route("smp/studentclassnotes/api/v1")]
    public class StudentClassNoteController : Controller
    {
        private readonly IStudentNoteService service;
        public StudentClassNoteController(IStudentNoteService service)
        {
            this.service = service;
        }

        [HttpGet("get-classnote/bystudents")]
        public async Task<IActionResult> GetStudentNoteCommentsAsync(string subjectId)
        {
            var response = await service.filterClassNotesByStudentsAsync(subjectId);
            return Ok(response);
        }
    }
}
