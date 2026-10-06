# Estándar BCP para stored procedures (versión de laboratorio)

> Estándar de referencia armado para el Lab 09 del curso. No es el documento oficial del banco.

Todo stored procedure en `db/procedures/` debe cumplir:

1. **Nombre:** `dbo.usp_<Entidad>_<Accion>` en PascalCase (por ejemplo `dbo.usp_Cuenta_Alta`). El archivo se llama igual que el procedimiento, sin el esquema: `usp_Cuenta_Alta.sql`. Prohibido el prefijo `sp_`, que SQL Server reserva para procedimientos de sistema.
2. **Creación idempotente:** `CREATE OR ALTER PROCEDURE`.
3. **Esquema explícito:** el procedimiento y todas las tablas se referencian con esquema (`dbo.Cuenta`, nunca `Cuenta`).
4. **Cabecera:** bloque de comentario al inicio con `Descripción`, `Parámetros` y `Retorno`.
5. **Parámetros:** prefijo `@p_` en PascalCase (`@p_Titular`), con tipos explícitos y longitudes acotadas.
6. **`SET NOCOUNT ON;`** como primera instrucción del cuerpo.
7. **Columnas explícitas:** prohibido `SELECT *`; los `INSERT` listan sus columnas.
8. **Manejo de errores:** las operaciones que escriben datos van en `BEGIN TRY ... BEGIN TRANSACTION ... COMMIT` con `BEGIN CATCH` que hace `ROLLBACK` si `@@TRANCOUNT > 0` y relanza con `THROW;`.
9. **Validaciones de negocio:** se informan con `THROW 50000-50999, '<mensaje>', 1;`, nunca con `RAISERROR` ni `PRINT`.
10. **Terminación:** cada instrucción termina en `;` y el archivo termina con `GO`.
