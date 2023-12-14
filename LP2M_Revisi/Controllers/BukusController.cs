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
    public class BukusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BukusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bukus
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
            var applicationDbContext = _context.Bukus.Include(b => b.EditbyNavigation).Include(b => b.InputbyNavigation);
            return View(await applicationDbContext.ToListAsync());
        }
        public string GenerateNextId()
        {
            // Cari ID terakhir dalam database
            var lastId = _context.Bukus
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
            string nextId = $"PPB{lastIdNumeric:D2}";

            return nextId;
        }

        // GET: Bukus/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Bukus == null)
            {
                return NotFound();
            }

            var buku = await _context.Bukus
                .Include(b => b.EditbyNavigation)
                .Include(b => b.InputbyNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (buku == null)
            {
                return NotFound();
            }

            return View(buku);
        }

        // GET: Bukus/Create
        public IActionResult Create()
        {
            Buku buku = new Buku();
            buku.Id = GenerateNextId();
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            return View(buku);
        }

        // POST: Bukus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Judulbuku,Isbn,Penerbit,Tahun,Status,Inputby,Inputdate,Editby,Editdate")] Buku buku, List<string> selectedUserIds)
        {
            if (ModelState.IsValid)
            {
                var serializedModel = HttpContext.Session.GetString("Identity");
                var pengguna = JsonConvert.DeserializeObject<Pengguna>(serializedModel);
                buku.Inputby = pengguna.Id;
                buku.Editby = pengguna.Id;
                DateTime tgl = DateTime.Now;
                buku.Inputdate = tgl;
                buku.Editdate = tgl;
                buku.Status = 0;
                buku.Id = GenerateNextId();
                _context.Add(buku);

                // Buat entitas Detailbuku untuk setiap pengguna yang dipilih
                if (selectedUserIds != null)
                {
                    foreach (var userId in selectedUserIds)
                    {
                        var detailBuku = new Detailbuku
                        {
                            Idbuku = buku.Id,
                            Idpengguna = userId,
                            Status = "Aktif" // Atur status atau properti lain jika diperlukan
                        };

                        _context.Add(detailBuku);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", buku.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", buku.Inputby);
            return View(buku);
        }


        // GET: Bukus/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Bukus == null)
            {
                return NotFound();
            }

            var buku = await _context.Bukus.FindAsync(id);
            if (buku == null)
            {
                return NotFound();
            }
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", buku.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", buku.Inputby);
            return View(buku);
        }

        // POST: Bukus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Judulbuku,Isbn,Penerbit,Tahun,Status,Inputby,Inputdate,Editby,Editdate")] Buku buku)
        {
            if (id != buku.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(buku);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BukuExists(buku.Id))
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
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Id", buku.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Id", buku.Inputby);
            return View(buku);
        }

        // GET: Bukus/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Bukus == null)
            {
                return NotFound();
            }

            var buku = await _context.Bukus
                .Include(b => b.EditbyNavigation)
                .Include(b => b.InputbyNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (buku == null)
            {
                return NotFound();
            }

            return View(buku);
        }

        // POST: Bukus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Bukus == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Bukus'  is null.");
            }
            var buku = await _context.Bukus.FindAsync(id);
            if (buku != null)
            {
                _context.Bukus.Remove(buku);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BukuExists(string id)
        {
          return (_context.Bukus?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
