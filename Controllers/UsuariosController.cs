using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using TeaMall.Data;
using TeaMall.Models;

namespace TeaMall.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly DataContext _context;
        private readonly PasswordHasher<Usuario> _passwordHasher = new();

        public UsuariosController(DataContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            var locales = await _context.Locales.ToListAsync();

            ViewBag.Locales = locales;

            return View(usuarios);
        }

        // GET: Usuarios/Create
        public async Task<IActionResult> Create()
        {
            await CargarLocales();

            return View(new Usuario { Activo = true });
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                // Proteger la contraseña antes de guardarla
                usuario.Contrasenia = _passwordHasher.HashPassword(
                    usuario,
                    usuario.Contrasenia
                );

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Si hay errores, volver a cargar los locales
            await CargarLocales();

            return View(usuario);
        }

        private async Task CargarLocales()
        {
            ViewBag.Locales = new SelectList(
                await _context.Locales
                    .Where(l => l.Activo)
                    .ToListAsync(),
                "IdLocal",
                "Nombre"
            );
        }
    }
}