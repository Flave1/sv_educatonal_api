using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SMP.BLL.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.FileUploadService
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment environment;
        private readonly IHttpContextAccessor accessor;
        private static string ProfileImagePath = "ProfileImage";
        private static string SchoolLogoPath = "SchoolLogo";
        private static string PrincipalStampPath = "PrincipalStamp";
        private static string LessonNotePath = "LessonNote";
        public FileUploadService(IWebHostEnvironment environment, IHttpContextAccessor httpContext)
        {
            this.environment = environment;
            accessor = httpContext;
        }
        string IFileUploadService.UploadProfileImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return "";
            }
            int maxFileSize = 1024 * 1024 / 2;
            var fileSize = file.Length;

            if (fileSize > maxFileSize)
            {
                throw new ArgumentException($"file limit exceeded, greater than {maxFileSize}");
            }

            if (file.FileName.EndsWith(".jpg")
                        || file != null && file.Length > 0 || file.FileName.EndsWith(".jpg")
                        || file.FileName.EndsWith(".jpeg") || file.FileName.EndsWith(".png"))
            {
                string extension = Path.GetExtension(file.FileName);
                string fileName = Guid.NewGuid().ToString() + extension;

                var filePath = Path.Combine(environment.ContentRootPath, "wwwroot/" + ProfileImagePath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    fileStream.Position = 0;
                    file.CopyTo(fileStream);
                    fileStream.Flush();
                    fileStream.Close();
                }

                var host = accessor.HttpContext.Request.Host.ToUriComponent();
                var url = $"{accessor.HttpContext.Request.Scheme}://{host}/{ProfileImagePath}/{fileName}";
                return url;
            }
            throw new ArgumentException("Invalid Profile Image");
        }
        string IFileUploadService.UpdateProfileImage(IFormFile file, string filePath)
        {

            if (file == null || file.Length == 0)
            {
                return filePath;
            }

            int maxFileSize = 1024 * 1024 / 2;
            var fileSize = file.Length;

            if (fileSize > maxFileSize)
            {
                throw new ArgumentException($"file limit exceeded, greater than {maxFileSize}");
            }
            if (file.FileName.EndsWith(".jpg")
                        || file != null && file.Length > 0 || file.FileName.EndsWith(".jpg")
                        || file.FileName.EndsWith(".jpeg") || file.FileName.EndsWith(".png"))
            {
                string ext = Path.GetExtension(file.FileName);
                string fileName = Guid.NewGuid().ToString() + ext;

                bool fileExists = File.Exists(filePath);
                if (fileExists)
                {
                    File.Delete(filePath);
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        fileStream.Position = 0;
                        file.CopyTo(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                    }
                }
                else
                {
                    filePath = Path.Combine(environment.ContentRootPath, "wwwroot/" + ProfileImagePath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        fileStream.Position = 0;
                        file.CopyTo(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                    }
                }
                var host = accessor.HttpContext.Request.Host.ToUriComponent();
                var url = $"{accessor.HttpContext.Request.Scheme}://{host}/{ProfileImagePath}/{fileName}";
                return url;
            }
            throw new ArgumentException("Invalid Profile Image");
        }
        string IFileUploadService.UploadPrincipalStamp(IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                return "";
            }
            int maxFileSize = 1024 * 1024 / 2;
            var fileSize = file.Length;

            if (fileSize > maxFileSize)
            {
                throw new ArgumentException($"file limit exceeded, greater than {maxFileSize}");
            }

            if (file.FileName.EndsWith(".jpg")
                        || file != null && file.Length > 0 || file.FileName.EndsWith(".jpg")
                        || file.FileName.EndsWith(".jpeg") || file.FileName.EndsWith(".png"))
            {
                string extension = Path.GetExtension(file.FileName);
                string fileName = Guid.NewGuid().ToString() + extension;

                var filePath = Path.Combine(environment.ContentRootPath, "wwwroot/" + PrincipalStampPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    fileStream.Position = 0;
                    file.CopyTo(fileStream);
                    fileStream.Flush();
                    fileStream.Close();
                }

                var host = accessor.HttpContext.Request.Host.ToUriComponent();
                var url = $"{accessor.HttpContext.Request.Scheme}://{host}/{PrincipalStampPath}/{fileName}";
                return url;
            }
            throw new ArgumentException("Invalid Principal Stamp");
        }
        string IFileUploadService.UpdatePrincipalStamp(IFormFile file, string filePath)
        {

            if (file == null || file.Length == 0)
            {
                return filePath;
            }
            int maxFileSize = 1024 * 1024 / 2;
            var fileSize = file.Length;
            if (fileSize > maxFileSize)
            {
                throw new ArgumentException($"file limit exceeded, greater than {maxFileSize}");
            }
            if (file.FileName.EndsWith(".jpg")
                        || file != null && file.Length > 0 || file.FileName.EndsWith(".jpg")
                        || file.FileName.EndsWith(".jpeg") || file.FileName.EndsWith(".png"))
            {
                string ext = Path.GetExtension(file.FileName);
                string fileName = Guid.NewGuid().ToString() + ext;

                bool fileExists = File.Exists(filePath);
                if (fileExists)
                {
                    File.Delete(filePath);
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        fileStream.Position = 0;
                        file.CopyTo(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                    }
                }
                else
                {
                    filePath = Path.Combine(environment.ContentRootPath, "wwwroot/" + PrincipalStampPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        fileStream.Position = 0;
                        file.CopyTo(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                    }
                }
                var host = accessor.HttpContext.Request.Host.ToUriComponent();
                var url = $"{accessor.HttpContext.Request.Scheme}://{host}/{PrincipalStampPath}/{fileName}";
                return url;
            }
            throw new ArgumentException("Invalid Principal Stamp");
        }
        string IFileUploadService.UploadSchoolLogo(IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                return "";
            }

            int maxFileSize = 1024 * 1024 / 2;
            var fileSize = file.Length;

            if (fileSize > maxFileSize)
            {
                throw new ArgumentException($"file limit exceeded, greater than {maxFileSize}");
            }

            if (file.FileName.EndsWith(".jpg")
                        || file != null && file.Length > 0 || file.FileName.EndsWith(".jpg")
                        || file.FileName.EndsWith(".jpeg") || file.FileName.EndsWith(".png"))
            {
                string extension = Path.GetExtension(file.FileName);
                string fileName = Guid.NewGuid().ToString() + extension;

                var filePath = Path.Combine(environment.ContentRootPath, "wwwroot/" + SchoolLogoPath, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    fileStream.Position = 0;
                    file.CopyTo(fileStream);
                    fileStream.Flush();
                    fileStream.Close();
                }

                var host = accessor.HttpContext.Request.Host.ToUriComponent();
                var url = $"{accessor.HttpContext.Request.Scheme}://{host}/{SchoolLogoPath}/{fileName}";
                return url;
            }
            throw new ArgumentException("Invalid School Logo");
        }
        string IFileUploadService.UpdateSchoolLogo(IFormFile file, string filePath)
        {
            if (file == null || file.Length == 0)
            {
                return filePath;
            }

            int maxFileSize = 1024 * 1024 / 2;
            var fileSize = file.Length;

            if (fileSize > maxFileSize)
            {
                throw new ArgumentException($"file limit exceeded, greater than {maxFileSize}");

            }
            if (file.FileName.EndsWith(".jpg")
                        || file != null && file.Length > 0 || file.FileName.EndsWith(".jpg")
                        || file.FileName.EndsWith(".jpeg") || file.FileName.EndsWith(".png"))
            {
                string ext = Path.GetExtension(file.FileName);
                string fileName = Guid.NewGuid().ToString() + ext;

                bool fileExists = File.Exists(filePath);
                if (fileExists)
                {
                    File.Delete(filePath);
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        fileStream.Position = 0;
                        file.CopyTo(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                    }
                }
                else
                {
                    filePath = Path.Combine(environment.ContentRootPath, "wwwroot/" + SchoolLogoPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        fileStream.Position = 0;
                        file.CopyTo(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                    }
                }
                var host = accessor.HttpContext.Request.Host.ToUriComponent();
                var url = $"{accessor.HttpContext.Request.Scheme}://{host}/{SchoolLogoPath}/{fileName}";
                return url;
            }
            throw new ArgumentException("Invalid School Logo");
        }
        string IFileUploadService.UploadLessonNote(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName);
            string fileName = Guid.NewGuid().ToString() + extension;
            string filepath = Path.Combine(environment.ContentRootPath, "wwwroot/" + LessonNotePath, fileName);
            if (file == null || file.Length == 0)
            {
                return filepath;
            }
            if (file.FileName.EndsWith(".pdf")
                        || file != null && file.Length > 0 || file.FileName.EndsWith(".docx")
                        || file.FileName.EndsWith(".txt"))
            {
                bool fileExists = File.Exists(filepath);
                if (fileExists)
                {
                    File.Delete(filepath);
                    using (var fileStream = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        fileStream.Position = 0;
                        file.CopyTo(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                    }
                }
                else
                {
                    var filePath = Path.Combine(environment.ContentRootPath, "wwwroot/" + LessonNotePath, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        fileStream.Position = 0;
                        file.CopyTo(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                    }
                    var host = accessor.HttpContext.Request.Host.ToUriComponent();
                    var url = $"{accessor.HttpContext.Request.Scheme}://{host}/{LessonNotePath}/{fileName}";
                    string textNote = string.Empty;
                    if (extension.Equals(".pdf"))
                        textNote = ReadTextForPdf(filePath);
                    else if (extension.Equals(".txt"))
                        textNote = ReadTextForTxt(filePath, fileName);
                    else
                        textNote = ReadTextForDocx(filePath);
                    File.Delete(filepath);

                    return textNote;
                }

            }
            throw new ArgumentException("Invalid file format");
            return "";
        } 
        private string ReadTextForTxt(string filePath,string fileName)
        {
            string text;
            if (filePath == null) { throw new ArgumentNullException("file does not exist"); }
            var fileStream = new FileStream(Path.Combine(environment.ContentRootPath, "wwwroot/" + LessonNotePath, fileName), FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                text = streamReader.ReadToEnd();
                return text;
            }
            return "";
        }

        private string ReadTextForPdf(string filePath)
        {
            if (filePath == null) { throw new ArgumentNullException("file does not exist"); }
                StringBuilder text = new StringBuilder();
            using (PdfReader reader = new PdfReader(filePath))
            {
                for (int i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, i));
                }
            }

            return text.ToString();
        }
        private string ReadTextForDocx(string filePath)
        {

            var filePathAsString = filePath as string;
            if (string.IsNullOrEmpty(filePathAsString))
            {
                throw new ArgumentNullException("filePath");
            }

            if (!File.Exists(filePathAsString))
            {
                throw new FileNotFoundException("Could not find file", filePathAsString);
            }

            var textFromWordDocument = string.Empty;
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            Microsoft.Office.Interop.Word.Document wordDocument = null;
            Microsoft.Office.Interop.Word.Range wordContentRange = null;

            try
            {
                wordDocument = wordApp.Documents.Open(filePath, Missing.Value, true);
                wordContentRange = wordDocument.Content;
                textFromWordDocument = wordContentRange.Text;
            }
            catch
            {
                // handle the COM exception
            }

            return textFromWordDocument;
           // return "";
        }

    }

}
