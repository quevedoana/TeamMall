using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeaMall.Data;
using TeaMall.Models;

namespace TeaMall.Controllers
{
    public class LocalesController : Controller
    {
        private readonly DataContext _context;

        public LocalesController(DataContext context)
        {
            _context = context;
        }

        // GET: Locales
        public async Task<IActionResult> Index()
        {
            var locales = await _context.Locales.ToListAsync();

            return View(locales);
        }

        // GET: Locales/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Locales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Local local)
        {
            if (ModelState.IsValid)
            {
                _context.Add(local);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(local);
        }

        // GET: Locales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var local = await _context.Locales.FindAsync(id);

            if (local == null)
            {
                return NotFound();
            }

            return View(local);
        }

        // POST: Locales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Local local)
        {
            if (id != local.IdLocal)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(local);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(local);
        }

        // POST: Locales/Desactivar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Desactivar(int id)
        {
            var local = await _context.Locales.FindAsync(id);

            if (local == null)
            {
                return NotFound();
            }

            local.Activo = false;

            _context.Update(local);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}