
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TramitesWeb.Data;
using TramitesWeb.Models;
using TramitesWeb.ViewModels;

namespace TramitesWeb.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public HomeController(
        AppDbContext context,
        IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Historial()
    {
        return View();
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(
        CrearSolicitudViewModel modelo)
    {
        // Validaciones adicionales en el servidor.
        if (!EsCorreoValido(modelo.Correo))
        {
            ModelState.AddModelError(
                nameof(modelo.Correo),
                "Introduce un correo válido.");
        }

        if (!EsTelefonoValido(modelo.Telefono))
        {
            ModelState.AddModelError(
                nameof(modelo.Telefono),
                "El teléfono debe contener 10 dígitos mexicanos.");
        }

        if (modelo.FechaNacimiento.Date > DateTime.Today)
        {
            ModelState.AddModelError(
                nameof(modelo.FechaNacimiento),
                "La fecha de nacimiento no puede ser futura.");
        }

        const long maxSize = 10 * 1024 * 1024;

        if (!ArchivoValido(modelo.ArchivoPdf, ".pdf", maxSize))
        {
            ModelState.AddModelError(
                nameof(modelo.ArchivoPdf),
                "Selecciona un PDF válido de hasta 10 MB.");
        }

        if (!ArchivoValido(modelo.ArchivoTxt, ".txt", maxSize))
        {
            ModelState.AddModelError(
                nameof(modelo.ArchivoTxt),
                "Selecciona un TXT válido de hasta 10 MB.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                mensaje = "Revisa los datos del formulario.",
                errores = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors
                            .Select(e => e.ErrorMessage)
                            .ToArray())
            });
        }

        var carpeta = Path.Combine(
            _environment.ContentRootPath,
            "App_Data",
            "Uploads");

        Directory.CreateDirectory(carpeta);

        var nombrePdf = $"{Guid.NewGuid():N}.pdf";
        var nombreTxt = $"{Guid.NewGuid():N}.txt";

        var rutaPdf = Path.Combine(carpeta, nombrePdf);
        var rutaTxt = Path.Combine(carpeta, nombreTxt);

        try
        {
            await using (var stream = System.IO.File.Create(rutaPdf))
            {
                await modelo.ArchivoPdf!.CopyToAsync(stream);
            }

            await using (var stream = System.IO.File.Create(rutaTxt))
            {
                await modelo.ArchivoTxt!.CopyToAsync(stream);
            }

            var solicitud = new Solicitud
            {
                Folio = Guid.NewGuid().ToString("N")[..8],
                Nombre = modelo.Nombre.Trim(),
                FechaNacimiento = modelo.FechaNacimiento,
                Correo = modelo.Correo.Trim(),
                Telefono = modelo.Telefono.Trim(),
                Ocupacion = modelo.Ocupacion,
                IngresosMensuales = modelo.IngresosMensuales,
                Estado = modelo.Estado,
                Municipio = modelo.Municipio.Trim(),
                NombreArchivoPdf = Path.GetFileName(
                    modelo.ArchivoPdf!.FileName),
                RutaArchivoPdf = Path.Combine("App_Data", "Uploads", nombrePdf),
                NombreArchivoTxt = Path.GetFileName(
                    modelo.ArchivoTxt!.FileName),
                RutaArchivoTxt = Path.Combine("App_Data", "Uploads", nombreTxt),
                FechaRegistro = DateTime.UtcNow,
                EstadoSolicitud = "Registrada"
            };

            _context.Solicitudes.Add(solicitud);
            await _context.SaveChangesAsync();

            // El Id de SQL Server permite construir un folio determinista.
            if (solicitud.Id > 99999)
            {
                _context.Solicitudes.Remove(solicitud);
                await _context.SaveChangesAsync();

                throw new InvalidOperationException(
                    "Se alcanzó el límite de folios de cinco dígitos.");
            }

            solicitud.Folio = $"TR-{solicitud.Id:D5}";
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Solicitud registrada correctamente.",
                folio = solicitud.Folio
            });
        }
        catch
        {
            if (System.IO.File.Exists(rutaPdf))
                System.IO.File.Delete(rutaPdf);

            if (System.IO.File.Exists(rutaTxt))
                System.IO.File.Delete(rutaTxt);

            throw;
        }
    }

    private static bool EsCorreoValido(string? correo)
    {
        if (string.IsNullOrWhiteSpace(correo))
            return false;

        var partes = correo.Trim().Split('@');

        if (partes.Length != 2)
            return false;

        var usuario = partes[0];
        var dominio = partes[1];
        var etiquetas = dominio.Split('.');

        return usuario.Length > 0
            && !usuario.Contains("..")
            && !usuario.StartsWith('.')
            && !usuario.EndsWith('.')
            && etiquetas.Length >= 2
            && etiquetas.All(e =>
                e.Length > 0
                && !e.StartsWith('-')
                && !e.EndsWith('-')
                && e.All(c => char.IsAsciiLetterOrDigit(c) || c == '-'))
            && etiquetas[^1].Length >= 2
            && etiquetas[^1].All(char.IsAsciiLetter)
            && etiquetas.Take(etiquetas.Length - 1)
                .Any(e => e.Any(char.IsAsciiLetter));
    }

    private static bool EsTelefonoValido(string? telefono)
    {
        if (string.IsNullOrWhiteSpace(telefono))
            return false;

        var digitos = new string(telefono
            .Where(char.IsDigit)
            .ToArray());

        if (telefono.Trim().StartsWith("+52"))
            digitos = digitos.Length == 12 && digitos.StartsWith("52")
                ? digitos[2..]
                : string.Empty;

        return digitos.Length == 10
            && digitos.All(char.IsAsciiDigit);
    }

    private static bool ArchivoValido(
        IFormFile? archivo,
        string extension,
        long maxSize)
    {
        return archivo is not null
            && archivo.Length > 0
            && archivo.Length <= maxSize
            && string.Equals(
                Path.GetExtension(archivo.FileName),
                extension,
                StringComparison.OrdinalIgnoreCase);
    }

}
