﻿using System;
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
    public class JurnalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JurnalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Jurnals
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
            var applicationDbContext = _context.Jurnals.Include(j => j.EditbyNavigation).Include(j => j.InputbyNavigation);
            return View(await applicationDbContext.ToListAsync());
        }
        public string GenerateNextId()
        {
            // Cari ID terakhir dalam database
            var lastId = _context.Jurnals
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
            string nextId = $"PPJ{lastIdNumeric:D2}";

            return nextId;
        }
        // GET: Jurnals/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Jurnals == null)
            {
                return NotFound();
            }

            var jurnal = await _context.Jurnals
                .Include(j => j.EditbyNavigation)
                .Include(j => j.InputbyNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jurnal == null)
            {
                return NotFound();
            }

            return View(jurnal);
        }

        // GET: Jurnals/Create
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
            Jurnal jurnal = new Jurnal();
            jurnal.Id = GenerateNextId();
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            return View(jurnal);
        }

        // POST: Jurnals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Judulmakalah,Namajurnal,Issn,Volume,Nomor,Halamanawal,Halamanakhir,Url,Kategori,Status,Inputby,Inputdate,Editby,Editdate")] Jurnal jurnal, string SelectedUserIds)
        {
            if (ModelState.IsValid)
            {
                var serializedModel = HttpContext.Session.GetString("Identity");
                var pengguna = JsonConvert.DeserializeObject<Pengguna>(serializedModel);
                jurnal.Inputby = pengguna.Id;
                jurnal.Editby = pengguna.Id;
                DateTime tgl = DateTime.Now;
                jurnal.Inputdate = tgl;
                jurnal.Editdate = tgl;
                jurnal.Status = 0;
                jurnal.Id = GenerateNextId();
                string[] selectedIdsArray = SelectedUserIds.Split(',');

                foreach (var userId in selectedIdsArray)
                {
                    if (!string.IsNullOrEmpty(userId))
                    {
                        // Buat objek Detailbuku dan set nilainya
                        var detailjurnal = new Detailjurnal
                        {
                            Idjurnal = jurnal.Id, // Sesuaikan dengan properti yang sesuai
                            Idpengguna = userId,
                        };

                        // Tambahkan objek Detailbuku ke konteks
                        _context.Detailjurnals.Add(detailjurnal);
                    }
                }
                _context.Add(jurnal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", jurnal.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", jurnal.Inputby);
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            return View(jurnal);
        }

        // GET: Jurnals/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Jurnals == null)
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
            var jurnal = await _context.Jurnals.FindAsync(id);
            if (jurnal == null)
            {
                return NotFound();
            }
            var detailbuku = await _context.Detailjurnals
            .Where(d => d.Idjurnal == id)
            .Select(d => d.Idpengguna)
            .ToListAsync();
            var penggunaDetails = await _context.Penggunas
            .Where(p => detailbuku.Contains(p.Id))
            .Select(p => new { Id = p.Id, Nama = p.Nama })
            .ToListAsync();
            ViewBag.Detail = penggunaDetails;
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", jurnal.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", jurnal.Inputby);
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            return View(jurnal);
        }

        // POST: Jurnals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Judulmakalah,Namajurnal,Issn,Volume,Nomor,Halamanawal,Halamanakhir,Url,Kategori,Status,Inputby,Inputdate,Editby,Editdate")] Jurnal jurnal, string SelectedUserIds)
        {
            if (id != jurnal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string[] selectedIdsArray = SelectedUserIds.Split(',');

                    var existingDetailsurats = await _context.Detailjurnals
                        .Where(d => d.Idjurnal == jurnal.Id)
                        .ToListAsync();

                    _context.Detailjurnals.RemoveRange(existingDetailsurats);
                    await _context.SaveChangesAsync();

                    // Add new Detailsurat entries based on the selected user IDs
                    foreach (var userId in selectedIdsArray)
                    {
                        if (!string.IsNullOrEmpty(userId))
                        {
                            // Buat objek Detailbuku dan set nilainya
                            var detailsurat = new Detailjurnal
                            {
                                Idjurnal = jurnal.Id,
                                Idpengguna = userId,
                            };
                            // Tambahkan objek Detailbuku ke konteks
                            _context.Detailjurnals.Add(detailsurat);
                        }
                    }
                    _context.Update(jurnal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JurnalExists(jurnal.Id))
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
            var detailbuku = await _context.Detailjurnals
            .Where(d => d.Idjurnal == id)
            .Select(d => d.Idpengguna)
            .ToListAsync();
            var penggunaDetails = await _context.Penggunas
            .Where(p => detailbuku.Contains(p.Id))
            .Select(p => new { Id = p.Id, Nama = p.Nama })
            .ToListAsync();
            ViewBag.Detail = penggunaDetails;
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", jurnal.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", jurnal.Inputby);
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            return View(jurnal);
        }

        // GET: Jurnals/Delete/5
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
            if (id == null || _context.Jurnals == null)
            {
                return NotFound();
            }

            var jurnal = await _context.Jurnals
                .Include(j => j.EditbyNavigation)
                .Include(j => j.InputbyNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jurnal == null)
            {
                return NotFound();
            }

            return View(jurnal);
        }

        // POST: Jurnals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Jurnals == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Jurnals'  is null.");
            }
            var jurnal = await _context.Jurnals.FindAsync(id);
            var detailjurnal = await _context.Detailjurnals.Where(b => b.Idjurnal == id).ToListAsync();

            if (detailjurnal.Any() || jurnal != null)
            {
                if (detailjurnal.Any())
                {
                    _context.Detailjurnals.RemoveRange(detailjurnal);
                }
                if (jurnal != null)
                {
                    _context.Jurnals.Remove(jurnal);
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

        private bool JurnalExists(string id)
        {
          return (_context.Jurnals?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
