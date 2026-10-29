using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TeaMall.Data;

namespace TeaMall.Controllers
{
    public class NotificacionesController : Controller
    {
        private readonly DataContext _context;

        public NotificacionesController(DataContext context)
        {
            _context = context;
        }

        // GET: Notificaciones
        public async Task<IActionResult> Index(int pagina = 1)
        {
            var idUsuarioClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (!int.TryParse(idUsuarioClaim, out int idUsuario))
            {
                return Unauthorized();
            }

            var notificaciones = await _context.Notificaciones
                .Where(n => n.IdUsuario == idUsuario)
                .OrderByDescending(n => n.FechaCreacion)
                .ToListAsync();

            var notificacionesNoLeidas = notificaciones
                .Where(n => !n.Leida)
                .ToList();

            if (notificacionesNoLeidas.Any())
            {
                foreach (var notificacion in notificacionesNoLeidas)
                {
                    notificacion.Leida = true;
                    notificacion.FechaLectura = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }

            var cantidadPorPagina = 10;

            var totalNotificaciones = notificaciones.Count;

            var totalPaginas = (int)Math.Ceiling(
                totalNotificaciones / (double)cantidadPorPagina
            );

            if (totalPaginas == 0)
            {
                totalPaginas = 1;
            }

            if (pagina < 1)
            {
                pagina = 1;
            }

            if (pagina > totalPaginas)
            {
                pagina = totalPaginas;
            }

            var resultado = await _context.Notificaciones
                .Where(n => n.IdUsuario == idUsuario)
                .Join(
                    _context.Publicaciones,
                    n => n.IdPublicacion,
                    p => p.IdPublicacion,
                    (n, p) => new
                    {
                        Notificacion = n,
                        Publicacion = p
                    }
                )
                .OrderByDescending(x => x.Notificacion.FechaCreacion)
                .Skip((pagina - 1) * cantidadPorPagina)
                .Take(cantidadPorPagina)
                .ToListAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;

            return View(resultado);
        }

        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            var idUsuarioClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (!int.TryParse(idUsuarioClaim, out int idUsuario))
            {
                return Unauthorized();
            }

            var notificaciones = await _context.Notificaciones
                .Where(n =>
                    n.IdUsuario == idUsuario &&
                    n.FechaAccesoPublicacion == null
                )
                .Join(
                    _context.Publicaciones,
                    n => n.IdPublicacion,
                    p => p.IdPublicacion,
                    (n, p) => new
                    {
                        idNotificacion = n.IdNotificacion,
                        idPublicacion = p.IdPublicacion,
                        titulo = p.Titulo,
                        descripcion = p.Descripcion,
                        fechaCreacion = n.FechaCreacion,
                        leida = n.Leida
                    }
                )
                .OrderByDescending(n => n.fechaCreacion)
                .ToListAsync();

            return Json(notificaciones);
        }
        [HttpPost]
        public async Task<IActionResult> MarcarComoLeidas()
        {
            var idUsuarioClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (!int.TryParse(idUsuarioClaim, out int idUsuario))
            {
                return Unauthorized();
            }

            var notificaciones = await _context.Notificaciones
                .Where(n =>
                    n.IdUsuario == idUsuario &&
                    !n.Leida
                )
                .ToListAsync();

            foreach (var notificacion in notificaciones)
            {
                notificacion.Leida = true;
                notificacion.FechaLectura = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        // GET: Notificaciones/VerPublicacion/5
        // GET: Notificaciones/VerPublicacion/5
        public async Task<IActionResult> VerPublicacion(int id)
        {
            var idUsuarioClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (!int.TryParse(idUsuarioClaim, out int idUsuario))
            {
                return Unauthorized();
            }

            // Verificamos que la notificación pertenezca al usuario
            var notificacion = await _context.Notificaciones
                .FirstOrDefaultAsync(n =>
                    n.IdPublicacion == id &&
                    n.IdUsuario == idUsuario
                );

            if (notificacion == null)
            {
                return NotFound();
            }

            // Registramos el segundo visto:
            // el vendedor entró a la publicación.
            if (!notificacion.FechaAccesoPublicacion.HasValue)
            {
                notificacion.FechaAccesoPublicacion = DateTime.Now;

                await _context.SaveChangesAsync();
            }

            // Buscamos la publicación
            var publicacion = await _context.Publicaciones
                .FirstOrDefaultAsync(p => p.IdPublicacion == id);

            if (publicacion == null)
            {
                return NotFound();
            }

            return View(publicacion);
        }
    }
}