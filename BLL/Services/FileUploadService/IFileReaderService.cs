using System.IO;

namespace SMP.BLL.Services.FileUploadService
{
    public interface IFileReaderService
    {/*
        string ReadTextForTxt(FileStream fileStream);
        string ReadTextForPdf(string filePath);
        string ReadTextForDocx(string filePath);*/
        string ReadFile(string filepath);
    }
}