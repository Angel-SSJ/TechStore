using System.ComponentModel.DataAnnotations;

namespace TechStore.Models
{
    public class ContactoViewModel
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
        [Display(Name = "Correo electrónico")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El asunto es obligatorio.")]
        public string Asunto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El mensaje es obligatorio.")]
        [DataType(DataType.MultilineText)]
        public string Mensaje { get; set; } = string.Empty;
    }
}