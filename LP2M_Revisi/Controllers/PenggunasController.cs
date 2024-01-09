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
    public class PenggunasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PenggunasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Penggunas
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
            var applicationDbContext = _context.Penggunas.Include(p => p.ProdiNavigation);
            return View(await applicationDbContext.ToListAsync());
        }
        public string GenerateNextId()
        {
            // Cari ID terakhir dalam database
            var lastId = _context.Penggunas
                .OrderByDescending(b => b.Id)
                .FirstOrDefault();

            int lastIdNumeric = 0;

            if (lastId != null)
            {
                // Jika ada ID terakhir, ambil angka dari ID tersebut
                lastIdNumeric = int.Parse(lastId.Id.Substring(3));
            }

            // Tingkatkan angka terakhir
            lastIdNumeric++;

            // Format angka terakhir sebagai "PPBXX" dengan angka menggunakan dua digit
            string nextId = $"USR{lastIdNumeric:D2}";

            return nextId;
        }

        // GET: Penggunas/Details/5
        public async Task<IActionResult> Details(string id)
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
            if (id == null || _context.Penggunas == null)
            {
                return NotFound();
            }

            var pengguna = await _context.Penggunas
                .Include(p => p.ProdiNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pengguna == null)
            {
                return NotFound();
            }

            return View(pengguna);
        }

        // GET: Penggunas/Create
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
            Pengguna pengguna = new Pengguna();
            pengguna.Id = GenerateNextId();

            ViewData["Prodi"] = new SelectList(_context.Prodis, "Id", "Nama");
            return View(pengguna);
        }

        // POST: Penggunas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nama,Username,Password,Role,Email,Notelepon,Prodi")] Pengguna pengguna)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pengguna);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Prodi"] = new SelectList(_context.Prodis, "Id", "Nama", pengguna.Prodi);
            return View(pengguna);
        }

        // GET: Penggunas/Edit/5
        public async Task<IActionResult> Edit(string id)
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
            if (id == null || _context.Penggunas == null)
            {
                return NotFound();
            }

            var pengguna = await _context.Penggunas.FindAsync(id);
            if (pengguna == null)
            {
                return NotFound();
            }
            ViewData["Prodi"] = new SelectList(_context.Prodis, "Id", "Nama", pengguna.Prodi);
            return View(pengguna);
        }

        // POST: Penggunas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Nama,Username,Password,Role,Email,Notelepon,Prodi")] Pengguna pengguna)
        {
            if (id != pengguna.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pengguna);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PenggunaExists(pengguna.Id))
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
            ViewData["Prodi"] = new SelectList(_context.Prodis, "Id", "Nama", pengguna.Prodi);
            return View(pengguna);
        }

        // GET: Penggunas/Delete/5
        public async Task<IActionResult> Delete(string id)
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
            if (id == null || _context.Penggunas == null)
            {
                return NotFound();
            }

            var pengguna = await _context.Penggunas
                .Include(p => p.ProdiNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pengguna == null)
            {
                return NotFound();
            }

            return View(pengguna);
        }

        // POST: Penggunas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Penggunas == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Penggunas'  is null.");
            }
            var pengguna = await _context.Penggunas.FindAsync(id);
            if (pengguna != null)
            {
                _context.Penggunas.Remove(pengguna);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PenggunaExists(string id)
        {
          return (_context.Penggunas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
