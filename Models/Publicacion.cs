using System.ComponentModel.DataAnnotations;

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

        public string? Archivo { get; set; }

        [Required(ErrorMessage = "La fecha de publicación es obligatoria.")]
        public DateTime FechaPublicacion { get; set; }

        [Required(ErrorMessage = "El usuario creador es obligatorio.")]
        public int IdUsuario { get; set; }
    }
}