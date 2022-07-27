using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.FileUpload
{
    public interface IFileUploadService
    {
        string UploadProfileImage(IFormFile file);
        string UpdateProfileImage(IFormFile file, string filePath);
        string UploadSchoolLogo(IFormFile file);
        string UpdateSchoolLogo(IFormFile file, string filePath);
        string UploadPrincipalStamp(IFormFile file);
        string UpdatePrincipalStamp(IFormFile file, string filePath);
    }
}
