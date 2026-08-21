using TechStore.Models;

namespace TechStore.Data
{
    /// <summary>
    /// Fuente de datos en memoria
    /// </summary>
    public static class TechStoreData
    {
        public static List<Categoria> Categorias => new()
        {
            new Categoria { Id = 1, Nombre = "Laptops", Descripcion = "Equipos portátiles para trabajo, estudio y gaming.", Icono = "bi-laptop" },
            new Categoria { Id = 2, Nombre = "Smartphones", Descripcion = "Teléfonos inteligentes de última generación.", Icono = "bi-phone" },
            new Categoria { Id = 3, Nombre = "Audio", Descripcion = "Audífonos, parlantes y accesorios de sonido.", Icono = "bi-headphones" },
            new Categoria { Id = 4, Nombre = "Accesorios", Descripcion = "Periféricos y accesorios tecnológicos en general.", Icono = "bi-mouse2" },
        };

        public static List<Producto> Productos => new()
        {
            new Producto { Id = 1, Nombre = "Laptop UltraBook Pro 14\"", Descripcion = "Procesador de última generación, 16GB RAM, SSD 512GB. Ideal para trabajo y multitarea.", Precio = 899.99m, Categoria = "Laptops", Imagen = "/images/products/laptop-pro.jpg", Stock = 12, Estado = true, Destacado = true },
            new Producto { Id = 2, Nombre = "Laptop Gamer X15", Descripcion = "Tarjeta gráfica dedicada, pantalla 144Hz, teclado RGB. Rendimiento para gaming exigente.", Precio = 1299.00m, Categoria = "Laptops", Imagen = "/images/products/laptop-gamer.jpg", Stock = 5, Estado = true, Destacado = false },
            new Producto { Id = 3, Nombre = "Smartphone Nova 12", Descripcion = "Pantalla AMOLED 6.5\", cámara triple de 108MP y batería de larga duración.", Precio = 549.50m, Categoria = "Smartphones", Imagen = "/images/products/phone-nova.jpg", Stock = 20, Estado = true, Destacado = true },
            new Producto { Id = 4, Nombre = "Smartphone Lite S", Descripcion = "Opción económica sin sacrificar rendimiento. Perfecto para el día a día.", Precio = 259.00m, Categoria = "Smartphones", Imagen = "/images/products/phone-lite.jpg", Stock = 0, Estado = false, Destacado = false },
            new Producto { Id = 5, Nombre = "Audífonos Bluetooth SoundMax", Descripcion = "Cancelación activa de ruido y hasta 30 horas de batería.", Precio = 79.99m, Categoria = "Audio", Imagen = "/images/products/headphones.jpg", Stock = 35, Estado = true, Destacado = true },
            new Producto { Id = 6, Nombre = "Parlante Portátil BoomBox", Descripcion = "Sonido envolvente resistente al agua, ideal para exteriores.", Precio = 45.00m, Categoria = "Audio", Imagen = "/images/products/speaker.jpg", Stock = 18, Estado = true, Destacado = false },
            new Producto { Id = 7, Nombre = "Mouse Inalámbrico ErgoTech", Descripcion = "Diseño ergonómico, sensor de alta precisión y conexión dual.", Precio = 24.99m, Categoria = "Accesorios", Imagen = "/images/products/mouse.jpg", Stock = 40, Estado = true, Destacado = false },
            new Producto { Id = 8, Nombre = "Teclado Mecánico TypeFast", Descripcion = "Switches mecánicos, retroiluminación RGB y diseño compacto.", Precio = 59.99m, Categoria = "Accesorios", Imagen = "/images/products/keyboard.jpg", Stock = 22, Estado = true, Destacado = true },
        };

        public static List<Producto> ProductosDestacados =>
            Productos.Where(p => p.Destacado).ToList();
    }
}