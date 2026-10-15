using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using TeaMall.Data;
using TeaMall.Models;

namespace TeaMall.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly DataContext _context;
        private readonly PasswordHasher<Usuario> _passwordHasher = new();

        public UsuariosController(DataContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            var locales = await _context.Locales.ToListAsync();

            ViewBag.Locales = locales;

            return View(usuarios);
        }

        // GET: Usuarios/Create
        public async Task<IActionResult> Create()
        {
            await CargarLocales();

            return View(new Usuario { Activo = true });
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            // Limpiar espacios innecesarios
            usuario.DNI = usuario.DNI?.Trim() ?? "";
            usuario.Nombre = usuario.Nombre?.Trim() ?? "";
            usuario.Apellido = usuario.Apellido?.Trim() ?? "";
            usuario.Email = usuario.Email?.Trim() ?? "";

            // Validar DNI duplicado
            if (!string.IsNullOrWhiteSpace(usuario.DNI))
            {
                bool dniExiste = await _context.Usuarios
                    .AnyAsync(u => u.DNI == usuario.DNI);

                if (dniExiste)
                {
                    ModelState.AddModelError(
                        nameof(usuario.DNI),
                        "Ya existe un usuario registrado con ese DNI."
                    );
                }
            }

            // Validar email duplicado, ignorando mayúsculas
            if (!string.IsNullOrWhiteSpace(usuario.Email))
            {
                string emailNormalizado = usuario.Email.ToLower();

                bool emailExiste = await _context.Usuarios
                    .AnyAsync(u => u.Email.ToLower() == emailNormalizado);

                if (emailExiste)
                {
                    ModelState.AddModelError(
                        nameof(usuario.Email),
                        "Ya existe un usuario registrado con ese email."
                    );
                }
            }

            

            // Validar local asignado
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
                        "El local seleccionado no existe o está inactivo."
                    );
                }
            }

            // Guardar solamente si todas las validaciones pasaron
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

            // Recargar los locales si hay errores
            await CargarLocales();

            return View(usuario);
        }

        // GET: Usuarios/Edit/5
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

    await CargarLocales();

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

    // La contraseña no se modifica desde este formulario
    ModelState.Remove(nameof(usuario.Contrasenia));

    // Limpiar espacios innecesarios
    usuario.DNI = usuario.DNI?.Trim() ?? "";
    usuario.Nombre = usuario.Nombre?.Trim() ?? "";
    usuario.Apellido = usuario.Apellido?.Trim() ?? "";
    usuario.Email = usuario.Email?.Trim() ?? "";

    // Comprobar DNI duplicado en otro usuario
    if (await _context.Usuarios.AnyAsync(u =>
        u.IdUsuario != id && u.DNI == usuario.DNI))
    {
        ModelState.AddModelError(
            nameof(usuario.DNI),
            "Ese DNI ya pertenece a otro usuario."
        );
    }

    // Comprobar email duplicado en otro usuario
    if (await _context.Usuarios.AnyAsync(u =>
        u.IdUsuario != id &&
        u.Email.ToLower() == usuario.Email.ToLower()))
    {
        ModelState.AddModelError(
            nameof(usuario.Email),
            "Ese email ya pertenece a otro usuario."
        );
    }

    // Comprobar que el rol sea válido
    if (usuario.Rol != "Administrador" &&
        usuario.Rol != "Vendedor")
    {
        ModelState.AddModelError(
            nameof(usuario.Rol),
            "Seleccioná un rol válido."
        );
    }

    // Comprobar que el local exista y esté activo
    if (usuario.IdLocal.HasValue)
    {
        bool localValido = await _context.Locales.AnyAsync(l =>
            l.IdLocal == usuario.IdLocal.Value && l.Activo);

        if (!localValido)
        {
            ModelState.AddModelError(
                nameof(usuario.IdLocal),
                "El local seleccionado no existe o está inactivo."
            );
        }
    }

    if (ModelState.IsValid)
    {
        // Actualizar los campos permitidos
        usuarioExistente.DNI = usuario.DNI;
        usuarioExistente.Nombre = usuario.Nombre;
        usuarioExistente.Apellido = usuario.Apellido;
        usuarioExistente.Email = usuario.Email;
        usuarioExistente.Rol = usuario.Rol;
        usuarioExistente.IdLocal = usuario.IdLocal;
        usuarioExistente.Activo = usuario.Activo;

        // Conservar la contraseña y el avatar existentes
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    await CargarLocales();

    return View(usuario);
}

// POST: Usuarios/Desactivar/5
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Desactivar(int id)
{
    var usuario = await _context.Usuarios.FindAsync(id);

    if (usuario == null)
    {
        return NotFound();
    }

    usuario.Activo = !usuario.Activo;

    await _context.SaveChangesAsync();

    return RedirectToAction(nameof(Index));
}


        // Cargar locales activos en el selector
        private async Task CargarLocales()
{
    var locales = await _context.Locales
        .Where(l => l.Activo)
        .OrderBy(l => l.Nombre)
        .ToListAsync();

    ViewBag.Locales = new SelectList(
        locales,
        "IdLocal",
        "Nombre"
    );
}
    }
}