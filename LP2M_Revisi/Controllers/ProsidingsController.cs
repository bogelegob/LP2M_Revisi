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
    public class ProsidingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProsidingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Prosidings
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
            var applicationDbContext = _context.Prosidings.Include(p => p.EditbyNavigation).Include(p => p.InputbyNavigation);
            return View(await applicationDbContext.ToListAsync());
        }
        public string GenerateNextId()
        {
            // Cari ID terakhir dalam database
            var lastId = _context.Prosidings
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
            string nextId = $"PPP{lastIdNumeric:D2}";

            return nextId;
        }

        // GET: Prosidings/Details/5
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
            if (id == null || _context.Prosidings == null)
            {
                return NotFound();
            }

            var prosiding = await _context.Prosidings
                .Include(p => p.EditbyNavigation)
                .Include(p => p.InputbyNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prosiding == null)
            {
                return NotFound();
            }

            return View(prosiding);
        }

        // GET: Prosidings/Create
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
            Prosiding prosiding = new Prosiding();
            prosiding.Id = GenerateNextId();
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            return View(prosiding);
        }

        // POST: Prosidings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Judulprogram,Judulpaper,Kategori,Penyelenggara,Waktuterbit,Tempatpelaksanaan,Keterangan,Status,Inputby,Inputdate,Editby,Editdate")] Prosiding prosiding, string SelectedUserIds)
        {
            if (ModelState.IsValid)
            {
                var serializedModel = HttpContext.Session.GetString("Identity");
                var pengguna = JsonConvert.DeserializeObject<Pengguna>(serializedModel);
                prosiding.Inputby = pengguna.Id;
                prosiding.Editby = pengguna.Id;
                DateTime tgl = DateTime.Now;
                prosiding.Inputdate = tgl;
                prosiding.Editdate = tgl;
                prosiding.Status = 0;
                prosiding.Id = GenerateNextId();
                string[] selectedIdsArray = SelectedUserIds.Split(',');

                foreach (var userId in selectedIdsArray)
                {
                    if (!string.IsNullOrEmpty(userId))
                    {
                        // Buat objek Detailbuku dan set nilainya
                        var detailprosiding = new Detailprosiding
                        {
                            Idprosiding = prosiding.Id, // Sesuaikan dengan properti yang sesuai
                            Idpengguna = userId,
                        };

                        // Tambahkan objek Detailbuku ke konteks
                        _context.Detailprosidings.Add(detailprosiding);
                    }
                }
                _context.Add(prosiding);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Data berhasil ditambahkan.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", prosiding.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", prosiding.Inputby);
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            return View(prosiding);
        }

        // GET: Prosidings/Edit/5
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
            if (id == null || _context.Prosidings == null)
            {
                return NotFound();
            }

            var prosiding = await _context.Prosidings.FindAsync(id);
            if (prosiding == null)
            {
                return NotFound();
            }
            var detailbuku = await _context.Detailprosidings
            .Where(d => d.Idprosiding == id)
            .Select(d => d.Idpengguna)
            .ToListAsync();
            var penggunaDetails = await _context.Penggunas
            .Where(p => detailbuku.Contains(p.Id))
            .Select(p => new { Id = p.Id, Nama = p.Nama })
            .ToListAsync();
            ViewBag.Detail = penggunaDetails;
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Id", prosiding.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Id", prosiding.Inputby);
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            return View(prosiding);
        }

        // POST: Prosidings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Judulprogram,Judulpaper,Kategori,Penyelenggara,Waktuterbit,Tempatpelaksanaan,Keterangan,Status,Inputby,Inputdate,Editby,Editdate")] Prosiding prosiding, string SelectedUserIds)
        {
            if (id != prosiding.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string[] selectedIdsArray = SelectedUserIds.Split(',');

                    var existingDetailsurats = await _context.Detailprosidings
                        .Where(d => d.Idprosiding == prosiding.Id)
                        .ToListAsync();

                    _context.Detailprosidings.RemoveRange(existingDetailsurats);
                    await _context.SaveChangesAsync();

                    // Add new Detailsurat entries based on the selected user IDs
                    foreach (var userId in selectedIdsArray)
                    {
                        if (!string.IsNullOrEmpty(userId))
                        {
                            // Buat objek Detailbuku dan set nilainya
                            var detailsurat = new Detailprosiding
                            {
                                Idprosiding = prosiding.Id,
                                Idpengguna = userId,
                            };
                            // Tambahkan objek Detailbuku ke konteks
                            _context.Detailprosidings.Add(detailsurat);
                        }
                    }
                    _context.Update(prosiding);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Data berhasil diedit.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProsidingExists(prosiding.Id))
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
            var detailbuku = await _context.Detailprosidings
            .Where(d => d.Idprosiding == id)
            .Select(d => d.Idpengguna)
            .ToListAsync();
            var penggunaDetails = await _context.Penggunas
            .Where(p => detailbuku.Contains(p.Id))
            .Select(p => new { Id = p.Id, Nama = p.Nama })
            .ToListAsync();
            ViewBag.Detail = penggunaDetails;
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Id", prosiding.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Id", prosiding.Inputby);
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            return View(prosiding);
        }

        // GET: Prosidings/Delete/5
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
            if (id == null || _context.Prosidings == null)
            {
                return NotFound();
            }

            var prosiding = await _context.Prosidings
                .Include(p => p.EditbyNavigation)
                .Include(p => p.InputbyNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (prosiding == null)
            {
                return NotFound();
            }

            return View(prosiding);
        }

        // POST: Prosidings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Prosidings == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Prosidings'  is null.");
            }
            var prosiding = await _context.Prosidings.FindAsync(id);
            var detailprosiding = await _context.Detailprosidings.Where(b => b.Idprosiding == id).ToListAsync();

            if (detailprosiding.Any() || prosiding != null)
            {
                if (detailprosiding.Any())
                {
                    _context.Detailprosidings.RemoveRange(detailprosiding);
                }
                if (prosiding != null)
                {
                    _context.Prosidings.Remove(prosiding);
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

        private bool ProsidingExists(string id)
        {
          return (_context.Prosidings?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
