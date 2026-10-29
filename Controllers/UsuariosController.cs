
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using TeaMall.Data;
using TeaMall.Models;

namespace TeaMall.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly DataContext _context;
        private readonly PasswordHasher<Usuario> _passwordHasher = new();

        public UsuariosController(DataContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        [HttpGet]
        public IActionResult Index()
        {
            // Los datos se obtienen mediante Listar() usando AJAX.
            return View();
        }

        // GET: Usuarios/Listar
        [HttpGet]
        public async Task<IActionResult> Listar(
            int pagina = 1,
            int tamaño = 5,
            string busqueda = "")
        {
            pagina = Math.Max(1, pagina);
            tamaño = Math.Clamp(tamaño, 1, 50);

            var consulta = _context.Usuarios
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                busqueda = busqueda.Trim();

                consulta = consulta.Where(u =>
                    u.Nombre.Contains(busqueda) ||
                    u.Apellido.Contains(busqueda) ||
                    u.DNI.Contains(busqueda) ||
                    u.Email.Contains(busqueda));
            }

            var total = await consulta.CountAsync();

            var totalPaginas = (int)Math.Ceiling(
                total / (double)tamaño);

            pagina = totalPaginas == 0
                ? 1
                : Math.Min(pagina, totalPaginas);

            var usuarios = await (
                from u in consulta
                join l in _context.Locales
                    on u.IdLocal equals (int?)l.IdLocal into grupoLocales
                from l in grupoLocales.DefaultIfEmpty()
                orderby u.Nombre, u.Apellido
                select new
                {
                    idUsuario = u.IdUsuario,
                    dni = u.DNI,
                    nombre = u.Nombre,
                    apellido = u.Apellido,
                    email = u.Email,
                    rol = u.Rol,
                    idLocal = u.IdLocal,
                    nombreLocal = l != null ? l.Nombre : null,
                    activo = u.Activo
                })
                .Skip((pagina - 1) * tamaño)
                .Take(tamaño)
                .ToListAsync();

            return Json(new
            {
                usuarios,
                pagina,
                tamaño,
                total,
                totalPaginas
            });
        }

        // GET: Usuarios/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Usuario { Activo = true });
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            usuario.DNI = usuario.DNI?.Trim() ?? "";
            usuario.Nombre = usuario.Nombre?.Trim() ?? "";
            usuario.Apellido = usuario.Apellido?.Trim() ?? "";
            usuario.Email = usuario.Email?.Trim() ?? "";

            if (!string.IsNullOrWhiteSpace(usuario.DNI))
            {
                bool dniExiste = await _context.Usuarios
                    .AnyAsync(u => u.DNI == usuario.DNI);

                if (dniExiste)
                {
                    ModelState.AddModelError(
                        nameof(usuario.DNI),
                        "Ya existe un usuario registrado con ese DNI.");
                }
            }

            if (!string.IsNullOrWhiteSpace(usuario.Email))
            {
                string emailNormalizado = usuario.Email.ToLower();

                bool emailExiste = await _context.Usuarios
                    .AnyAsync(u => u.Email.ToLower() == emailNormalizado);

                if (emailExiste)
                {
                    ModelState.AddModelError(
                        nameof(usuario.Email),
                        "Ya existe un usuario registrado con ese email.");
                }
            }

            if (usuario.IdLocal.HasValue)
            {
                bool localValido = await _context.Locales
                    .AnyAsync(l =>
                        l.IdLocal == usuario.IdLocal.Value &&
                        l.Activo);

                if (!localValido)
                {
                    ModelState.AddModelError(
                        nameof(usuario.IdLocal),
                        "El local seleccionado no existe o está inactivo.");
                }
            }

            if (ModelState.IsValid)
            {
                usuario.Contrasenia = _passwordHasher.HashPassword(
                    usuario,
                    usuario.Contrasenia
                );

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            await CargarNombreLocalActual(usuario.IdLocal);

            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }

            var usuarioExistente = await _context.Usuarios.FindAsync(id);

            if (usuarioExistente == null)
            {
                return NotFound();
            }

            ModelState.Remove(nameof(usuario.Contrasenia));

            usuario.DNI = usuario.DNI?.Trim() ?? "";
            usuario.Nombre = usuario.Nombre?.Trim() ?? "";
            usuario.Apellido = usuario.Apellido?.Trim() ?? "";
            usuario.Email = usuario.Email?.Trim() ?? "";

            if (await _context.Usuarios.AnyAsync(u =>
                u.IdUsuario != id && u.DNI == usuario.DNI))
            {
                ModelState.AddModelError(
                    nameof(usuario.DNI),
                    "Ese DNI ya pertenece a otro usuario.");
            }

            if (await _context.Usuarios.AnyAsync(u =>
                u.IdUsuario != id &&
                u.Email.ToLower() == usuario.Email.ToLower()))
            {
                ModelState.AddModelError(
                    nameof(usuario.Email),
                    "Ese email ya pertenece a otro usuario.");
            }

            if (usuario.Rol != "Administrador" &&
                usuario.Rol != "Vendedor")
            {
                ModelState.AddModelError(
                    nameof(usuario.Rol),
                    "Seleccioná un rol válido.");
            }

            if (usuario.IdLocal.HasValue)
            {
                bool localValido = await _context.Locales.AnyAsync(l =>
                    l.IdLocal == usuario.IdLocal.Value && l.Activo);

                if (!localValido)
                {
                    ModelState.AddModelError(
                        nameof(usuario.IdLocal),
                        "El local seleccionado no existe o está inactivo.");
                }
            }

            if (ModelState.IsValid)
            {
                usuarioExistente.DNI = usuario.DNI;
                usuarioExistente.Nombre = usuario.Nombre;
                usuarioExistente.Apellido = usuario.Apellido;
                usuarioExistente.Email = usuario.Email;
                usuarioExistente.Rol = usuario.Rol;
                usuarioExistente.IdLocal = usuario.IdLocal;
                usuarioExistente.Activo = usuario.Activo;

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            await CargarNombreLocalActual(usuario.IdLocal);

            return View(usuario);
        }

        // POST: Usuarios/Desactivar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound(new
                {
                    mensaje = "No se encontró el usuario."
                });
            }

            usuario.Activo = !usuario.Activo;
            await _context.SaveChangesAsync();

            return Json(new
            {
                mensaje = usuario.Activo
                    ? "Usuario activado correctamente."
                    : "Usuario desactivado correctamente.",
                idUsuario = usuario.IdUsuario,
                activo = usuario.Activo
            });
        }

        // Recuperar el nombre del local sin cargar todos los locales.
        private async Task CargarNombreLocalActual(int? idLocal)
        {
            if (!idLocal.HasValue)
            {
                ViewBag.NombreLocalActual = "";
                return;
            }

            ViewBag.NombreLocalActual = await _context.Locales
                .AsNoTracking()
                .Where(l => l.IdLocal == idLocal.Value)
                .Select(l => l.Nombre)
                .FirstOrDefaultAsync() ?? "";
        }
    }
}
