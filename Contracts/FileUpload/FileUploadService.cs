using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMP.Contracts.FileUpload
{
    public class FileUploadService : IFileUpload
    {
        private readonly IWebHostEnvironment environment;
        public FileUploadService(IWebHostEnvironment environment)
        {
            this.environment = environment;
        }
        public string Upload(IFormFile file)
        {
            if (file != null && file.Length > 0 && (file.FileName.EndsWith(".jpg")
                        || file != null && file.Length > 0 || file.FileName.EndsWith(".jpg")
                        || file.FileName.EndsWith(".jpeg") || file.FileName.EndsWith(".png"))) 
            {
                string ext = Path.GetExtension(file.FileName);
                string fileName = Guid.NewGuid().ToString() + ext;

                var filePath = Path.Combine(environment.ContentRootPath, "Uploads", fileName);

                bool exists = System.IO.Directory.Exists(filePath);
                 
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    fileStream.Position = 0;
                    file.CopyTo(fileStream);
                    fileStream.Flush();
                    fileStream.Close();
                }

                return filePath;
            }
            return null;
        }
    }
}
