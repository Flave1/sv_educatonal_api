using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Text;
//using Code7248.word_reader;

namespace SMP.BLL.Helpers
{
    public class ReadFromFile
    {
        public static string ReadFile(string filePath, string fileExtension)
        {
            if (filePath == null) { throw new ArgumentNullException("file does not exist"); }
            else if (fileExtension == ".pdf")
            {
                StringBuilder text = new StringBuilder();
                using (PdfReader reader = new PdfReader(filePath))
                {
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                    }
                }

                return text.ToString();
            }
            else if (fileExtension == ".docx")
            {
               
            }
            else if (fileExtension == ".txt")
            {
                string text;
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    text = streamReader.ReadToEnd();
                }

                return text;
            }
            return "";
        }
    }
}
