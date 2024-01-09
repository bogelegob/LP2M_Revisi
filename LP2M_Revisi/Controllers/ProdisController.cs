using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LP2M_Revisi.Models;

namespace LP2M_Revisi.Controllers
{
    public class ProdisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProdisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Prodis
        public async Task<IActionResult> Index()
        {
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
            }
            return _context.Prodis != null ? 
                          View(await _context.Prodis.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Prodis'  is null.");
        }


        // GET: Prodis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
            }
            if (id == null || _context.Prodis == null)
            {
                return NotFound();
            }

            var prodi = await _context.Prodis
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prodi == null)
            {
                return NotFound();
            }

            return View(prodi);
        }

        // GET: Prodis/Create
        public IActionResult Create()
        {
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
            }
            return View();
        }

        // POST: Prodis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nama")] Prodi prodi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prodi);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Data berhasil ditambahkan.";
                return RedirectToAction(nameof(Index));
            }
            return View(prodi);
        }

        // GET: Prodis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
            }
            if (id == null || _context.Prodis == null)
            {
                return NotFound();
            }

            var prodi = await _context.Prodis.FindAsync(id);
            if (prodi == null)
            {
                return NotFound();
            }
            return View(prodi);
        }

        // POST: Prodis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nama")] Prodi prodi)
        {
            if (id != prodi.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prodi);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Data berhasil diedit.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdiExists(prodi.Id))
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
            return View(prodi);
        }

        // GET: Prodis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
            }
            if (id == null || _context.Prodis == null)
            {
                return NotFound();
            }

            var prodi = await _context.Prodis
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prodi == null)
            {
                return NotFound();
            }

            return View(prodi);
        }

        // POST: Prodis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Prodis == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Prodis'  is null.");
            }
            var prodi = await _context.Prodis.FindAsync(id);
            if (prodi != null)
            {
                _context.Prodis.Remove(prodi);
            }
            
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Data berhasil dihapus.";
            return RedirectToAction(nameof(Index));
        }

        private bool ProdiExists(int id)
        {
          return (_context.Prodis?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
