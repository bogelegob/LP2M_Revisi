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
            var applicationDbContext = _context.Hakcipta.Include(h => h.EditbyNavigation).Include(h => h.InputbyNavigation);
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
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
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
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
            }
            Hakciptum hakciptum = new Hakciptum();
            hakciptum.Id = GenerateNextId();
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            return View(hakciptum);
        }

        // POST: Hakciptums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Judul,Noaplikasi,Nosertifikat,Keterangan,Status,Inputby,Inputdate,Editby,Editdate")] Hakciptum hakciptum, string SelectedUserIds)
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
                string[] selectedIdsArray = SelectedUserIds.Split(',');

                foreach (var userId in selectedIdsArray)
                {
                    if (!string.IsNullOrEmpty(userId))
                    {
                        // Buat objek Detailbuku dan set nilainya
                        var detailhakcipta = new Detailhakcipta
                        {
                            Idhakcipta = hakciptum.Id, // Sesuaikan dengan properti yang sesuai
                            Idpengguna = userId
                        };

                        // Tambahkan objek Detailbuku ke konteks
                        _context.Detailhakciptas.Add(detailhakcipta);
                    }
                }
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Data berhasil ditambahkan.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
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
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
            }
            var hakciptum = await _context.Hakcipta.FindAsync(id);
            if (hakciptum == null)
            {
                return NotFound();
            }
            var detailbuku = await _context.Detailhakciptas
            .Where(d => d.Idhakcipta == id)
            .Select(d => d.Idpengguna)
            .ToListAsync();
            var penggunaDetails = await _context.Penggunas
            .Where(p => detailbuku.Contains(p.Id))
            .Select(p => new { Id = p.Id, Nama = p.Nama })
            .ToListAsync();
            ViewBag.Detail = penggunaDetails;
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Id", hakciptum.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Id", hakciptum.Inputby);
            return View(hakciptum);
        }

        // POST: Hakciptums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Judul,Noaplikasi,Nosertifikat,Keterangan,Status,Inputby,Inputdate,Editby,Editdate")] Hakciptum hakciptum, string SelectedUserIds)
        {
            if (id != hakciptum.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string[] selectedIdsArray = SelectedUserIds.Split(',');

                    var existingDetailsurats = await _context.Detailhakciptas
                        .Where(d => d.Idhakcipta == hakciptum.Id)
                        .ToListAsync();

                    _context.Detailhakciptas.RemoveRange(existingDetailsurats);
                    await _context.SaveChangesAsync();

                    // Add new Detailsurat entries based on the selected user IDs
                    foreach (var userId in selectedIdsArray)
                    {
                        if (!string.IsNullOrEmpty(userId))
                        {
                            // Buat objek Detailbuku dan set nilainya
                            var detailsurat = new Detailhakcipta
                            {
                                Idhakcipta = hakciptum.Id,
                                Idpengguna = userId,
                            };
                            // Tambahkan objek Detailbuku ke konteks
                            _context.Detailhakciptas.Add(detailsurat);
                        }
                    }
                    _context.Update(hakciptum);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Data berhasil diedit.";
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
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
            }
            var detailbuku = await _context.Detailhakciptas
            .Where(d => d.Idhakcipta == id)
            .Select(d => d.Idpengguna)
            .ToListAsync();
            var penggunaDetails = await _context.Penggunas
            .Where(p => detailbuku.Contains(p.Id))
            .Select(p => new { Id = p.Id, Nama = p.Nama })
            .ToListAsync();
            ViewBag.Detail = penggunaDetails;
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
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
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
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
            var detailhakciptum = await _context.Detailhakciptas.Where(b => b.Idhakcipta == id).ToListAsync();

            if (detailhakciptum.Any() || hakciptum != null)
            {
                if (detailhakciptum.Any())
                {
                    _context.Detailhakciptas.RemoveRange(detailhakciptum);
                }
                if (hakciptum != null)
                {
                    _context.Hakcipta.Remove(hakciptum);
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

        private bool HakciptumExists(string id)
        {
          return (_context.Hakcipta?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
