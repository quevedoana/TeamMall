using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeaMall.Data;
using TeaMall.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace TeaMall.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly DataContext _context;

        public UsuariosController(DataContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.IdLocal)
                .ToListAsync();

            return View(usuarios);
        }

        // GET: Usuarios/Create
        public async Task<IActionResult> Create()
        {
            await CargarLocales();
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

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