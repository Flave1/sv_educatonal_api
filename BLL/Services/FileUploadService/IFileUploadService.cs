﻿using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

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
        void DeleteFile(string filePath);
        Task<string> ReadFileAsync(string fileName, string extension, string filePath);
        Task<string> RetunFileContent(IFormFile file);
    }
}
