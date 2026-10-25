
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TramitesWeb.ViewModels;

public class CrearSolicitudViewModel
{
    
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(120, MinimumLength = 10,
        ErrorMessage = "El nombre debe tener entre 10 y 120 caracteres.")]
    [RegularExpression(
        @"^[\p{L}\p{M}]+(?:[ '\u2019-][\p{L}\p{M}]+)*$",
        ErrorMessage = "El nombre solo puede contener letras, espacios, guiones y apóstrofes.")]
    public string Nombre { get; set; } = string.Empty;


    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    [DataType(DataType.Date)]
    public DateTime FechaNacimiento { get; set; }

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [StringLength(150)]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [RegularExpression(@"^(?:\+52[\s-]?)?(?:\(?\d{2,3}\)?[\s-]?)?\d{7,8}$",
        ErrorMessage = "Introduce un teléfono válido.")]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "La ocupación es obligatoria.")]
    [StringLength(50)]
    public string Ocupacion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El ingreso mensual es obligatorio.")]
    [Range(0, 100000000)]
    public decimal IngresosMensuales { get; set; }

    [Required(ErrorMessage = "El estado es obligatorio.")]
    [StringLength(100)]
    public string Estado { get; set; } = string.Empty;

    [Required(ErrorMessage = "El municipio es obligatorio.")]
    [StringLength(100)]
    public string Municipio { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debes adjuntar un PDF.")]
    public IFormFile? ArchivoPdf { get; set; }

    [Required(ErrorMessage = "Debes adjuntar un TXT.")]
    public IFormFile? ArchivoTxt { get; set; }
}
