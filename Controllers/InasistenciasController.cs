using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TeaMall.Data;
using TeaMall.Models;

namespace TeaMall.Controllers
{
    public class InasistenciasController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _environment;

        public InasistenciasController(
            DataContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Inasistencias
        public async Task<IActionResult> Index()
        {
            var inasistencias = await _context.Inasistencias
                .OrderByDescending(i => i.FechaAusencia)
                .ToListAsync();

            return View(inasistencias);
        }

        // GET: Inasistencias/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inasistencias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            Inasistencia inasistencia,
            IFormFile? ArchivoSubido)
        {
            if (!ModelState.IsValid)
            {
                return View(inasistencia);
            }

            var idUsuarioClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(idUsuarioClaim, out int idUsuario))
            {
                return Unauthorized();
            }

            inasistencia.IdUsuario = idUsuario;

            inasistencia.FechaCarga = DateTime.Now;
            inasistencia.Estado = "Pendiente";

            // Guardar archivo si se seleccionó uno
            if (ArchivoSubido != null && ArchivoSubido.Length > 0)
            {
                string carpeta = Path.Combine(
                    _environment.WebRootPath,
                    "Uploads",
                    "Inasistencias"
                );

                if (!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }

                string extension = Path.GetExtension(ArchivoSubido.FileName);
                string nombreArchivo = $"{Guid.NewGuid()}{extension}";

                string rutaArchivo = Path.Combine(
                    carpeta,
                    nombreArchivo
                );

                using (var stream = new FileStream(
                    rutaArchivo,
                    FileMode.Create))
                {
                    await ArchivoSubido.CopyToAsync(stream);
                }

                inasistencia.Archivo =
                    "/Uploads/Inasistencias/" + nombreArchivo;
            }

            _context.Inasistencias.Add(inasistencia);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Create));
        }
    }
}