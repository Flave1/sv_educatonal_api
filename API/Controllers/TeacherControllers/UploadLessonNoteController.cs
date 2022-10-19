using BLL.Filter;
using BLL.Helpers;
using BLL.MiddleWares;
using Contracts.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SMP.BLL.Services.FileUploadService;
using SMP.BLL.Services.NoteServices;
using SMP.Contracts.Notes;
using System;
using System.Threading.Tasks;

namespace SMP.API.Controllers.TeacherControllers
{
    [Route("lessonnote/api/v1")]
    public class UploadLessonNoteController : Controller
    {
        private readonly IFileUploadService _service;

        public UploadLessonNoteController(IFileUploadService service)
        {
            _service = service;
        }

        
        [HttpPost("update/lessonnote")]
        public async Task<IActionResult> UploadTeachersNote(IFormFile file)
        {
            var response = _service.UploadLessonNote(file);
            if (response != null)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
