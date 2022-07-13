﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.FileUpload
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment environment;
        private readonly IHttpContextAccessor accessor;
        private static string ProfileImagePath = "ProfileImage";
        public FileUploadService(IWebHostEnvironment environment, IHttpContextAccessor httpContext)
        {
            this.environment = environment;
            this.accessor = httpContext;
        }
        string IFileUploadService.UploadProfileImage(IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                return "";
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
                return "";
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
        
    }
}