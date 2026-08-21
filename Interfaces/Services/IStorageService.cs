using Microsoft.AspNetCore.Http;

namespace TechStore.Interfaces.Services
{
    public interface IStorageService
    {
        void ValidateFile(IFormFile file);
        string GetAbsoluteDirectoryPath(string subDirectory = "");
        int GetNextFileNumber(string directoryPath);
        Task SaveFileToDiskAsync(IFormFile file, string destinationPath);
        void DeleteFile(string relativePath);
        void DeleteDirectory(string subDirectory);
    }
}
