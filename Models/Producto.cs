namespace TechStore.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string Imagen { get; set; } = string.Empty;
        public int Stock { get; set; }
        public bool Estado { get; set; } // true = Disponible, false = Agotado
        public bool Destacado { get; set; }
    }
}