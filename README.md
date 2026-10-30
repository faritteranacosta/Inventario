# Inventario — Backend

API REST para un sistema de inventario SaaS (catálogo de productos, categorías, ventas, ingresos y gráficas), construida con **.NET 10** siguiendo **Clean Architecture + Arquitectura Hexagonal**.

## Stack

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core (InMemory en desarrollo)
- JWT Bearer Authentication
- OpenAPI (documento en `/openapi/v1.json` en desarrollo)

## Estructura

```
src/
  Unimag.Inventario.Domain/          # Núcleo: entidades, contratos, excepciones
  Unimag.Inventario.Application/     # Casos de uso (Commands/Queries + UseCases)
  Unimag.Inventario.Infrastructure/  # Adaptadores (EF Core, repositorios, JWT, storage)
  Unimag.Inventario.Api/             # Controladores, middleware, composición (Program.cs)
tests/
  Unimag.Inventario.ArchitectureTests/  # Reglas de dependencia entre capas
  Unimag.Inventario.UnitTests/          # Tests unitarios de casos de uso
```

Dominios: `Samples` (referencia), `Categories`, `Products`, `Sales`, `Tenants`, `Users` (+ Auth).

## Requisitos

- .NET SDK 10.0+
- (Opcional) EF Core tools para migraciones

## Ejecutar

```bash
dotnet restore
dotnet run --project src/Unimag.Inventario.Api
```

Por defecto la API queda en `https://localhost:7xxx` / `http://localhost:5xxx`. Base de datos InMemory (sin configuración).

## Documentación OpenAPI

En desarrollo, el documento OpenAPI está disponible en:

```
http://localhost:<puerto>/openapi/v1.json
```

Puedes importarlo en Swagger UI, Postman o cualquier cliente HTTP.

## Autenticación

1. Crear un tenant: `POST /api/tenants` `{ "name": "Mi Negocio" }`
2. Registrar usuario: `POST /api/auth/register` `{ "tenantId": "...", "email": "a@b.com", "password": "...", "role": "Admin" }`
3. Login: `POST /api/auth/login` `{ "email": "a@b.com", "password": "..." }` → devuelve `{ "token": "...", "user": {...} }`
4. Usar el token en el header: `Authorization: Bearer <token>`

## Endpoints principales

| Método | Ruta | Descripción |
|---|---|---|
| POST | /api/tenants | Crear tenant |
| GET  | /api/tenants | Listar tenants |
| POST | /api/auth/register | Registrar usuario |
| POST | /api/auth/login | Login (JWT) |
| GET  | /api/categories | Listar categorías |
| POST | /api/categories | Crear categoría |
| GET  | /api/products | Listar productos (?categoryId, ?search) |
| POST | /api/products | Crear producto |
| POST | /api/products/{id}/image | Establecer URL de imagen |
| POST | /api/products/{id}/image/upload | Subir imagen (multipart) |
| POST | /api/sales | Registrar venta |
| GET  | /api/sales | Listar ventas (?from=&to=) |
| GET  | /api/sales/summary | Ingresos por día/mes (?from=&to=&groupBy=day\|month) |

## Tests

```bash
dotnet test
```

## Versionamiento y CI/CD

- Rama `main`: producción.
- Ramas `feat/*`, `fix/*`: nuevas funciones/correcciones.
- Commits siguiendo convención Conventional Commits (`feat:`, `fix:`, `test:`, `ci:`, `chore:`).
- CI en GitHub Actions: build + tests en cada push/PR.

## Licencia

Privado.
