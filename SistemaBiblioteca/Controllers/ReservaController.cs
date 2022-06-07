using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SistemaBiblioteca.Data;
using SistemaBiblioteca.Models;
using SistemaBiblioteca.Request;

namespace SistemaBiblioteca.Controllers
{
    public class ReservaController : Controller
    {
        private readonly SistemaBibliotecaContext _context;

        public ReservaController(SistemaBibliotecaContext context)
        {
            _context = context;
        }

        // GET: Reserva
        public async Task<IActionResult> Index()
        {
            var sistemaBibliotecaContext = _context.Reservas.Include(r => r.Usuario);
            return View(await sistemaBibliotecaContext.ToListAsync());
        }

        // GET: Reserva/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reservas == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // GET: Reserva/Create
        public IActionResult Create()
        {
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Id");
            return View();
        }

        // POST: Reserva/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FechaInicio,FechaFin,DiasReserva,UsuarioId")] Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Id", reserva.UsuarioId);
            return View(reserva);
        }

        // GET: Reserva/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reservas == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Id", reserva.UsuarioId);
            return View(reserva);
        }

        // POST: Reserva/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FechaInicio,FechaFin,DiasReserva,UsuarioId")] Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
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
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Id", reserva.UsuarioId);
            return View(reserva);
        }

        // GET: Reserva/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reservas == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reserva/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reservas == null)
            {
                return Problem("Entity set 'SistemaBibliotecaContext.Reservas'  is null.");
            }
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ListaMaterialReserva() {
            IList<Material> materiales = _context.Materiales.ToList();
            return View(materiales);
        }

        public IActionResult InformacionMaterial(int id) {
            Material material = _context.Materiales.Where(x => x.Id == id).FirstOrDefault();
            return View(material);
        }

        public ActionResult ReservaFormulario() {
            return View();
        }

        private bool ReservaExists(int id)
        {
          return _context.Reservas.Any(e => e.Id == id);
        }


        [HttpGet]
        [IgnoreAntiforgeryToken]
        public IActionResult Export(string html) {
            using (MemoryStream stream = new System.IO.MemoryStream()) {
                StringReader sr = new StringReader(html);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 100f, 0f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "ComprobanteReserva.pdf");
            }
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public ActionResult CompletarReserva(string request) {

            ReservaRequest requestObject = JsonConvert.DeserializeObject<ReservaRequest>(request);
            ICollection<Material> materialsReserve = new List<Material>();
            foreach(int materialId in requestObject.MaterialIds) {
                Material material = _context.Materiales.Where(x => x.Id == materialId).FirstOrDefault();
                material.State = false;
                _context.Materiales.Update(material);
                _context.SaveChanges();
                materialsReserve.Add(material);

            }
            Reserva reservaToAdd = new Reserva() {
                FechaInicio = requestObject.FechaInicio,
                FechaFin = requestObject.FechaFin,
                DiasReserva = requestObject.DiasReserva,
                Materiales = materialsReserve,
                UsuarioId = requestObject.IdUsuario,
                Usuario = _context.Usuarios.Where(x => x.Id == requestObject.IdUsuario).FirstOrDefault()
            };

            _context.Add(reservaToAdd);
            _context.SaveChanges();

            return Json(new { redirectUrl = String.Format("{0}", "/Reserva/Export?html=" + requestObject.HTMLFactura) });


        }

        [HttpGet]
        [IgnoreAntiforgeryToken]
        public IActionResult ReservaFin(string request) {
            IList<int> materialIds = JsonConvert.DeserializeObject<IList<int>>(request);
            IList<Material> materiales = new List<Material>();
            foreach(int id in materialIds) {
                materiales.Add(_context.Materiales.Where(x => x.Id == id).FirstOrDefault());
            }
            return View(materiales);
        }

    }
}
