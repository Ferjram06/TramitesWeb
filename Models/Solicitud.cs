
using System.ComponentModel.DataAnnotations;

namespace TramitesWeb.Models;

public class Solicitud
{
    // Identificador interno de la base de datos.
    public int Id { get; set; }

    // Folio visible para el usuario: TR-00001, por ejemplo.
    [Required]
    [StringLength(8)]
    public string Folio { get; set; } = string.Empty;

    // Datos personales
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(120)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    [DataType(DataType.Date)]
    public DateTime FechaNacimiento { get; set; }

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [StringLength(150)]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(20)]
    public string Telefono { get; set; } = string.Empty;

    // Datos económicos
    [Required(ErrorMessage = "La ocupación es obligatoria.")]
    [StringLength(50)]
    public string Ocupacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El ingreso mensual es obligatorio.")]
    [Range(0, 100000000,
        ErrorMessage = "El ingreso debe ser igual o mayor que cero.")]
    public decimal IngresosMensuales { get; set; }

    // Datos geográficos
    [Required(ErrorMessage = "El estado es obligatorio.")]
    [StringLength(100)]
    public string Estado { get; set; } = string.Empty;

    [Required(ErrorMessage = "El municipio o alcaldía es obligatorio.")]
    [StringLength(100)]
    public string Municipio { get; set; } = string.Empty;

    // Documento PDF
    [Required]
    [StringLength(255)]
    public string NombreArchivoPdf { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string RutaArchivoPdf { get; set; } = string.Empty;

    // Documento TXT
    [Required]
    [StringLength(255)]
    public string NombreArchivoTxt { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string RutaArchivoTxt { get; set; } = string.Empty;

    // Información del trámite
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(30)]
    public string EstadoSolicitud { get; set; } = "Registrada";
}
