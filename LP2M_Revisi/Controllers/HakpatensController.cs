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
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
            }
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
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
            }
            Hakpaten hakpaten = new Hakpaten();
            hakpaten.Id = GenerateNextId();
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            return View(hakpaten);
        }

        // POST: Hakpatens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Judul,Nopermohonan,TanggalPenerimaan,Status,Inputby,Inputdate,Editby,Editdate")] Hakpaten hakpaten, string SelectedUserIds)
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
                string[] selectedIdsArray = SelectedUserIds.Split(',');

                foreach (var userId in selectedIdsArray)
                {
                    if (!string.IsNullOrEmpty(userId))
                    {
                        // Buat objek Detailbuku dan set nilainya
                        var detailhakpaten = new Detailhakpaten
                        {
                            Idhakpaten = hakpaten.Id, // Sesuaikan dengan properti yang sesuai
                            Idpengguna = userId,
                        };

                        // Tambahkan objek Detailbuku ke konteks
                        _context.Detailhakpatens.Add(detailhakpaten);
                    }
                }
                _context.Add(hakpaten);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", hakpaten.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", hakpaten.Inputby);
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            return View(hakpaten);
        }

        // GET: Hakpatens/Edit/5
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
            if (id == null || _context.Hakpatens == null)
            {
                return NotFound();
            }

            var hakpaten = await _context.Hakpatens.FindAsync(id);
            if (hakpaten == null)
            {
                return NotFound();
            }
            var detailbuku = await _context.Detailhakpatens
            .Where(d => d.Idhakpaten == id)
            .Select(d => d.Idpengguna)
            .ToListAsync();
            var penggunaDetails = await _context.Penggunas
            .Where(p => detailbuku.Contains(p.Id))
            .Select(p => new { Id = p.Id, Nama = p.Nama })
            .ToListAsync();
            ViewBag.Detail = penggunaDetails;
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Id", hakpaten.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Id", hakpaten.Inputby);
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            return View(hakpaten);
        }

        // POST: Hakpatens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Judul,Nopermohonan,TanggalPenerimaan,Status,Inputby,Inputdate,Editby,Editdate")] Hakpaten hakpaten, string SelectedUserIds)
        {
            if (id != hakpaten.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string[] selectedIdsArray = SelectedUserIds.Split(',');

                    var existingDetailsurats = await _context.Detailhakpatens
                        .Where(d => d.Idhakpaten == hakpaten.Id)
                        .ToListAsync();

                    _context.Detailhakpatens.RemoveRange(existingDetailsurats);
                    await _context.SaveChangesAsync();

                    // Add new Detailsurat entries based on the selected user IDs
                    foreach (var userId in selectedIdsArray)
                    {
                        if (!string.IsNullOrEmpty(userId))
                        {
                            // Buat objek Detailbuku dan set nilainya
                            var detailsurat = new Detailhakpaten
                            {
                                Idhakpaten = hakpaten.Id,
                                Idpengguna = userId,
                            };
                            // Tambahkan objek Detailbuku ke konteks
                            _context.Detailhakpatens.Add(detailsurat);
                        }
                    }
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
            var detailbuku = await _context.Detailhakpatens
            .Where(d => d.Idhakpaten == id)
            .Select(d => d.Idpengguna)
            .ToListAsync();
            var penggunaDetails = await _context.Penggunas
            .Where(p => detailbuku.Contains(p.Id))
            .Select(p => new { Id = p.Id, Nama = p.Nama })
            .ToListAsync();
            ViewBag.Detail = penggunaDetails;
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Id", hakpaten.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Id", hakpaten.Inputby);
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            return View(hakpaten);
        }

        // GET: Hakpatens/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Hakpatens == null)
            {
                return NotFound();
            }
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
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
            var detailhakpaten = await _context.Detailhakpatens.Where(b => b.Idhakpaten == id).ToListAsync();

            if (detailhakpaten.Any() || hakpaten != null)
            {
                if (detailhakpaten.Any())
                {
                    _context.Detailhakpatens.RemoveRange(detailhakpaten);
                }
                if (hakpaten != null)
                {
                    _context.Hakpatens.Remove(hakpaten);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Data berhasil dihapus.";
            }
            else
            {
                TempData["ErrorMessage"] = "Tidak ada data yang dihapus.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool HakpatenExists(string id)
        {
          return (_context.Hakpatens?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
