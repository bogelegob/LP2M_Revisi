using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LP2M_Revisi.Models;
using Newtonsoft.Json;

namespace LP2M_Revisi.Controllers
{
    public class HakpatensController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HakpatensController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Hakpatens
        public async Task<IActionResult> Index()
        {
            Pengguna penggunaModel;
            string serializedModel = HttpContext.Session.GetString("Identity");
            Console.WriteLine(serializedModel);
            if (serializedModel == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                penggunaModel = JsonConvert.DeserializeObject<Pengguna>(serializedModel);
            }
            if (penggunaModel.Role != "Admin" && penggunaModel.Role != "Karyawan")
            {
                return RedirectToAction("Index", "Login");
            }
            if (penggunaModel.Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
            }
            ViewBag.Pengguna = penggunaModel.Role;
            var applicationDbContext = _context.Hakpatens.Include(h => h.EditbyNavigation).Include(h => h.InputbyNavigation);
            return View(await applicationDbContext.ToListAsync());
        }
        public string GenerateNextId()
        {
            // Cari ID terakhir dalam database
            var lastId = _context.Hakpatens
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
            string nextId = $"PHP{lastIdNumeric:D2}";

            return nextId;
        }

        // GET: Hakpatens/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Hakpatens == null)
            {
                return NotFound();
            }

            var hakpaten = await _context.Hakpatens
                .Include(h => h.EditbyNavigation)
                .Include(h => h.InputbyNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hakpaten == null)
            {
                return NotFound();
            }

            return View(hakpaten);
        }

        // GET: Hakpatens/Create
        public IActionResult Create()
        {
            Hakpaten hakpaten = new Hakpaten();
            hakpaten.Id = GenerateNextId();
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            return View(hakpaten);
        }

        // POST: Hakpatens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Judul,Nopermohonan,TanggalPenerimaan,Status,Inputby,Inputdate,Editby,Editdate")] Hakpaten hakpaten)
        {
            if (ModelState.IsValid)
            {
                var serializedModel = HttpContext.Session.GetString("Identity");
                var pengguna = JsonConvert.DeserializeObject<Pengguna>(serializedModel);
                hakpaten.Inputby = pengguna.Id;
                hakpaten.Editby = pengguna.Id;
                DateTime tgl = DateTime.Now;
                hakpaten.Inputdate = tgl;
                hakpaten.Editdate = tgl;
                hakpaten.Status = 1;
                hakpaten.Id = GenerateNextId();
                _context.Add(hakpaten);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", hakpaten.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", hakpaten.Inputby);
            return View(hakpaten);
        }

        // GET: Hakpatens/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Hakpatens == null)
            {
                return NotFound();
            }

            var hakpaten = await _context.Hakpatens.FindAsync(id);
            if (hakpaten == null)
            {
                return NotFound();
            }
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Id", hakpaten.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Id", hakpaten.Inputby);
            return View(hakpaten);
        }

        // POST: Hakpatens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Judul,Nopermohonan,TanggalPenerimaan,Status,Inputby,Inputdate,Editby,Editdate")] Hakpaten hakpaten)
        {
            if (id != hakpaten.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hakpaten);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HakpatenExists(hakpaten.Id))
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
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Id", hakpaten.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Id", hakpaten.Inputby);
            return View(hakpaten);
        }

        // GET: Hakpatens/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Hakpatens == null)
            {
                return NotFound();
            }

            var hakpaten = await _context.Hakpatens
                .Include(h => h.EditbyNavigation)
                .Include(h => h.InputbyNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hakpaten == null)
            {
                return NotFound();
            }

            return View(hakpaten);
        }

        // POST: Hakpatens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Hakpatens == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Hakpatens'  is null.");
            }
            var hakpaten = await _context.Hakpatens.FindAsync(id);
            if (hakpaten != null)
            {
                _context.Hakpatens.Remove(hakpaten);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HakpatenExists(string id)
        {
          return (_context.Hakpatens?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
