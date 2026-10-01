using System.ComponentModel.DataAnnotations;

namespace TeaMall.Models
{
    public class Notificacion
    {
        [Key]
        public int IdNotificacion { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public int IdPublicacion { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaLectura { get; set; }

        public bool Leida { get; set; }
    }
}