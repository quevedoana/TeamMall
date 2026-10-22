using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeaMall.Data;
using TeaMall.Models;
using Microsoft.AspNetCore.Authorization;

namespace TeaMall.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class LocalesController : Controller
    {
        private readonly DataContext _context;

        public LocalesController(DataContext context)
        {
            _context = context;
        }

        // GET: Locales
        public async Task<IActionResult> Index()
        {
            var locales = await _context.Locales.ToListAsync();

            return View(locales);
        }

        // GET: Locales/Listar
[HttpGet]
public async Task<IActionResult> Listar()
{
    var locales = await _context.Locales
        .AsNoTracking()
        .OrderBy(l => l.Nombre)
        .Select(l => new
        {
            idLocal = l.IdLocal,
            nombre = l.Nombre,
            activo = l.Activo
        })
        .ToListAsync();

    return Json(locales);
}

        // GET: Locales/Create
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([FromBody] Local local)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(new
        {
            mensaje = "Revisá los datos ingresados."
        });
    }

    _context.Locales.Add(local);
    await _context.SaveChangesAsync();

    return Json(new
    {
        mensaje = "Local creado correctamente.",
        idLocal = local.IdLocal,
        nombre = local.Nombre,
        activo = local.Activo
    });
}

        // GET: Locales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var local = await _context.Locales.FindAsync(id);

            if (local == null)
            {
                return NotFound();
            }

            return View(local);
        }

        // POST: Locales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Local local)
        {
            if (id != local.IdLocal)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(local);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(local);
        }

        // POST: Locales/Desactivar/5
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Desactivar(int id)
{
    var local = await _context.Locales.FindAsync(id);

    if (local == null)
    {
        return NotFound(new { mensaje = "No se encontró el local." });
    }

    local.Activo = false;
    await _context.SaveChangesAsync();

    return Json(new
    {
        mensaje = "Local desactivado correctamente.",
        idLocal = local.IdLocal
    });
}

    }
}