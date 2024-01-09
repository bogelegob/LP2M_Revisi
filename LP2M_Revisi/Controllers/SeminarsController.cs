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
    public class SeminarsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SeminarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Seminars
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
            var applicationDbContext = _context.Seminars.Include(s => s.EditbyNavigation).Include(s => s.InputbyNavigation);
            return View(await applicationDbContext.ToListAsync());
        }
        public string GenerateNextId()
        {
            // Cari ID terakhir dalam database
            var lastId = _context.Seminars
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
            string nextId = $"PPS{lastIdNumeric:D2}";

            return nextId;
        }

        // GET: Seminars/Details/5
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
            if (id == null || _context.Seminars == null)
            {
                return NotFound();
            }

            var seminar = await _context.Seminars
                .Include(s => s.EditbyNavigation)
                .Include(s => s.InputbyNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seminar == null)
            {
                return NotFound();
            }

            return View(seminar);
        }

        // GET: Seminars/Create
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
            Seminar seminar = new Seminar();
            seminar.Id = GenerateNextId();
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            return View(seminar);
        }

        // POST: Seminars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Judulprogram,Judulpaper,Kategori,Penyelenggara,Waktupelaksanaan,Tempatpelaksanaan,Keterangan,Status,Inputby,Inputdate,Editby,Editdate")] Seminar seminar, string SelectedUserIds)
        {
            if (ModelState.IsValid)
            {
                var serializedModel = HttpContext.Session.GetString("Identity");
                var pengguna = JsonConvert.DeserializeObject<Pengguna>(serializedModel);
                seminar.Inputby = pengguna.Id;
                seminar.Editby = pengguna.Id;
                DateTime tgl = DateTime.Now;
                seminar.Inputdate = tgl;
                seminar.Editdate = tgl;
                seminar.Status = 1;
                seminar.Id = GenerateNextId();
                _context.Add(seminar);
                string[] selectedIdsArray = SelectedUserIds.Split(',');

                foreach (var userId in selectedIdsArray)
                {
                    if (!string.IsNullOrEmpty(userId))
                    {
                        // Buat objek Detailbuku dan set nilainya
                        var detailBuku = new Detailseminar
                        {
                            Idseminar = seminar.Id, // Sesuaikan dengan properti yang sesuai
                            Idpengguna = userId
                        };

                        // Tambahkan objek Detailbuku ke konteks
                        _context.Detailseminars.Add(detailBuku);
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", seminar.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", seminar.Inputby);
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            return View(seminar);
        }

        // GET: Seminars/Edit/5
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
            if (id == null || _context.Seminars == null)
            {
                return NotFound();
            }

            var seminar = await _context.Seminars.FindAsync(id);
            if (seminar == null)
            {
                return NotFound();
            }
            var detailbuku = await _context.Detailseminars
            .Where(d => d.Idseminar == id)
            .Select(d => d.Idpengguna)
            .ToListAsync();
            var penggunaDetails = await _context.Penggunas
            .Where(p => detailbuku.Contains(p.Id))
            .Select(p => new { Id = p.Id, Nama = p.Nama })
            .ToListAsync();
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", seminar.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", seminar.Inputby);
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            ViewBag.Detail = penggunaDetails;
            return View(seminar);
        }

        // POST: Seminars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Judulprogram,Judulpaper,Kategori,Penyelenggara,Waktupelaksanaan,Tempatpelaksanaan,Keterangan,Status,Inputby,Inputdate,Editby,Editdate")] Seminar seminar, string SelectedUserIds)
        {
            if (id != seminar.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string[] selectedIdsArray = SelectedUserIds.Split(',');

                    var existingDetailsurats = await _context.Detailseminars
                        .Where(d => d.Idseminar == seminar.Id)
                        .ToListAsync();

                    _context.Detailseminars.RemoveRange(existingDetailsurats);
                    await _context.SaveChangesAsync();

                    // Add new Detailsurat entries based on the selected user IDs
                    foreach (var userId in selectedIdsArray)
                    {
                        if (!string.IsNullOrEmpty(userId))
                        {
                            // Buat objek Detailbuku dan set nilainya
                            var detailsurat = new Detailseminar
                            {
                                Idseminar = seminar.Id,
                                Idpengguna = userId,
                            };
                            // Tambahkan objek Detailbuku ke konteks
                            _context.Detailseminars.Add(detailsurat);
                        }
                    }
                    _context.Update(seminar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SeminarExists(seminar.Id))
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
            var detailbuku = await _context.Detailseminars
            .Where(d => d.Idseminar == id)
            .Select(d => d.Idpengguna)
            .ToListAsync();
            var penggunaDetails = await _context.Penggunas
            .Where(p => detailbuku.Contains(p.Id))
            .Select(p => new { Id = p.Id, Nama = p.Nama })
            .ToListAsync();
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Id", seminar.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Id", seminar.Inputby);
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            ViewBag.Detail = penggunaDetails;
            return View(seminar);
        }

        // GET: Seminars/Delete/5
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
            if (id == null || _context.Seminars == null)
            {
                return NotFound();
            }

            var seminar = await _context.Seminars
                .Include(s => s.EditbyNavigation)
                .Include(s => s.InputbyNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (seminar == null)
            {
                return NotFound();
            }

            return View(seminar);
        }

        // POST: Seminars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Seminars == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Seminars'  is null.");
            }
            var seminar = await _context.Seminars.FindAsync(id);
            var detailseminar = await _context.Detailseminars.Where(b => b.Idseminar == id).ToListAsync();

            if (detailseminar.Any() || seminar != null)
            {
                if (detailseminar.Any())
                {
                    _context.Detailseminars.RemoveRange(detailseminar);
                }
                if (seminar != null)
                {
                    _context.Seminars.Remove(seminar);
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

        private bool SeminarExists(string id)
        {
          return (_context.Seminars?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
