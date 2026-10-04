using System.ComponentModel.DataAnnotations;

namespace TeaMall.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        public string DNI { get; set; } = string.Empty;

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Rol { get; set; } = string.Empty;

        public string? Avatar { get; set; }

        public int? IdLocal { get; set; }

        public bool Activo { get; set; } = true;
    }
}