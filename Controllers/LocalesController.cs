
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
public async Task<IActionResult> Listar(
    int pagina = 1,
    int tamaño = 5,
    string busqueda = "")
{
    // Validar los parámetros de paginación.
    pagina = Math.Max(1, pagina);
    tamaño = Math.Clamp(tamaño, 1, 50);

    // Consulta inicial sin cargar todos los registros.
    var consulta = _context.Locales
        .AsNoTracking()
        .AsQueryable();

    // Búsqueda por nombre del local.
    if (!string.IsNullOrWhiteSpace(busqueda))
    {
        busqueda = busqueda.Trim();
        consulta = consulta.Where(l =>
            l.Nombre.Contains(busqueda));
    }

    // Contar los resultados después del filtro.
    var total = await consulta.CountAsync();

    var totalPaginas = (int)Math.Ceiling(
        total / (double)tamaño);

    // Si no hay resultados, la página actual será 1.
    if (totalPaginas == 0)
    {
        pagina = 1;
    }
    else
    {
        pagina = Math.Min(pagina, totalPaginas);
    }

    // Obtener solamente los registros de la página solicitada.
    var locales = await consulta
        .OrderBy(l => l.Nombre)
        .Skip((pagina - 1) * tamaño)
        .Take(tamaño)
        .Select(l => new
        {
            idLocal = l.IdLocal,
            nombre = l.Nombre,
            activo = l.Activo
        })
        .ToListAsync();

    // Devolver el formato que espera Index.cshtml.
    return Json(new
    {
        locales,
        pagina,
        tamaño,
        total,
        totalPaginas
    });
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
