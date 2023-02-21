using Microsoft.AspNetCore.Http;
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
        string UploadFeedbackFiles();
        string UpdateFeedbackFiles(string filePath);
        string RetunFileContent(IFormFile file);
        void DeleteFile(string filePath);
        void CopyFile(string oldFilePath, string newFilePath);
        string ReadFileAsync(string filePath);
        string UploadAdmissionCredentials(IFormFile file);
        string UploadAdmissionPassport(IFormFile file);
        void CreateClientDirectory(string clientId);
    }
}
