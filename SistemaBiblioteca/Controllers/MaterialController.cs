using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SistemaBiblioteca.Data;

namespace SistemaBiblioteca.Models
{
    public class MaterialController : Controller
    {
        private readonly SistemaBibliotecaContext _context;

        public MaterialController(SistemaBibliotecaContext context)
        {
            _context = context;
        }

        // GET: Material
        public async Task<IActionResult> Index()
        {
            var sistemaBibliotecaContext = _context.Materiales.Include(m => m.Prestamo).Include(m => m.Reserva);
            return View(await sistemaBibliotecaContext.ToListAsync());
        }

        // GET: Material/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Materiales == null)
            {
                return NotFound();
            }

            var material = await _context.Materiales
                .Include(m => m.Prestamo)
                .Include(m => m.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }

        // GET: Material/Create
        public IActionResult Create()
        {
            ViewData["PrestamoId"] = new SelectList(_context.Prestamos, "Id", "Id");
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id");
            return View();
        }

        // POST: Material/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ISBN,Titulo,Descripcion,Paginas,Tipo,Categoria,State,PrestamoId,ReservaId")] Material material)
        {
            if (ModelState.IsValid)
            {
                _context.Add(material);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PrestamoId"] = new SelectList(_context.Prestamos, "Id", "Id", material.PrestamoId);
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", material.ReservaId);
            return View(material);
        }

        // GET: Material/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Materiales == null)
            {
                return NotFound();
            }

            var material = await _context.Materiales.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }
            ViewData["PrestamoId"] = new SelectList(_context.Prestamos, "Id", "Id", material.PrestamoId);
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", material.ReservaId);
            return View(material);
        }

        // POST: Material/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ISBN,Titulo,Descripcion,Paginas,Tipo,Categoria,State,PrestamoId,ReservaId")] Material material)
        {
            if (id != material.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(material);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialExists(material.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PrestamoId"] = new SelectList(_context.Prestamos, "Id", "Id", material.PrestamoId);
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", material.ReservaId);
            return View(material);
        }

        // GET: Material/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Materiales == null)
            {
                return NotFound();
            }

            var material = await _context.Materiales
                .Include(m => m.Prestamo)
                .Include(m => m.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }

        // POST: Material/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Materiales == null)
            {
                return Problem("Entity set 'SistemaBibliotecaContext.Materiales'  is null.");
            }
            var material = await _context.Materiales.FindAsync(id);
            if (material != null)
            {
                _context.Materiales.Remove(material);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public ActionResult ConsultarMaterial(string parameter = "") {
            IList<Material> materiales = new List<Material>();
            if (parameter != "") {
                materiales = _context.Materiales.Where(x => (x.Paginas.ToString().Contains(parameter)) || (x.Autores.Select(a => a.Name).Where(a => a.Contains(parameter)).Count() > 1) || x.Titulo.Contains(parameter) || x.Tipo.Contains(parameter) || x.Categoria.Contains(parameter)  || x.Id.ToString() == parameter ).ToList();
            }
            return Json(materiales);
        }



        private bool MaterialExists(int id)
        {
          return _context.Materiales.Any(e => e.Id == id);
        }
    }
}
