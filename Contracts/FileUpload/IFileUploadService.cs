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
    }
}
