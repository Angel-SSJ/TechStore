using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using TechStore.Data;
using TechStore.Models;

namespace TechStore.Data.Seeders
{
    public class ProductSeeder : IDataSeeder
    {
        public int Order => 2;

        private readonly IWebHostEnvironment _environment;
        private static readonly byte[] ValidWebpBytes = new byte[]
        {
            0x52, 0x49, 0x46, 0x46, 0x24, 0x00, 0x00, 0x00,
            0x57, 0x45, 0x42, 0x50, 0x56, 0x50, 0x38, 0x4C,
            0x17, 0x00, 0x00, 0x00, 0x2F, 0x00, 0x00, 0x00,
            0x00, 0x07, 0x88, 0x85, 0x85, 0xFE, 0x8A, 0x9A,
            0x56, 0x54, 0x2A, 0x40, 0x36, 0x24, 0xA1, 0x48,
            0x30, 0x12, 0x00
        };

        public ProductSeeder(IWebHostEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Products.AnyAsync())
            {
                return;
            }

            var categories = await context.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);

            Guid GetCatId(string categoryName)
            {
                if (categories.TryGetValue(categoryName, out var id))
                {
                    return id;
                }
                throw new InvalidOperationException($"La categoría requerida '{categoryName}' no existe en la base de datos.");
            }

            var catLaptops = GetCatId("Laptops & MacBooks");
            var catSmartphones = GetCatId("Smartphones & iPhones");
            var catTablets = GetCatId("Tablets & iPads");
            var catCases = GetCatId("Fundas & Protectores (Cases)");
            var catCargadores = GetCatId("Cargadores & Cables");
            var catWearables = GetCatId("Smartwatches & Wearables");
            var catAudio = GetCatId("Audio & Auriculares");
            var catMonitores = GetCatId("Monitores & Pantallas");
            var catAlmacenamiento = GetCatId("Almacenamiento & Memorias");
            var catPerifericos = GetCatId("Periféricos & Gaming");

            string webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var rawProducts = new List<ProductSeedItem>
            {
                // 1. Laptops & MacBooks
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000001"),
                    Name = "MacBook Pro 16\" M3 Max (2024)",
                    Description = "Potencia extrema con chip M3 Max (CPU 16 núcleos, GPU 40 núcleos), 48 GB de memoria unificada, 1 TB SSD y pantalla Liquid Retina XDR de 16.2\".",
                    Price = 3999.00m,
                    Stock = 10,
                    CategoryIds = new[] { catLaptops },
                    ImageOriginalNames = new[] { "macbook_pro_16_front.webp", "macbook_pro_16_open.webp", "macbook_pro_16_side.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000002"),
                    Name = "MacBook Air 15\" Chip M3 - Medianoche",
                    Description = "Diseño ultradelgado de 15.3\" con pantalla Liquid Retina, chip M3 de Apple, 16 GB RAM, 512 GB SSD y hasta 18 horas de batería.",
                    Price = 1499.00m,
                    Stock = 15,
                    CategoryIds = new[] { catLaptops },
                    ImageOriginalNames = new[] { "macbook_air_15_midnight.webp", "macbook_air_15_side.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000003"),
                    Name = "Samsung Galaxy Book4 Ultra 16\"",
                    Description = "Laptop premium con Intel Core Ultra 9, NVIDIA GeForce RTX 4070, pantalla táctil Dynamic AMOLED 2X 120Hz y 32 GB RAM / 1 TB SSD.",
                    Price = 2899.00m,
                    Stock = 8,
                    CategoryIds = new[] { catLaptops, catPerifericos },
                    ImageOriginalNames = new[] { "galaxy_book4_ultra_front.webp", "galaxy_book4_ultra_open.webp", "galaxy_book4_ultra_angle.webp" }
                },

                // 2. Smartphones & iPhones
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000004"),
                    Name = "iPhone 16 Pro Max 256GB - Titanio Natural",
                    Description = "Carcasa de titanio de grado aeroespacial, chip A18 Pro, botón de Control de Cámara, teleobjetivo 5x de 48 MP y pantalla de 6.9\".",
                    Price = 1199.00m,
                    Stock = 25,
                    CategoryIds = new[] { catSmartphones },
                    ImageOriginalNames = new[] { "iphone_16_pro_max_front.webp", "iphone_16_pro_max_back.webp", "iphone_16_pro_max_cameras.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000005"),
                    Name = "iPhone 16 128GB - Verde Azulado",
                    Description = "Equipado con chip A18, Control de Cámara táctil, cámara Fusion de 48 MP con teleobjetivo 2x y botón de Acción.",
                    Price = 799.00m,
                    Stock = 30,
                    CategoryIds = new[] { catSmartphones },
                    ImageOriginalNames = new[] { "iphone_16_teal_front.webp", "iphone_16_teal_back.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000006"),
                    Name = "Samsung Galaxy S24 Ultra 512GB - Gris Titanio",
                    Description = "Con Galaxy AI integrado, pantalla plana Dynamic AMOLED 2X de 6.8\", cámara de 200 MP y S-Pen integrado.",
                    Price = 1299.00m,
                    Stock = 20,
                    CategoryIds = new[] { catSmartphones },
                    ImageOriginalNames = new[] { "galaxy_s24_ultra_front.webp", "galaxy_s24_ultra_spen.webp", "galaxy_s24_ultra_cameras.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000007"),
                    Name = "Samsung Galaxy Z Fold6 512GB - Plata Sombra",
                    Description = "Plegable ultraligero con pantalla interior AMOLED de 7.6\", Snapdragon 8 Gen 3 y funciones exclusivas de productividad AI.",
                    Price = 1899.00m,
                    Stock = 7,
                    CategoryIds = new[] { catSmartphones, catTablets },
                    ImageOriginalNames = new[] { "galaxy_z_fold6_folded.webp", "galaxy_z_fold6_unfolded.webp", "galaxy_z_fold6_side.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000008"),
                    Name = "Samsung Galaxy Z Flip6 256GB - Azul",
                    Description = "Plegable compacto con cámara de 50 MP, pantalla exterior interactiva FlexWindow de 3.4\" y batería de 4,000 mAh.",
                    Price = 999.00m,
                    Stock = 14,
                    CategoryIds = new[] { catSmartphones },
                    ImageOriginalNames = new[] { "galaxy_z_flip6_front.webp", "galaxy_z_flip6_flex.webp" }
                },

                // 3. Tablets & iPads
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000009"),
                    Name = "iPad Pro 13\" Chip M4 OLED 256GB - Negro Espacial",
                    Description = "Diseño ultradelgado con pantalla Ultra Retina XDR OLED en tándem, chip M4 y compatibilidad con Apple Pencil Pro.",
                    Price = 1299.00m,
                    Stock = 12,
                    CategoryIds = new[] { catTablets },
                    ImageOriginalNames = new[] { "ipad_pro_13_m4_front.webp", "ipad_pro_13_m4_back.webp", "ipad_pro_13_m4_pencil.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000010"),
                    Name = "iPad Air 11\" Chip M2 128GB - Azul Estelar",
                    Description = "Pantalla Liquid Retina de 11\", chip M2 ultrarrápido, cámara frontal horizontal de 12 MP con Encuadre Centrado y Touch ID.",
                    Price = 599.00m,
                    Stock = 18,
                    CategoryIds = new[] { catTablets },
                    ImageOriginalNames = new[] { "ipad_air_11_m2_front.webp", "ipad_air_11_m2_back.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000011"),
                    Name = "Samsung Galaxy Tab S9 Ultra 512GB con S-Pen",
                    Description = "Tablet de 14.6\" Dynamic AMOLED 2X 120Hz con certificación IP68 contra agua y polvo, Snapdragon 8 Gen 2 y S-Pen incluido.",
                    Price = 1199.00m,
                    Stock = 9,
                    CategoryIds = new[] { catTablets },
                    ImageOriginalNames = new[] { "galaxy_tab_s9_ultra_front.webp", "galaxy_tab_s9_ultra_spen.webp" }
                },

                // 4. Fundas & Protectores (Cases)
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000012"),
                    Name = "Funda de Silicón con MagSafe para iPhone 16 Pro Max - Negro",
                    Description = "Exterior de silicón de tacto suave, forro de microfibra e imanes alineados para carga MagSafe óptima.",
                    Price = 49.00m,
                    Stock = 50,
                    CategoryIds = new[] { catCases },
                    ImageOriginalNames = new[] { "case_iphone16_silicone_black.webp", "case_iphone16_silicone_inside.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000013"),
                    Name = "Funda Transparente Antiamarilleo con MagSafe para iPhone 16",
                    Description = "Policarbonato de alta claridad con absorción contra impactos de 3 metros y protección UV contra decoloración.",
                    Price = 39.00m,
                    Stock = 60,
                    CategoryIds = new[] { catCases },
                    ImageOriginalNames = new[] { "case_iphone16_clear_front.webp", "case_iphone16_clear_back.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000014"),
                    Name = "Funda Standing Grip Original para Samsung Galaxy S24 Ultra",
                    Description = "Funda oficial con correa retráctil integrada que funciona como soporte horizontal de manos libres.",
                    Price = 54.99m,
                    Stock = 40,
                    CategoryIds = new[] { catCases },
                    ImageOriginalNames = new[] { "case_s24_grip_front.webp", "case_s24_grip_strap.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000015"),
                    Name = "Funda Spigen Tough Armor para Galaxy Z Fold6",
                    Description = "Doble capa de absorción de impactos con tecnología Air Cushion y protección total de bisagra.",
                    Price = 59.99m,
                    Stock = 35,
                    CategoryIds = new[] { catCases },
                    ImageOriginalNames = new[] { "case_fold6_spigen_front.webp", "case_fold6_spigen_hinge.webp", "case_fold6_spigen_kickstand.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000016"),
                    Name = "Magic Keyboard para iPad Pro 13\" (M4) - Español",
                    Description = "Soporte flotante de aluminio, trackpad háptico de vidrio, 14 teclas de función y conector USB-C pass-through.",
                    Price = 349.00m,
                    Stock = 15,
                    CategoryIds = new[] { catCases, catPerifericos },
                    ImageOriginalNames = new[] { "magic_keyboard_ipad_open.webp", "magic_keyboard_ipad_closed.webp" }
                },

                // 5. Cargadores & Cables
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000017"),
                    Name = "Adaptador de Corriente Dual USB-C 35W Apple",
                    Description = "Carga dos dispositivos simultáneamente con clavijas plegables compactas y distribución inteligente de energía.",
                    Price = 59.00m,
                    Stock = 45,
                    CategoryIds = new[] { catCargadores },
                    ImageOriginalNames = new[] { "apple_charger_35w_front.webp", "apple_charger_35w_ports.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000018"),
                    Name = "Cargador GaN 100W 4 Puertos Anker Prime",
                    Description = "Tecnología GaNPrime con 3 puertos USB-C y 1 USB-A, capaz de alimentar MacBook Pro, iPhone y reloj simultáneamente.",
                    Price = 89.99m,
                    Stock = 30,
                    CategoryIds = new[] { catCargadores },
                    ImageOriginalNames = new[] { "anker_prime_100w_front.webp", "anker_prime_100w_ports.webp", "anker_prime_100w_scale.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000019"),
                    Name = "Estación de Carga Inalámbrica 3 en 1 MagSafe Belkin BoostCharge",
                    Description = "Carga rápida inalámbrica oficial de 15W para iPhone, Apple Watch y estuche de AirPods en acabado de acero pulido.",
                    Price = 149.99m,
                    Stock = 20,
                    CategoryIds = new[] { catCargadores },
                    ImageOriginalNames = new[] { "belkin_3in1_magsafe_front.webp", "belkin_3in1_magsafe_devices.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000020"),
                    Name = "Cable USB-C a USB-C Trenzado 240W 2 Metros",
                    Description = "Cable reforzado con soporte Power Delivery 3.1 hasta 240W para carga ultrarrápida de laptops y teléfonos.",
                    Price = 29.00m,
                    Stock = 80,
                    CategoryIds = new[] { catCargadores },
                    ImageOriginalNames = new[] { "cable_usbc_240w_coil.webp", "cable_usbc_240w_connector.webp" }
                },

                // 6. Smartwatches & Wearables
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000021"),
                    Name = "Apple Watch Ultra 2 GPS + Cellular 49mm - Titanio Negro",
                    Description = "Caja de titanio negro de 49 mm, pantalla de 3,000 nits, GPS de doble frecuencia y hasta 72 horas en modo ahorro.",
                    Price = 799.00m,
                    Stock = 10,
                    CategoryIds = new[] { catWearables },
                    ImageOriginalNames = new[] { "apple_watch_ultra_2_front.webp", "apple_watch_ultra_2_side.webp", "apple_watch_ultra_2_loop.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000022"),
                    Name = "Apple Watch Series 10 GPS 46mm - Aluminio Negro Azabache",
                    Description = "Pantalla OLED gran angular más delgada, sensor de apnea del sueño y carga rápida al 80% en 30 minutos.",
                    Price = 429.00m,
                    Stock = 22,
                    CategoryIds = new[] { catWearables },
                    ImageOriginalNames = new[] { "apple_watch_s10_front.webp", "apple_watch_s10_angle.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000023"),
                    Name = "Samsung Galaxy Watch Ultra 47mm LTE - Titanio Gris",
                    Description = "Marco de titanio grado aeroespacial, monitor de composición corporal BIA, GPS dual y resistencia al agua 10 ATM.",
                    Price = 649.99m,
                    Stock = 12,
                    CategoryIds = new[] { catWearables },
                    ImageOriginalNames = new[] { "galaxy_watch_ultra_front.webp", "galaxy_watch_ultra_strap.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000024"),
                    Name = "Samsung Galaxy Ring - Titanio Negro Talla 10",
                    Description = "Monitoreo discreto de sueño, frecuencia cardíaca y temperatura corporal con hasta 7 días continuos de batería.",
                    Price = 399.99m,
                    Stock = 15,
                    CategoryIds = new[] { catWearables },
                    ImageOriginalNames = new[] { "galaxy_ring_black_iso.webp", "galaxy_ring_black_case.webp" }
                },

                // 7. Audio & Auriculares
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000025"),
                    Name = "AirPods Pro (2.ª gen) con estuche MagSafe USB-C",
                    Description = "Cancelación Activa de Ruido 2x superior, Audio Espacial personalizado y chip H2 con Audio Adaptativo.",
                    Price = 249.00m,
                    Stock = 40,
                    CategoryIds = new[] { catAudio },
                    ImageOriginalNames = new[] { "airpods_pro_2_case.webp", "airpods_pro_2_earbuds.webp", "airpods_pro_2_magsafe.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000026"),
                    Name = "AirPods Max - Azul Cielo con USB-C",
                    Description = "Transductores de alta fidelidad, diadema de malla transpirable, cancelación activa de ruido y modo Ambiente.",
                    Price = 549.00m,
                    Stock = 11,
                    CategoryIds = new[] { catAudio },
                    ImageOriginalNames = new[] { "airpods_max_blue_front.webp", "airpods_max_blue_side.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000027"),
                    Name = "Samsung Galaxy Buds3 Pro - Plata",
                    Description = "Audio Hi-Fi de 24 bits a 96 kHz con diseño Blade Lights, cancelación adaptativa de ruido AI y altavoces duales de 2 vías.",
                    Price = 249.99m,
                    Stock = 28,
                    CategoryIds = new[] { catAudio },
                    ImageOriginalNames = new[] { "galaxy_buds3_pro_silver_case.webp", "galaxy_buds3_pro_silver_buds.webp" }
                },

                // 8. Monitores & Pantallas
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000028"),
                    Name = "Apple Studio Display 27\" 5K - Vidrio Estándar",
                    Description = "Panel Retina 5K de 27\", 600 nits, cámara Ultra Gran Angular de 12 MP con Encuadre Centrado y 6 bocinas con Audio Espacial.",
                    Price = 1599.00m,
                    Stock = 6,
                    CategoryIds = new[] { catMonitores },
                    ImageOriginalNames = new[] { "studio_display_front.webp", "studio_display_ports.webp", "studio_display_tilt.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000029"),
                    Name = "Samsung Odyssey OLED G9 49\" Curvo 240Hz",
                    Description = "Pantalla curva 1800R Dual QHD (5120x1440) con 0.03ms de respuesta, procesador Neo Quantum y Smart Hub integrado.",
                    Price = 1499.99m,
                    Stock = 4, // Stock bajo
                    CategoryIds = new[] { catMonitores, catPerifericos },
                    ImageOriginalNames = new[] { "odyssey_g9_front.webp", "odyssey_g9_rgb.webp" }
                },

                // 9. Almacenamiento & Memorias
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000030"),
                    Name = "SSD Portátil Samsung T9 2TB USB 3.2 Gen 2x2",
                    Description = "Velocidad de hasta 2,000 MB/s con recubrimiento de goma antichoque, resistencia a caídas de 3m y cifrado AES-256.",
                    Price = 219.99m,
                    Stock = 35,
                    CategoryIds = new[] { catAlmacenamiento },
                    ImageOriginalNames = new[] { "samsung_t9_front.webp", "samsung_t9_ports.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000031"),
                    Name = "SSD NVMe Kingston FURY Renegade 2TB con Disipador",
                    Description = "Almacenamiento PCIe 4.0 con lectura de 7,300 MB/s y escritura de 7,000 MB/s, listo para PC Master Race y PS5.",
                    Price = 179.99m,
                    Stock = 18,
                    CategoryIds = new[] { catAlmacenamiento, catPerifericos },
                    ImageOriginalNames = new[] { "kingston_fury_heatsink.webp", "kingston_fury_box.webp" }
                },

                // 10. Periféricos & Gaming
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000032"),
                    Name = "Magic Mouse Apple con superficie Multi-Touch - Negro",
                    Description = "Ratón inalámbrico recargable USB-C con soporte completo para gestos Multi-Touch en macOS.",
                    Price = 99.00m,
                    Stock = 30,
                    CategoryIds = new[] { catPerifericos },
                    ImageOriginalNames = new[] { "magic_mouse_black_top.webp", "magic_mouse_black_bottom.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000033"),
                    Name = "Teclado Mecánico Inalámbrico Logitech MX Mechanical Tactile",
                    Description = "Switches mecánicos táctiles silenciosos de bajo perfil, retroiluminación inteligente y conexión hasta 3 dispositivos.",
                    Price = 169.99m,
                    Stock = 22,
                    CategoryIds = new[] { catPerifericos },
                    ImageOriginalNames = new[] { "logitech_mx_mech_iso.webp", "logitech_mx_mech_top.webp", "logitech_mx_mech_switches.webp" }
                },
                new ProductSeedItem
                {
                    Id = Guid.Parse("a1000000-0000-0000-0000-000000000034"),
                    Name = "Ratón Ergonómico Inalámbrico Logitech MX Master 3S",
                    Description = "Sensor de 8,000 DPI con seguimiento en cristal, clics silenciosos y rueda electromagnética MagSpeed.",
                    Price = 99.99m,
                    Stock = 28,
                    CategoryIds = new[] { catPerifericos },
                    ImageOriginalNames = new[] { "logitech_mx_master_side.webp", "logitech_mx_master_top.webp" }
                }
            };

            var productEntities = new List<Product>();

            foreach (var item in rawProducts)
            {
                if (string.IsNullOrWhiteSpace(item.Name))
                {
                    throw new InvalidOperationException("El nombre del producto no puede estar vacío en el seeder.");
                }

                if (item.Price <= 0)
                {
                    throw new InvalidOperationException($"El precio del producto '{item.Name}' debe ser mayor a 0.");
                }

                if (item.Stock < 0)
                {
                    throw new InvalidOperationException($"El stock del producto '{item.Name}' no puede ser negativo.");
                }

                if (item.CategoryIds == null || item.CategoryIds.Length == 0)
                {
                    throw new InvalidOperationException($"El producto '{item.Name}' debe tener al menos una categoría asignada.");
                }

                var product = new Product
                {
                    Name = item.Name.Trim(),
                    Description = item.Description.Trim(),
                    Price = item.Price,
                    Stock = item.Stock,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                // Asignar ID predecible
                typeof(Product).GetProperty("Id")?.SetValue(product, item.Id);

                // Asignar categorías
                foreach (var catId in item.CategoryIds.Distinct())
                {
                    product.Categories.Add(new CategoryProduct
                    {
                        ProductId = product.Id,
                        CategoryId = catId
                    });
                }

                // Crear carpeta física en wwwroot/images/products/<id_product>/
                string productImagesDirectory = Path.Combine(webRoot, "images", "products", product.Id.ToString());
                if (!Directory.Exists(productImagesDirectory))
                {
                    Directory.CreateDirectory(productImagesDirectory);
                }

                // Generar archivos físicos e imágenes
                int imageSeq = 1;
                foreach (var originalName in item.ImageOriginalNames)
                {
                    string fileName = $"{imageSeq:D2}.webp";
                    string relativePath = $"images/products/{product.Id}/{fileName}";
                    string absoluteFilePath = Path.Combine(productImagesDirectory, fileName);

                    // Escribir archivo físico si no existe
                    if (!File.Exists(absoluteFilePath))
                    {
                        await File.WriteAllBytesAsync(absoluteFilePath, ValidWebpBytes);
                    }

                    var fileInfo = new FileInfo(absoluteFilePath);

                    product.Images.Add(new ProductImage
                    {
                        ProductId = product.Id,
                        ImagePath = relativePath,
                        ImageNumber = imageSeq,
                        OriginalFileName = originalName,
                        FileSize = fileInfo.Length > 0 ? fileInfo.Length : ValidWebpBytes.Length,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    });

                    imageSeq++;
                }

                productEntities.Add(product);
            }

            await context.Products.AddRangeAsync(productEntities);
            await context.SaveChangesAsync();
        }

        private sealed class ProductSeedItem
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public Guid[] CategoryIds { get; set; } = Array.Empty<Guid>();
            public string[] ImageOriginalNames { get; set; } = Array.Empty<string>();
        }
    }
}
