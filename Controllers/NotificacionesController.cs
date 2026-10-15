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
        public async Task<IActionResult> Index()
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

            return View(notificaciones);
        }
    }
}