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
    public class HakciptumsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HakciptumsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Hakciptums
        public async Task<IActionResult> Index()
        {
            Pengguna penggunaModel;
            string serializedModel = HttpContext.Session.GetString("Identity");
            string Role = HttpContext.Session.GetString("selectedRole");
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
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
            }
            ViewBag.Pengguna = penggunaModel.Role;
            var applicationDbContext = _context.Hakcipta.Include(h => h.EditbyNavigation).Include(h => h.InputbyNavigation).Where(h => h.Status == 1);
            return View(await applicationDbContext.ToListAsync());
        }
        public string GenerateNextId()
        {
            // Cari ID terakhir dalam database
            var lastId = _context.Hakcipta
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
            string nextId = $"PHC{lastIdNumeric:D2}";

            return nextId;
        }

        // GET: Hakciptums/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Hakcipta == null)
            {
                return NotFound();
            }

            var hakciptum = await _context.Hakcipta
                .Include(h => h.EditbyNavigation)
                .Include(h => h.InputbyNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hakciptum == null)
            {
                return NotFound();
            }

            return View(hakciptum);
        }

        // GET: Hakciptums/Create
        public IActionResult Create()
        {
            Hakciptum hakciptum = new Hakciptum();
            hakciptum.Id = GenerateNextId();
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            return View(hakciptum);
        }

        // POST: Hakciptums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Judul,Noaplikasi,Nosertifikat,Keterangan,Status,Inputby,Inputdate,Editby,Editdate")] Hakciptum hakciptum)
        {
            if (ModelState.IsValid)
            {
                var serializedModel = HttpContext.Session.GetString("Identity");
                var pengguna = JsonConvert.DeserializeObject<Pengguna>(serializedModel);
                hakciptum.Inputby = pengguna.Id;
                hakciptum.Editby = pengguna.Id;
                DateTime tgl = DateTime.Now;
                hakciptum.Inputdate = tgl;
                hakciptum.Editdate = tgl;
                hakciptum.Status = 1;
                hakciptum.Id = GenerateNextId();
                _context.Add(hakciptum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", hakciptum.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", hakciptum.Inputby);
            return View(hakciptum);
        }

        // GET: Hakciptums/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Hakcipta == null)
            {
                return NotFound();
            }

            var hakciptum = await _context.Hakcipta.FindAsync(id);
            if (hakciptum == null)
            {
                return NotFound();
            }
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Id", hakciptum.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Id", hakciptum.Inputby);
            return View(hakciptum);
        }

        // POST: Hakciptums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Judul,Noaplikasi,Nosertifikat,Keterangan,Status,Inputby,Inputdate,Editby,Editdate")] Hakciptum hakciptum)
        {
            if (id != hakciptum.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hakciptum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HakciptumExists(hakciptum.Id))
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
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Id", hakciptum.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Id", hakciptum.Inputby);
            return View(hakciptum);
        }

        // GET: Hakciptums/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Hakcipta == null)
            {
                return NotFound();
            }

            var hakciptum = await _context.Hakcipta
                .Include(h => h.EditbyNavigation)
                .Include(h => h.InputbyNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (hakciptum == null)
            {
                return NotFound();
            }

            return View(hakciptum);
        }

        // POST: Hakciptums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Hakcipta == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Hakcipta'  is null.");
            }
            var hakciptum = await _context.Hakcipta.FindAsync(id);
            if (hakciptum != null)
            {
                _context.Hakcipta.Remove(hakciptum);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HakciptumExists(string id)
        {
          return (_context.Hakcipta?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
