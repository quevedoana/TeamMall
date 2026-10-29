
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TeaMall.Data;
using TeaMall.Models;

namespace TeaMall.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private readonly PasswordHasher<Usuario> _passwordHasher = new();

        public AuthController(
            DataContext context,
            IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginApiRequest login)
        {
            if (string.IsNullOrWhiteSpace(login.DNI) ||
                string.IsNullOrWhiteSpace(login.Contrasenia))
            {
                return BadRequest(new
                {
                    mensaje = "Ingresá tu DNI y contraseña."
                });
            }

            string dni = login.DNI.Trim();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.DNI == dni && u.Activo);

            if (usuario == null)
            {
                return Unauthorized(new
                {
                    mensaje = "DNI o contraseña incorrectos."
                });
            }

            var resultado = _passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.Contrasenia,
                login.Contrasenia
            );

            if (resultado == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    mensaje = "DNI o contraseña incorrectos."
                });
            }

            var secretKey = _config[
                "TokenAuthentication:SecretKey"];

            if (string.IsNullOrWhiteSpace(secretKey))
            {
                return StatusCode(500, new
                {
                    mensaje = "La clave de autenticación no está configurada."
                });
            }

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    usuario.IdUsuario.ToString()
                ),
                new Claim(
                    ClaimTypes.Name,
                    usuario.Nombre + " " + usuario.Apellido
                ),
                new Claim(
                    ClaimTypes.Role,
                    usuario.Rol
                ),
                new Claim("DNI", usuario.DNI),
                new Claim(
                    "FullName",
                    usuario.Nombre + " " + usuario.Apellido
                )
            };

            var key = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(secretKey)
            );

            var credenciales = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _config["TokenAuthentication:Issuer"],
                audience: _config["TokenAuthentication:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credenciales
            );

            var tokenGenerado =
                new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(tokenGenerado);
        }
    }

    public class LoginApiRequest
    {
        public string DNI { get; set; } = string.Empty;

        public string Contrasenia { get; set; } = string.Empty;
    }
}
