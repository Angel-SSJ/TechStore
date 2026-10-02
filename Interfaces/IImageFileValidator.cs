using Microsoft.AspNetCore.Http;

namespace TechStore.Interfaces
{
    public interface IImageFileValidator
    {
        void Validate(IFormFile file);
    }
}
