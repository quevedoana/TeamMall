
using System.ComponentModel.DataAnnotations;

namespace TeaMall.DTOs
{
    // Datos que la API devuelve al consultar un local
    public class LocalDto
    {
        public int IdLocal { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public bool Activo { get; set; }
    }

    // Datos que la API recibe al crear un local
    public class CrearLocalDto
    {
        [Required(ErrorMessage = "El nombre del local es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;
    }

    // Datos que la API recibe al modificar un local
    public class ActualizarLocalDto
    {
        [Required(ErrorMessage = "El nombre del local es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        public bool Activo { get; set; }
    }
}
