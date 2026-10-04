using System.ComponentModel.DataAnnotations;

namespace TeaMall.Models
{
    public class Local
    {
        [Key]
        public int IdLocal { get; set; }

        [Required(ErrorMessage = "El nombre del local es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
    }
}