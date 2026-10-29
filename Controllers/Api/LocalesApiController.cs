
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeaMall.Data;
using TeaMall.DTOs;
using TeaMall.Models;

namespace TeaMall.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LocalesApiController : ControllerBase
    {
        private readonly DataContext _context;

        public LocalesApiController(DataContext context)
        {
            _context = context;
        }

        // GET: api/localesapi
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var locales = await _context.Locales
                .AsNoTracking()
                .Select(l => new LocalDto
                {
                    IdLocal = l.IdLocal,
                    Nombre = l.Nombre,
                    Activo = l.Activo
                })
                .ToListAsync();

            return Ok(locales);
        }

        // GET: api/localesapi/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPorId(int id)
        {
            var local = await _context.Locales
                .AsNoTracking()
                .Where(l => l.IdLocal == id)
                .Select(l => new LocalDto
                {
                    IdLocal = l.IdLocal,
                    Nombre = l.Nombre,
                    Activo = l.Activo
                })
                .FirstOrDefaultAsync();

            if (local == null)
                return NotFound(new
                {
                    mensaje = "No se encontró el local."
                });

            return Ok(local);
        }

        // POST: api/localesapi
        [HttpPost]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Administrador")]
        public async Task<IActionResult> Post(
            [FromBody] CrearLocalDto datos)
        {
            var nuevoLocal = new Local
            {
                Nombre = datos.Nombre.Trim(),
                Activo = true
            };

            _context.Locales.Add(nuevoLocal);
            await _context.SaveChangesAsync();

            var respuesta = new LocalDto
            {
                IdLocal = nuevoLocal.IdLocal,
                Nombre = nuevoLocal.Nombre,
                Activo = nuevoLocal.Activo
            };

            return CreatedAtAction(
                nameof(GetPorId),
                new { id = nuevoLocal.IdLocal },
                respuesta);
        }

        // PUT: api/localesapi/1
        [HttpPut("{id}")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Administrador")]
        public async Task<IActionResult> Put(
            int id,
            [FromBody] ActualizarLocalDto datos)
        {
            var local = await _context.Locales.FindAsync(id);

            if (local == null)
                return NotFound(new
                {
                    mensaje = "No se encontró el local."
                });

            local.Nombre = datos.Nombre.Trim();
            local.Activo = datos.Activo;

            await _context.SaveChangesAsync();

            var respuesta = new LocalDto
            {
                IdLocal = local.IdLocal,
                Nombre = local.Nombre,
                Activo = local.Activo
            };

            return Ok(respuesta);
        }

        // DELETE: api/localesapi/1
        [HttpDelete("{id}")]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var local = await _context.Locales.FindAsync(id);

            if (local == null)
                return NotFound(new
                {
                    mensaje = "No se encontró el local."
                });

            if (!local.Activo)
                return BadRequest(new
                {
                    mensaje = "El local ya está dado de baja."
                });

            // Baja lógica: no eliminamos el registro de la base de datos.
            local.Activo = false;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
