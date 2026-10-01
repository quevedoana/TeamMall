using System.ComponentModel.DataAnnotations;

namespace TeaMall.Models
{
    public class Inasistencia
    {
        [Key]
        public int IdInasistencia { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un tipo.")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de ausencia es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaAusencia { get; set; }

        [Required]
        public DateTime FechaCarga { get; set; }

        public string? Archivo { get; set; }

        public string? Comentario { get; set; }

        [Required]
        public string Estado { get; set; } = "Pendiente";
    }
}