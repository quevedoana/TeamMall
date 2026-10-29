
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TeaMall.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PerfilController : ControllerBase
    {
        // GET: api/perfil
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                idUsuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                nombre = User.Identity?.Name,
                dni = User.FindFirst("DNI")?.Value,
                rol = User.FindFirst(ClaimTypes.Role)?.Value
            });
        }
    }
}
