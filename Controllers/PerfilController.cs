
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using TeaMall.Data;
using TeaMall.Models;

namespace TeaMall.Controllers
{
    [Authorize]
    public class PerfilController : Controller
    {
        private readonly DataContext _context;

        public PerfilController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var idUsuario = ObtenerIdUsuario();

            if (idUsuario == null)
                return Challenge();

            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario.Value);

            if (usuario == null || !usuario.Activo)
                return Challenge();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            string nombre,
            string apellido,
            string email,
            IFormFile? avatar)
        {
            var idUsuario = ObtenerIdUsuario();

            if (idUsuario == null)
                return Challenge();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario.Value);

            if (usuario == null || !usuario.Activo)
                return Challenge();

            nombre = (nombre ?? "").Trim();
            apellido = (apellido ?? "").Trim();
            email = (email ?? "").Trim();

            if (string.IsNullOrWhiteSpace(nombre) || nombre.Length > 50)
            {
                ModelState.AddModelError(
                    "nombre",
                    "Ingresá un nombre de hasta 50 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(apellido) || apellido.Length > 50)
            {
                ModelState.AddModelError(
                    "apellido",
                    "Ingresá un apellido de hasta 50 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(email) ||
                email.Length > 100 ||
                !new EmailAddressAttribute().IsValid(email))
            {
                ModelState.AddModelError(
                    "email",
                    "Ingresá un email válido de hasta 100 caracteres.");
            }

            var emailEnUso = await _context.Usuarios.AnyAsync(u =>
                u.Email == email &&
                u.IdUsuario != usuario.IdUsuario);

            if (emailEnUso)
            {
                ModelState.AddModelError(
                    "email",
                    "Ese email ya está registrado.");
            }

            string? nuevaRutaAvatar = null;

            if (avatar != null && avatar.Length > 0)
            {
                const long limiteAvatar = 2 * 1024 * 1024;

                var extensionesPermitidas =
                    new[] { ".jpg", ".jpeg", ".png", ".webp" };

                var extension = Path.GetExtension(avatar.FileName)
                    .ToLowerInvariant();

                if (avatar.Length > limiteAvatar)
                {
                    ModelState.AddModelError(
                        "avatar",
                        "El avatar no puede superar los 2 MB.");
                }
                else if (!extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError(
                        "avatar",
                        "El avatar debe ser JPG, PNG o WEBP.");
                }
                else
                {
                    var carpeta = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "uploads",
                        "avatares");

                    Directory.CreateDirectory(carpeta);

                    var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                    var rutaFisica = Path.Combine(carpeta, nombreArchivo);

                    await using (var stream = new FileStream(
                        rutaFisica, FileMode.Create))
                    {
                        await avatar.CopyToAsync(stream);
                    }

                    nuevaRutaAvatar = $"/uploads/avatares/{nombreArchivo}";
                }
            }

            if (!ModelState.IsValid)
            {
                EliminarAvatar(nuevaRutaAvatar);

                usuario.Nombre = nombre;
                usuario.Apellido = apellido;
                usuario.Email = email;

                return View(usuario);
            }

            var avatarAnterior = usuario.Avatar;

            usuario.Nombre = nombre;
            usuario.Apellido = apellido;
            usuario.Email = email;

            if (nuevaRutaAvatar != null)
                usuario.Avatar = nuevaRutaAvatar;

            await _context.SaveChangesAsync();

            if (nuevaRutaAvatar != null &&
                !string.IsNullOrWhiteSpace(avatarAnterior))
            {
                EliminarAvatar(avatarAnterior);
            }

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    usuario.IdUsuario.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    usuario.Nombre + " " + usuario.Apellido),

                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim("DNI", usuario.DNI)
            };

            var identidad = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identidad));

            TempData["Mensaje"] =
                "Tu perfil se actualizó correctamente.";

            return RedirectToAction(nameof(Index));
        }

        private int? ObtenerIdUsuario()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(id, out var idUsuario)
                ? idUsuario
                : null;
        }

        private void EliminarAvatar(string? rutaAvatar)
        {
            if (string.IsNullOrWhiteSpace(rutaAvatar))
                return;

            var rutaRelativa = rutaAvatar
                .TrimStart('/')
                .Replace('/', Path.DirectorySeparatorChar);

            var rutaFisica = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                rutaRelativa);

            if (System.IO.File.Exists(rutaFisica))
                System.IO.File.Delete(rutaFisica);
        }
    }
}
