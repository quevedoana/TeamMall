using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
        public async Task<IActionResult> Index(int pagina = 1)
        {
            const int publicacionesPorPagina = 10;

            if (pagina < 1)
            {
                pagina = 1;
            }

            // Si es administrador, puede ver todas las publicaciones
            if (User.IsInRole("Administrador"))
            {
                var consulta = _context.Publicaciones
                    .AsQueryable();

                var totalPublicaciones = await consulta.CountAsync();

                var totalPaginas = (int)Math.Ceiling(
                    totalPublicaciones / (double)publicacionesPorPagina
                );

                if (totalPaginas > 0 && pagina > totalPaginas)
                {
                    pagina = totalPaginas;
                }

                var publicacionesAdmin = await consulta
                    .OrderByDescending(p => p.FechaPublicacion)
                    .Skip((pagina - 1) * publicacionesPorPagina)
                    .Take(publicacionesPorPagina)
                    .ToListAsync();

                ViewBag.PaginaActual = pagina;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.TotalPublicaciones = totalPublicaciones;

                return View(publicacionesAdmin);
            }

            // Obtenemos el ID del usuario logueado
            var idUsuarioClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (!int.TryParse(idUsuarioClaim, out int idUsuario))
            {
                return Unauthorized();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario);

            if (usuario == null || !usuario.IdLocal.HasValue)
            {
                ViewBag.PaginaActual = 1;
                ViewBag.TotalPaginas = 0;
                ViewBag.TotalPublicaciones = 0;

                return View(new List<Publicacion>());
            }

            int idLocal = usuario.IdLocal.Value;

            // El vendedor solamente ve publicaciones de su local
            var consultaVendedor = _context.Publicaciones
                .Where(p => _context.PublicacionesLocales
                    .Any(pl =>
                        pl.IdPublicacion == p.IdPublicacion &&
                        pl.IdLocal == idLocal
                    ));

            var totalPublicacionesVendedor =
                await consultaVendedor.CountAsync();

            var totalPaginasVendedor = (int)Math.Ceiling(
                totalPublicacionesVendedor /
                (double)publicacionesPorPagina
            );

            if (totalPaginasVendedor > 0 &&
                pagina > totalPaginasVendedor)
            {
                pagina = totalPaginasVendedor;
            }

            var publicaciones = await consultaVendedor
                .OrderByDescending(p => p.FechaPublicacion)
                .Skip((pagina - 1) * publicacionesPorPagina)
                .Take(publicacionesPorPagina)
                .ToListAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginasVendedor;
            ViewBag.TotalPublicaciones = totalPublicacionesVendedor;

            return View(publicaciones);
        }

        // GET: Publicaciones/Create
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create(Publicacion publicacion)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Locales = await _context.Locales
                    .Where(l => l.Activo)
                    .OrderBy(l => l.Nombre)
                    .ToListAsync();

                return View(publicacion);
            }

            var idUsuarioClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(idUsuarioClaim, out int idUsuario))
            {
                return Unauthorized();
            }

            publicacion.FechaPublicacion = DateTime.Now;
            publicacion.IdUsuario = idUsuario;

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
                var extensionesPermitidas = new[]
                {
                    ".pdf",
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".gif",
                    ".webp",
                    ".doc",
                    ".docx",
                    ".xls",
                    ".xlsx",
                    ".zip",
                    ".rar"
                };

                string extension = Path
                    .GetExtension(publicacion.ArchivoSubido.FileName)
                    .ToLowerInvariant();

                // Verificamos que la extensión esté permitida
                if (!extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError(
                        "ArchivoSubido",
                        "El tipo de archivo no está permitido. " +
                        "Podés subir PDF, imágenes, Word, Excel, ZIP o RAR."
                    );

                    ViewBag.Locales = await _context.Locales
                        .Where(l => l.Activo)
                        .OrderBy(l => l.Nombre)
                        .ToListAsync();

                    return View(publicacion);
                }

                string nombreArchivo = Guid.NewGuid().ToString() + extension;

                string rutaCompleta = Path.Combine(
                    carpeta,
                    nombreArchivo
                );

                using (var stream = new FileStream(
                    rutaCompleta,
                    FileMode.Create))
                {
                    await publicacion.ArchivoSubido.CopyToAsync(stream);
                }

                publicacion.Archivo =
                    "/Uploads/Publicaciones/" + nombreArchivo;
            }

            // Guardamos la publicación
            _context.Publicaciones.Add(publicacion);
            await _context.SaveChangesAsync();

            // Guardamos la relación entre publicación y locales
            foreach (var idLocal in publicacion.LocalesSeleccionados.Distinct())
            {
                _context.PublicacionesLocales.Add(new PublicacionLocal
                {
                    IdPublicacion = publicacion.IdPublicacion,
                    IdLocal = idLocal
                });
            }

            await _context.SaveChangesAsync();

            // Buscamos los vendedores de los locales asociados a la publicación
            var usuarios = await _context.PublicacionesLocales
                .Where(pl => pl.IdPublicacion == publicacion.IdPublicacion)
                .Join(
                    _context.Usuarios,
                    pl => pl.IdLocal,
                    u => u.IdLocal,
                    (pl, u) => u
                )
                .Where(u =>
                    u.Activo &&
                    u.Rol == "Vendedor"
                )
                .Select(u => u.IdUsuario)
                .Distinct()
                .ToListAsync();

            // Creamos una notificación para cada vendedor
            foreach (var idVendedor in usuarios)
            {
                _context.Notificaciones.Add(new Notificacion
                {
                    IdUsuario = idVendedor,
                    IdPublicacion = publicacion.IdPublicacion,
                    FechaCreacion = DateTime.Now,
                    FechaLectura = null,
                    Leida = false
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Publicaciones/Edit/5
        [Authorize(Roles = "Administrador")]
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
        // POST: Publicaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
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

            var publicacionExistente = await _context.Publicaciones
                .FirstOrDefaultAsync(p => p.IdPublicacion == id);

            if (publicacionExistente == null)
            {
                return NotFound();
            }

            // Actualizamos los datos de la publicación
            publicacionExistente.Titulo = publicacion.Titulo;
            publicacionExistente.Descripcion = publicacion.Descripcion;
            publicacionExistente.Tipo = publicacion.Tipo;

            // Si se seleccionó un archivo nuevo
            if (publicacion.ArchivoSubido != null)
            {
                var extensionesPermitidas = new[]
                {
                    ".pdf",
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".gif",
                    ".webp",
                    ".doc",
                    ".docx",
                    ".xls",
                    ".xlsx",
                    ".zip",
                    ".rar"
                };

                string extension = System.IO.Path
                    .GetExtension(publicacion.ArchivoSubido.FileName)
                    .ToLowerInvariant();

                // Verificamos que la extensión esté permitida
                if (!extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError(
                        "ArchivoSubido",
                        "El tipo de archivo no está permitido. " +
                        "Podés subir PDF, imágenes, Word, Excel, ZIP o RAR."
                    );

                    return View(publicacion);
                }

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

                string nombreArchivo =
                    Guid.NewGuid().ToString() + extension;

                string rutaCompleta = Path.Combine(
                    carpeta,
                    nombreArchivo
                );

                using (var stream = new FileStream(
                    rutaCompleta,
                    FileMode.Create))
                {
                    await publicacion.ArchivoSubido.CopyToAsync(stream);
                }

                publicacionExistente.Archivo =
                    "/Uploads/Publicaciones/" + nombreArchivo;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublicacionExists(publicacionExistente.IdPublicacion))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Publicaciones/Delete/5
        [Authorize(Roles = "Administrador")]
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
        [Authorize(Roles = "Administrador")]
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

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Accesos(int? id)
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

            var accesos = await _context.Notificaciones
                .Where(n => n.IdPublicacion == id)
                .Join(
                    _context.Usuarios,
                    n => n.IdUsuario,
                    u => u.IdUsuario,
                    (n, u) => new
                    {
                        Usuario = u.Nombre + " " + u.Apellido,
                        Leida = n.Leida,
                        FechaLectura = n.FechaLectura,
                        FechaAccesoPublicacion = n.FechaAccesoPublicacion
                    }
                )
                .ToListAsync();

            ViewBag.Publicacion = publicacion;

            return View(accesos);
        }
    }
}