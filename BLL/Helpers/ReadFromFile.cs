using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Text;

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
                StringBuilder text = new StringBuilder();
                Microsoft.Office.Interop.Word.Application word = new Microsoft.Office.Interop.Word.Application();
                object miss = System.Reflection.Missing.Value;
                object path = filePath;
                object readOnly = true;
                Microsoft.Office.Interop.Word.Document docs = word.Documents.Open(ref path, ref miss, ref readOnly, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss, ref miss);

                for (int i = 0; i < docs.Paragraphs.Count; i++)
                {
                    text.Append(" \r\n " + docs.Paragraphs[i + 1].Range.Text.ToString());
                }

                return text.ToString();
            }
            else if (fileExtension == ".txt")
            {
                string text = System.IO.File.ReadAllText(filePath);

                return text.ToString();
            }
            return "";
        }
    }
}
