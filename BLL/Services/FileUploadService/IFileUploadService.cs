using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SMP.BLL.Services.FileUploadService
{
    public interface IFileUploadService
    {
        string UploadProfileImage(IFormFile file);
        string UpdateProfileImage(IFormFile file, string filePath);
        string UploadSchoolLogo(IFormFile file);
        string UpdateSchoolLogo(IFormFile file, string filePath);
        string UploadPrincipalStamp(IFormFile file);
        string UpdatePrincipalStamp(IFormFile file, string filePath);
        List<string> UploadAssessmentFiles(List<IFormFile> files);
        List<string> UpdateAssessmentFiles(List<IFormFile> files, List<string> filepaths);
    }
}
