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
        string UploadProfileImageAsync(IFormFile file);
        string UpdateProfileImageAsync(IFormFile file, string filePath);
        string UploadSchoolLogoAsync(IFormFile file);
        string UpdateSchoolLogoAsync(IFormFile file, string filePath);
        string UploadPrincipalStampAsync(IFormFile file);
        string UpdatePrincipalStampAsync(IFormFile file, string filePath);
    }
}
