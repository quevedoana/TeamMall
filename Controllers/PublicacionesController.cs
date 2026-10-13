using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeaMall.Data;
using TeaMall.Models;

namespace TeaMall.Controllers
{
    public class PublicacionesController : Controller
    {
        private readonly DataContext _context;

        public PublicacionesController(DataContext context)
        {
            _context = context;
        }

        // GET: Publicaciones
        public async Task<IActionResult> Index()
        {
            var publicaciones = await _context.Publicaciones
                .OrderByDescending(p => p.FechaPublicacion)
                .ToListAsync();

            return View(publicaciones);
        }

        // GET: Publicaciones/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Locales = await _context.Locales
                .Where(l => l.Activo)
                .OrderBy(l => l.Nombre)
                .ToListAsync();

            return View();
        }

        // POST: Publicaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Publicacion publicacion)
        {
            if (!ModelState.IsValid)
            {
                return View(publicacion);
            }

            publicacion.FechaPublicacion = DateTime.Now;

            publicacion.IdUsuario = 1;

            // Por ahora usamos una carpeta general para los archivos de publicaciones
            string carpeta = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "Uploads",
                "Publicaciones"
            );

            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }

            // Si el administrador seleccionó un archivo
            if (publicacion.ArchivoSubido != null)
            {
                string extension = Path.GetExtension(publicacion.ArchivoSubido.FileName);
                string nombreArchivo = Guid.NewGuid().ToString() + extension;

                string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await publicacion.ArchivoSubido.CopyToAsync(stream);
                }

                publicacion.Archivo = "/Uploads/Publicaciones/" + nombreArchivo;
            }

            _context.Publicaciones.Add(publicacion);
            await _context.SaveChangesAsync();

            foreach (var idLocal in publicacion.LocalesSeleccionados)
            {
                _context.PublicacionesLocales.Add(new PublicacionLocal
                {
                    IdPublicacion = publicacion.IdPublicacion,
                    IdLocal = idLocal
                });
            }

            if (publicacion.LocalesSeleccionados.Any())
            {
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Publicaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publicacion = await _context.Publicaciones.FindAsync(id);

            if (publicacion == null)
            {
                return NotFound();
            }

            return View(publicacion);
        }

        // POST: Publicaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Publicacion publicacion)
        {
            if (id != publicacion.IdPublicacion)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(publicacion);
            }

            try
            {
                _context.Update(publicacion);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublicacionExists(publicacion.IdPublicacion))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Publicaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publicacion = await _context.Publicaciones
                .FirstOrDefaultAsync(p => p.IdPublicacion == id);

            if (publicacion == null)
            {
                return NotFound();
            }

            return View(publicacion);
        }

        // POST: Publicaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publicacion = await _context.Publicaciones.FindAsync(id);

            if (publicacion != null)
            {
                _context.Publicaciones.Remove(publicacion);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PublicacionExists(int id)
        {
            return _context.Publicaciones.Any(e => e.IdPublicacion == id);
        }
    }
}