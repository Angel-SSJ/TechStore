using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TechStore.Interfaces.Services;

namespace TechStore.Services
{
    public class StorageService : IStorageService
    {
        protected readonly IWebHostEnvironment _environment;
        protected virtual string BaseDirectory { get; } = "uploads";
        protected virtual string[] AllowedExtensions { get; } = { ".webp", ".jpg", ".jpeg", ".png" };
        protected virtual long MaxFileSize { get; } = 5 * 1024 * 1024; // 5MB

        public StorageService(IWebHostEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public virtual void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("El archivo es requerido.", nameof(file));
            }

            if (file.Length > MaxFileSize)
            {
                throw new ArgumentException($"El archivo no debe exceder {MaxFileSize / (1024 * 1024)}MB.");
            }

            string extension = Path.GetExtension(file.FileName).ToLower();
            if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
            {
                throw new ArgumentException($"Solo se permiten archivos con extensiones: {string.Join(", ", AllowedExtensions)}");
            }
        }

        public virtual string GetAbsoluteDirectoryPath(string subDirectory = "")
        {
            string webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            return string.IsNullOrWhiteSpace(subDirectory)
                ? Path.Combine(webRoot, BaseDirectory)
                : Path.Combine(webRoot, BaseDirectory, subDirectory);
        }

        public virtual int GetNextFileNumber(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                return 1;
            }

            var files = Directory.GetFiles(directoryPath);
            return files.Length + 1;
        }

        public virtual async Task SaveFileToDiskAsync(IFormFile file, string destinationPath)
        {
            var directory = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using var stream = new FileStream(destinationPath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        public virtual void DeleteFile(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
            {
                return;
            }

            string webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string filePath = Path.Combine(webRoot, relativePath.TrimStart('/', '\\'));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            string? directoryPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directoryPath) &&
                Directory.Exists(directoryPath) &&
                Directory.GetFiles(directoryPath).Length == 0 &&
                Directory.GetDirectories(directoryPath).Length == 0)
            {
                Directory.Delete(directoryPath);
            }
        }

        public virtual void DeleteDirectory(string subDirectory)
        {
            string directoryPath = GetAbsoluteDirectoryPath(subDirectory);
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, recursive: true);
            }
        }
    }
}
