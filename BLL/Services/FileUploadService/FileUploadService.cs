﻿using iTextSharp.text.pdf;
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
        private readonly IFileReaderService fileReader;
        private static string ProfileImagePath = "ProfileImage";
        private static string SchoolLogoPath = "SchoolLogo";
        private static string PrincipalStampPath = "PrincipalStamp";
        private static string LessonNotePath = "LessonNote";
        public FileUploadService(IWebHostEnvironment environment, IHttpContextAccessor httpContext, IFileReaderService fileReader)
        {
            this.environment = environment;
            accessor = httpContext;
            this.fileReader = fileReader;
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
        async Task<string> IFileUploadService.RetunFileContent(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName);
            string fileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(environment.ContentRootPath, "wwwroot/" + LessonNotePath, fileName);
            if (file == null || file.Length == 0)
            {
                return "";
            }
            if (file.FileName.EndsWith(".pdf")
                        || file != null && file.Length > 0 || file.FileName.EndsWith(".docx")
                        || file.FileName.EndsWith(".txt"))
            {

                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        fileStream.Position = 0;
                        file.CopyTo(fileStream);
                        fileStream.Flush();
                        fileStream.Close();
                    }
                    var host = accessor.HttpContext.Request.Host.ToUriComponent();
                    var url = $"{accessor.HttpContext.Request.Scheme}://{host}/{LessonNotePath}/{fileName}";
                    var content = await (this as IFileUploadService).ReadFileAsync(fileName, extension, url);
                    (this as IFileUploadService).DeleteFile(filePath);
                    return content;
                }
                catch (Exception)
                {
                    (this as IFileUploadService).DeleteFile(filePath);
                }

            }
            throw new ArgumentException("Invalid file format");
        } 
       
        void IFileUploadService.DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        async Task<string> IFileUploadService.ReadFileAsync(string fileName, string extension, string filePath)
        {
            var fileStream = new FileStream(Path.Combine(environment.ContentRootPath, "wwwroot/" + LessonNotePath, fileName), FileMode.Open, FileAccess.Read);
            string noteContent = string.Empty;
            if (extension.Equals(".pdf"))
                noteContent = fileReader.ReadTextForPdf(filePath);
            else if (extension.Equals(".txt"))
                noteContent = fileReader.ReadTextForTxt(fileStream);
            else
                noteContent = fileReader.ReadTextForDocx(filePath);

            fileStream.Flush();
            fileStream.Close();
            return await Task.Run(() => noteContent);
        }
    }

}
