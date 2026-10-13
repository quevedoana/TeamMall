using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeaMall.Models
{
    public class Publicacion
    {
        [Key]
        public int IdPublicacion { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de publicación es obligatorio.")]
        public string Tipo { get; set; } = string.Empty;

        // Ruta del archivo guardada en la base de datos
        public string? Archivo { get; set; }

        // Archivo que llega desde el formulario
        [NotMapped]
        [Display(Name = "Archivo")]
        public IFormFile? ArchivoSubido { get; set; }

        // Locales seleccionados para la publicación
        [NotMapped]
        public List<int> LocalesSeleccionados { get; set; } = new List<int>();

        [Required(ErrorMessage = "La fecha de publicación es obligatoria.")]
        public DateTime FechaPublicacion { get; set; }

        [Required(ErrorMessage = "El usuario creador es obligatorio.")]
        public int IdUsuario { get; set; }
    }
}