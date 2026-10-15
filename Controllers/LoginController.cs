using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TeaMall.Data;
using TeaMall.Models;

namespace TeaMall.Controllers
{
    public class LoginController : Controller
    {
        private readonly DataContext _context;
        private readonly PasswordHasher<Usuario> _passwordHasher = new();

        public LoginController(DataContext context)
        {
            _context = context;
        }

        // GET: Login
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string dni, string contrasenia)
        {
            if (string.IsNullOrWhiteSpace(dni) ||
                string.IsNullOrWhiteSpace(contrasenia))
            {
                ViewBag.Error = "Ingresá tu DNI y contraseña.";
                return View();
            }

            dni = dni.Trim();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.DNI == dni && u.Activo);

            if (usuario == null)
            {
                ViewBag.Error = "DNI o contraseña incorrectos.";
                return View();
            }

            var resultado = _passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.Contrasenia,
                contrasenia
            );

            if (resultado == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "DNI o contraseña incorrectos.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre + " " + usuario.Apellido),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim("DNI", usuario.DNI)
            };

            var identidad = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identidad);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            return RedirectToAction("Index", "Home");
        }

        // GET: Login/AccesoDenegado
        [HttpGet]
        public IActionResult AccesoDenegado()
        {
            return View();
        }

        // Cerrar sesión
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction(nameof(Index));
        }
    }
}