using System.Text.RegularExpressions;

namespace app_curso_claude.Tests
{
    public class EstandarSpTests
    {
        private static readonly string CarpetaProcedures = Path.Combine(BuscarRaizDelRepo(), "db", "procedures");

        public static IEnumerable<object[]> ArchivosSql()
        {
            return ObtenerArchivosSql().Select(ruta => new object[] { Path.GetFileName(ruta) });
        }

        [Fact]
        public void Procedures_ExisteAlMenosUnArchivoSql()
        {
            Assert.True(Directory.Exists(CarpetaProcedures), $"No existe la carpeta {CarpetaProcedures}");
            Assert.NotEmpty(ObtenerArchivosSql());
        }

        [Theory]
        [MemberData(nameof(ArchivosSql))]
        public void Archivo_TieneNombreUspEntidadAccion(string archivo)
        {
            var nombre = Path.GetFileNameWithoutExtension(archivo);

            Assert.Matches(@"^usp_[A-Z][A-Za-z0-9]*_[A-Z][A-Za-z0-9]*$", nombre);
        }

        [Theory]
        [MemberData(nameof(ArchivosSql))]
        public void Procedimiento_UsaCreateOrAlterConEsquemaDboYElNombreDelArchivo(string archivo)
        {
            var codigo = LeerCodigo(archivo);
            var nombre = Path.GetFileNameWithoutExtension(archivo);

            Assert.Matches(new Regex($@"\bCREATE\s+OR\s+ALTER\s+PROCEDURE\s+dbo\.{Regex.Escape(nombre)}\b", RegexOptions.IgnoreCase), codigo);
        }

        [Theory]
        [MemberData(nameof(ArchivosSql))]
        public void Tablas_SeReferencianConEsquemaDbo(string archivo)
        {
            var codigo = LeerCodigo(archivo);

            var sinEsquema = Regex.Matches(codigo, @"\b(?:FROM|JOIN|INTO|UPDATE)\s+(?!dbo\.)([\[\]\w.]+)", RegexOptions.IgnoreCase)
                .Select(m => m.Groups[1].Value)
                .ToList();

            Assert.True(sinEsquema.Count == 0, $"Tablas sin esquema dbo.: {string.Join(", ", sinEsquema)}");
        }

        [Theory]
        [MemberData(nameof(ArchivosSql))]
        public void Cuerpo_EmpiezaConSetNocountOn(string archivo)
        {
            var codigo = LeerCodigo(archivo);

            Assert.Matches(new Regex(@"\bAS\s+(?:BEGIN\s+)?SET\s+NOCOUNT\s+ON;", RegexOptions.IgnoreCase), codigo);
        }

        [Theory]
        [MemberData(nameof(ArchivosSql))]
        public void Codigo_NoUsaSelectAsterisco(string archivo)
        {
            var codigo = LeerCodigo(archivo);

            Assert.DoesNotMatch(new Regex(@"\bSELECT\s+(?:TOP\s*\(?\s*\d+\s*\)?\s+)?\*", RegexOptions.IgnoreCase), codigo);
        }

        [Theory]
        [MemberData(nameof(ArchivosSql))]
        public void Codigo_NoUsaRaiserrorNiPrint(string archivo)
        {
            var codigo = LeerCodigo(archivo);

            Assert.DoesNotMatch(new Regex(@"\b(?:RAISERROR|PRINT)\b", RegexOptions.IgnoreCase), codigo);
        }

        [Theory]
        [MemberData(nameof(ArchivosSql))]
        public void ProcedimientoQueEscribe_UsaTryCatchConTransaccionYThrow(string archivo)
        {
            var codigo = LeerCodigo(archivo);
            if (!Regex.IsMatch(codigo, @"\b(?:INSERT|UPDATE|DELETE|MERGE)\b", RegexOptions.IgnoreCase))
            {
                return;
            }

            var opciones = RegexOptions.IgnoreCase | RegexOptions.Singleline;
            Assert.Matches(new Regex(@"\bBEGIN\s+TRY\b.*\bBEGIN\s+TRAN(?:SACTION)?\b.*\bCOMMIT\b.*\bEND\s+TRY\b", opciones), codigo);
            Assert.Matches(new Regex(@"\bBEGIN\s+CATCH\b.*@@TRANCOUNT\s*>\s*0.*\bROLLBACK\b.*\bTHROW\s*;.*\bEND\s+CATCH\b", opciones), codigo);
        }

        [Theory]
        [MemberData(nameof(ArchivosSql))]
        public void Archivo_TerminaConGo(string archivo)
        {
            var lineas = File.ReadAllLines(Path.Combine(CarpetaProcedures, archivo))
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();

            Assert.NotEmpty(lineas);
            Assert.Equal("GO", lineas[^1].Trim(), ignoreCase: true);
        }

        private static string[] ObtenerArchivosSql()
        {
            return Directory.Exists(CarpetaProcedures)
                ? Directory.GetFiles(CarpetaProcedures, "*.sql").OrderBy(r => r, StringComparer.Ordinal).ToArray()
                : Array.Empty<string>();
        }

        // Devuelve el código sin comentarios, para que la cabecera no genere falsos positivos.
        private static string LeerCodigo(string archivo)
        {
            var texto = File.ReadAllText(Path.Combine(CarpetaProcedures, archivo));
            texto = Regex.Replace(texto, @"/\*.*?\*/", " ", RegexOptions.Singleline);
            return Regex.Replace(texto, @"--[^\r\n]*", " ");
        }

        private static string BuscarRaizDelRepo()
        {
            var directorio = new DirectoryInfo(AppContext.BaseDirectory);
            while (directorio != null && !File.Exists(Path.Combine(directorio.FullName, "app-curso-claude.slnx")))
            {
                directorio = directorio.Parent;
            }

            return directorio?.FullName
                ?? throw new InvalidOperationException("No se encontró app-curso-claude.slnx subiendo desde " + AppContext.BaseDirectory);
        }
    }
}
