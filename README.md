# TechStore - API & Backend

TechStore es una API REST y aplicacion web desarrollada con ASP.NET Core (.NET 10) y Entity Framework Core, orientada a la gestion de productos tecnologicos y categorias bajo una relacion muchos a muchos (N:M) con almacenamiento fisico de imagenes.

---

## 1. Requisitos

- .NET 10 SDK o superior.
- Microsoft SQL Server (instancia local, SQLEXPRESS o Docker).
- dotnet-ef CLI (opcional para manejo manual de migraciones):
  ```bash
  dotnet tool install --global dotnet-ef
  ```

---

## 2. Configuracion de Base de Datos

Edita la cadena de conexion en `appsettings.json` segun tu servidor SQL Server:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=RAMIREZ\\sqlexpress;Database=TechStore;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

Para instancias genericas o Docker:
- Servidor local: `Server=localhost;Database=TechStore;Trusted_Connection=True;TrustServerCertificate=True;`
- SQL Express: `Server=localhost\\sqlexpress;Database=TechStore;Trusted_Connection=True;TrustServerCertificate=True;`

---

## 3. Comandos para Levantar la Base de Datos

### Opcion A: Iniciar servicio local de SQL Server (PowerShell como Administrador)
```powershell
Start-Service -Name "MSSQLSERVER"
# O si usas SQL Express:
Start-Service -Name "MSSQL$SQLEXPRESS"
```

### Opcion B: Iniciar contenedor de SQL Server en Docker
```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=TuPasswordFuerte123!" -p 1433:1433 --name sqlserver_techstore -d mcr.microsoft.com/mssql/server:2022-latest
```

---

## 4. Comandos para Levantar la Aplicacion Web

### Levantar la aplicacion con migracion y sembrado automatico
```bash
dotnet run
```
Al iniciar en entorno Development:
1. Aplica automaticamente las migraciones pendientes en SQL Server.
2. Ejecuta los seeders (`CategorySeeder` y `ProductSeeder`), sembrando 10 categorias y 34 productos con sus imagenes.

### Forzar sembrado de datos por parametro
```bash
dotnet run --seed
```

### Ejecutar la suite automatizada de pruebas y generar logs
```bash
dotnet run -- --test
```

---

## 5. Comandos de Entity Framework Core (Migraciones Manuales)

### Aplicar migraciones a la base de datos
```bash
dotnet ef database update
```

### Crear una nueva migracion
```bash
dotnet ef migrations add NombreDeLaMigracion -o Data/Migrations
```

### Revertir la ultima migracion
```bash
dotnet ef migrations remove
```

---

## 6. Endpoints de la API

### Categorias (`/api/Category`)

| Metodo | Endpoint | Descripcion |
|---|---|---|
| `GET` | `/api/Category` | Lista categorias activas (`?includeInactive=true` para todas) |
| `GET` | `/api/Category/active` | Lista categorias activas |
| `GET` | `/api/Category/inactive` | Lista categorias inactivas (soft delete) |
| `GET` | `/api/Category/{id}` | Detalle de categoria por ID con productos asociados |
| `GET` | `/api/Category/by-name/{name}` | Busca categoria por nombre exacto |
| `GET` | `/api/Category/exists/{name}` | Retorna si el nombre ya existe |
| `POST` | `/api/Category` | Crea una nueva categoria |
| `PUT` | `/api/Category/{id}` | Actualiza nombre de la categoria |
| `DELETE` | `/api/Category/{id}` | Desactiva la categoria (Soft Delete) |
| `POST` | `/api/Category/{id}/restore` | Reactiva una categoria desactivada |

### Productos (`/api/Product`)

| Metodo | Endpoint | Descripcion |
|---|---|---|
| `GET` | `/api/Product` | Lista productos con filtros combinados (`?searchTerm=texto&categoryId={guid}`) |
| `GET` | `/api/Product/active` | Lista productos activos |
| `GET` | `/api/Product/inactive` | Lista productos inactivos |
| `GET` | `/api/Product/{id}` | Detalle de producto con nombres de categorias e imagenes (sin N+1) |
| `GET` | `/api/Product/by-category/{categoryId}` | Filtra productos por ID de categoria |
| `GET` | `/api/Product/search?term={texto}` | Busca productos por coincidencia en nombre |
| `GET` | `/api/Product/low-stock?threshold=5` | Retorna productos con stock menor o igual al umbral |
| `POST` | `/api/Product` | Crea producto (`multipart/form-data`: datos, CategoryIds y archivos Images) |
| `PUT` | `/api/Product/{id}` | Actualiza producto, sincroniza tabla CategoryProduct y agrega imagenes |
| `DELETE` | `/api/Product/{id}` | Desactiva el producto (Soft Delete) |
| `POST` | `/api/Product/{id}/restore` | Reactiva un producto desactivado |

### Gestion de Imagenes (`/api/Product/{id}/images`)

| Metodo | Endpoint | Descripcion |
|---|---|---|
| `POST` | `/api/Product/{id}/images` | Sube imagenes a `wwwroot/images/products/<id>/` (`multipart/form-data`) |
| `DELETE` | `/api/Product/{id}/images/{imageId}` | Elimina el registro en BD y borra el archivo fisico de disco |

---

## 7. Estructura de Archivos

```
TechStore/
├── Controllers/
│   ├── CategoryController.cs
│   ├── ProductController.cs
│   └── HomeController.cs
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── Migrations/
│   ├── Repositories/
│   │   ├── GenericRepository.cs
│   │   ├── CategoryRepository.cs
│   │   └── ProductRepository.cs
│   └── Seeders/
│       ├── IDataSeeder.cs
│       ├── CategorySeeder.cs
│       ├── ProductSeeder.cs
│       └── DbInitializer.cs
├── Interfaces/
│   ├── IEntity.cs
│   ├── Repositories/
│   └── Services/
├── Models/
│   ├── Entity.cs
│   ├── Category.cs
│   ├── Product.cs
│   ├── CategoryProduct.cs
│   ├── ProductImage.cs
│   └── DTOs/
├── Services/
│   ├── GenericService.cs
│   ├── CategoryService.cs
│   ├── ProductService.cs
│   ├── StorageService.cs
│   └── ProductImageStorageService.cs
├── wwwroot/
│   └── images/products/
├── appsettings.json
├── Program.cs
└── README.md
```
