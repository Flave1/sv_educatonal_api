using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SMP.BLL.Services.FileUploadService
{
    public class FileReaderService : IFileReaderService
    {
        private readonly IWebHostEnvironment environment;
        private readonly IHttpContextAccessor accessor;
        public FileReaderService(IWebHostEnvironment environment, IHttpContextAccessor accessor)
        {
            this.environment = environment;
            this.accessor = accessor;
        }

        string IFileReaderService.ReadTextForTxt(FileStream fileStream)
        {
            string text;
            
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                text = streamReader.ReadToEnd();
                return text;
            }
        }

        string IFileReaderService.ReadTextForPdf(string filePath)
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
        string IFileReaderService.ReadTextForDocx(string filePath)
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
