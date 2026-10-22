
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TeaMall.Data;
using TeaMall.Models;

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
        [HttpGet]
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
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Locales/Create
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
        [HttpGet]
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
public async Task<IActionResult> Edit(int id, [FromBody] Local local)
{
    if (id != local.IdLocal)
    {
        return BadRequest(new
        {
            mensaje = "El identificador del local no coincide."
        });
    }

    if (!ModelState.IsValid)
    {
        return BadRequest(new
        {
            mensaje = "Revisá los datos ingresados."
        });
    }

    var localExistente = await _context.Locales.FindAsync(id);

    if (localExistente == null)
    {
        return NotFound(new
        {
            mensaje = "No se encontró el local."
        });
    }

    localExistente.Nombre = local.Nombre;
    localExistente.Activo = local.Activo;

    await _context.SaveChangesAsync();

    return Json(new
    {
        mensaje = "Local actualizado correctamente.",
        idLocal = localExistente.IdLocal,
        nombre = localExistente.Nombre,
        activo = localExistente.Activo
    });
}


        // POST: Locales/Desactivar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            var local = await _context.Locales.FindAsync(id);

            if (local == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el local."
                });
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
