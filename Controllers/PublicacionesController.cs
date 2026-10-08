using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeaMall.Data;

namespace TeaMall.Controllers
{
    public class PublicacionesController : Controller
    {
        private readonly DataContext _context;

        public PublicacionesController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var publicaciones = await _context.Publicaciones.ToListAsync();

            return Json(publicaciones);
        }
    }
}