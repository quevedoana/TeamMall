using System.ComponentModel.DataAnnotations;

namespace TeaMall.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI debe tener 7 u 8 números.")]
        public string DNI { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede superar los 50 caracteres.")]
        [RegularExpression(@"^(?=.*\S).+$", ErrorMessage = "El nombre no puede contener solo espacios.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido no puede superar los 50 caracteres.")]
        [RegularExpression(@"^(?=.*\S).+$", ErrorMessage = "El apellido no puede contener solo espacios.")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingresá un email válido.")]
        [StringLength(100, ErrorMessage = "El email no puede superar los 100 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [StringLength(100, ErrorMessage = "La contraseña no puede superar los 100 caracteres.")]
        public string Contrasenia { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccioná un rol.")]
        public string Rol { get; set; } = string.Empty;

        public string? Avatar { get; set; }

        public int? IdLocal { get; set; }

        public bool Activo { get; set; } = true;
    }
}