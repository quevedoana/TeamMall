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
        // GET: Inasistencias
        public async Task<IActionResult> Index(
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            string? tipo,
            int pagina = 1)
        {
            const int registrosPorPagina = 10;

            if (pagina < 1)
            {
                pagina = 1;
            }

            // Si es administrador, puede ver todas las inasistencias
            if (User.IsInRole("Administrador"))
            {
                var consulta = _context.Inasistencias
                    .AsQueryable();

                // Filtro por fecha desde
                if (fechaDesde.HasValue)
                {
                    consulta = consulta.Where(i =>
                        i.FechaAusencia >= fechaDesde.Value);
                }

                // Filtro por fecha hasta
                if (fechaHasta.HasValue)
                {
                    consulta = consulta.Where(i =>
                        i.FechaAusencia <= fechaHasta.Value);
                }

                // Filtro por tipo
                if (!string.IsNullOrEmpty(tipo))
                {
                    consulta = consulta.Where(i =>
                        i.Tipo == tipo);
                }

                // Cantidad total de registros
                var totalRegistros = await consulta.CountAsync();

                // Cantidad total de páginas
                var totalPaginas = (int)Math.Ceiling(
                    totalRegistros / (double)registrosPorPagina
                );

                if (totalPaginas > 0 && pagina > totalPaginas)
                {
                    pagina = totalPaginas;
                }

                // Registros de la página actual
                var inasistenciasAdmin = await consulta
                    .OrderByDescending(i => i.FechaAusencia)
                    .Skip((pagina - 1) * registrosPorPagina)
                    .Take(registrosPorPagina)
                    .ToListAsync();

                // Usuarios para mostrar nombre en lugar de ID
                var usuarios = await _context.Usuarios
                    .ToDictionaryAsync(
                        u => u.IdUsuario,
                        u => u.Nombre + " " + u.Apellido
                    );

                ViewBag.Usuarios = usuarios;
                ViewBag.PaginaActual = pagina;
                ViewBag.TotalPaginas = totalPaginas;
                ViewBag.TotalRegistros = totalRegistros;
                ViewBag.FechaDesde = fechaDesde;
                ViewBag.FechaHasta = fechaHasta;
                ViewBag.Tipo = tipo;

                return View(inasistenciasAdmin);
            }

            // Obtenemos el ID del usuario logueado
            var idUsuarioClaim = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

            if (!int.TryParse(idUsuarioClaim, out int idUsuario))
            {
                return Unauthorized();
            }

            // El vendedor solamente ve sus propias inasistencias
            var consultaVendedor = _context.Inasistencias
                .Where(i => i.IdUsuario == idUsuario);

            var totalRegistrosVendedor =
                await consultaVendedor.CountAsync();

            var totalPaginasVendedor = (int)Math.Ceiling(
                totalRegistrosVendedor / (double)registrosPorPagina
            );

            if (totalPaginasVendedor > 0 &&
                pagina > totalPaginasVendedor)
            {
                pagina = totalPaginasVendedor;
            }

            var inasistencias = await consultaVendedor
                .OrderByDescending(i => i.FechaAusencia)
                .Skip((pagina - 1) * registrosPorPagina)
                .Take(registrosPorPagina)
                .ToListAsync();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginasVendedor;
            ViewBag.TotalRegistros = totalRegistrosVendedor;

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

            return RedirectToAction(nameof(Index));
        }
    }
}