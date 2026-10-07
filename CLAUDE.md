# CLAUDE.md

App ASP.NET Core MVC en .NET 10 (`app-curso-claude.csproj`) con pruebas xUnit en `tests/app-curso-claude.Tests`; ambos están en `app-curso-claude.slnx`. Namespaces: `app_curso_claude.<Carpeta>`, block-scoped.

## Comandos

```bash
dotnet build
dotnet test
dotnet run                               # http://localhost:5007
dotnet run --urls http://localhost:5008  # en un worktree: 5008, 5009, 5010... para no chocar con otra sesión
```

## Estructura

Hoy existen `Models/`, `Services/`, `Controllers/`, `Views/` y `tests/app-curso-claude.Tests/`. Las interfaces y los repositorios en memoria están por ahora en `Services/`.

Estructura objetivo (lo marcado como *nuevo* todavía no existe; crearlo cuando haga falta):
- `Models/`: entidades y modelos de vista
- `Data/` *nuevo*: `AppDbContext` y `Data/Migrations/`
- `Data/Repositories/` *nuevo*: interfaces de repositorio
- `Data/Repositories/InMemory/` y `Data/Repositories/EfCore/` *nuevo*: implementaciones
- `Services/`: reglas de negocio
- `Controllers/` y `Views/`: MVC
- `tests/app-curso-claude.Tests/`: pruebas

## Reglas

- Los servicios se registran en `Program.cs` debajo de `// === Application services (register here) ===`.
- Los controladores y las vistas no tienen reglas de negocio: llaman a un servicio.
- Estándar de datos: el esquema solo cambia con migraciones de EF Core (`Data/Migrations`); tablas y columnas en inglés y PascalCase; toda columna de texto con `HasMaxLength`; decimales con `HasPrecision(18, 2)`; nada de SQL armado por concatenación o interpolación (`FromSqlRaw`/`ExecuteSqlRaw` con datos del usuario).
- La baja de productos es lógica (`IsActive = false`), nunca `Remove` ni `DELETE`.
- Ninguna credencial en archivos versionados.
- Nunca escribas correos, teléfonos ni direcciones de clientes en los logs.
- Todo comportamiento nuevo lleva su prueba; no se termina una tarea sin `dotnet test` en verde.
- No hagas `git push` ni merge salvo que te lo pidan.
