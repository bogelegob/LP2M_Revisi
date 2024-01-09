using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LP2M_Revisi.Models;
using Newtonsoft.Json;
using OfficeOpenXml;

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
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
            }
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
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
            }
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
        public async Task<IActionResult> Create([Bind("Id,Judulbuku,Isbn,Penerbit,Tahun,Status,Inputby,Inputdate,Editby,Editdate")] Buku buku, string SelectedUserIds)
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
                _context.Add(buku);
                string[] selectedIdsArray = SelectedUserIds.Split(',');

                foreach (var userId in selectedIdsArray)
                {
                    if (!string.IsNullOrEmpty(userId))
                    {
                        // Buat objek Detailbuku dan set nilainya
                        var detailBuku = new Detailbuku
                        {
                            Idbuku = buku.Id, // Sesuaikan dengan properti yang sesuai
                            Idpengguna = userId,
                            Status = "Aktif" 
                        };

                        // Tambahkan objek Detailbuku ke konteks
                        _context.Detailbukus.Add(detailBuku);
                    }
                }
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Data berhasil ditambahkan.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", buku.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", buku.Inputby);
            return View(buku);
        }


        // GET: Bukus/Edit/5
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
            if (id == null || _context.Bukus == null)
            {
                return NotFound();
            }

            var buku = await _context.Bukus.FindAsync(id);
            if (buku == null)
            {
                return NotFound();
            }
            var detailbuku = await _context.Detailbukus
            .Where(d => d.Idbuku == id)
            .Select(d => d.Idpengguna)
            .ToListAsync();
            var penggunaDetails = await _context.Penggunas
            .Where(p => detailbuku.Contains(p.Id))
            .Select(p => new { Id = p.Id, Nama = p.Nama })
            .ToListAsync();
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", buku.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", buku.Inputby);
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            ViewBag.Detail = penggunaDetails;
            return View(buku);
        }

        // POST: Bukus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Judulbuku,Isbn,Penerbit,Tahun,Status,Inputby,Inputdate,Editby,Editdate")] Buku buku, string SelectedUserIds)
        {
            if (id != buku.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string[] selectedIdsArray = SelectedUserIds.Split(',');

                    var existingDetailsurats = await _context.Detailbukus
                        .Where(d => d.Idbuku == buku.Id)
                        .ToListAsync();

                    _context.Detailbukus.RemoveRange(existingDetailsurats);
                    await _context.SaveChangesAsync();

                    // Add new Detailsurat entries based on the selected user IDs
                    foreach (var userId in selectedIdsArray)
                    {
                        if (!string.IsNullOrEmpty(userId))
                        {
                            // Buat objek Detailbuku dan set nilainya
                            var detailsurat = new Detailbuku
                            {
                                Idbuku = buku.Id,
                                Idpengguna = userId,
                            };
                            // Tambahkan objek Detailbuku ke konteks
                            _context.Detailbukus.Add(detailsurat);
                        }
                    }
                    _context.Update(buku);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Data berhasil diedit.";
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
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
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
            var detailbuku = await _context.Detailbukus.Where(b => b.Idbuku == id).ToListAsync();
            var buku = await _context.Bukus.FindAsync(id);

            if (detailbuku.Any() || buku != null)
            {
                if (detailbuku.Any())
                {
                    _context.Detailbukus.RemoveRange(detailbuku);
                }
                if (buku != null)
                {
                    _context.Bukus.Remove(buku);
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

        private bool BukuExists(string id)
        {
          return (_context.Bukus?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> ExportToExcel(string searchString, string statusFilter)
        {
            IQueryable<Buku> publikasibuku = _context.Bukus.Include(b => b.EditbyNavigation).Include(b => b.InputbyNavigation);
            IQueryable<Detailbuku> detail = _context.Detailbukus.Include(b => b.IdpenggunaNavigation);
            Pengguna penggunaModel;
            string serializedModel = HttpContext.Session.GetString("Identity");
            penggunaModel = JsonConvert.DeserializeObject<Pengguna>(serializedModel);

            // Filter data sesuai pencarian
            if (!string.IsNullOrEmpty(searchString))
            {
                publikasibuku = publikasibuku.Where(b => b.Judulbuku.Contains(searchString)
                                                         || b.Penerbit.Contains(searchString)
                                                         );
            }
            // Filter data sesuai status
            if (!string.IsNullOrEmpty(statusFilter))
            {
                publikasibuku = publikasibuku.Where(b => b.Status == Convert.ToInt32(statusFilter));
            }

            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Karyawan")
            {
                publikasibuku = publikasibuku.Where(b => b.Inputby == penggunaModel.Id);
            }

            var publikasibukuList = await publikasibuku.ToListAsync();

            // Create Excel package
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Publikasi Buku");

                // Header
                var headerRange = worksheet.Cells["A1:J1"];
                headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells["A1"].Value = "NO";
                worksheet.Cells["B1"].Value = "Judul Buku";
                worksheet.Cells["C1"].Value = "ISBN";
                worksheet.Cells["D1"].Value = "Penerbit";
                worksheet.Cells["E1"].Value = "Tahun";
                worksheet.Cells["F1"].Value = "Tanggal Input";
                worksheet.Cells["G1"].Value = "Created By";
                worksheet.Cells["H1"].Value = "Penulis dan Editor";

                int no = 1;
                // Data
                for (int i = 0; i < publikasibukuList.Count; i++)
                {
                    if (publikasibukuList[i].Id != null)
                    {
                        detail = detail.Where(b => b.Idbuku == publikasibukuList[i].Id);
                    }
                    var detailList = await detail.ToListAsync();

                    worksheet.Cells[i + 2, 1].Value = no;
                    worksheet.Cells[i + 2, 2].Value = publikasibukuList[i].Judulbuku;
                    worksheet.Cells[i + 2, 3].Value = publikasibukuList[i].Isbn;
                    worksheet.Cells[i + 2, 4].Value = publikasibukuList[i].Penerbit;
                    worksheet.Cells[i + 2, 5].Value = publikasibukuList[i].Tahun;
                    worksheet.Cells[i + 2, 6].Value = publikasibukuList[i].Inputdate;
                    worksheet.Cells[i + 2, 7].Value = publikasibukuList[i].InputbyNavigation.Nama;
                    for (int z = 0; z < detailList.Count; z++)
                    {
                        Console.WriteLine("asfesfgwrgvreagsttasgrthgsw "+detailList[z].IdpenggunaNavigation.Nama);
                        worksheet.Cells[i + 2, 8].Value = worksheet.Cells[i + 2, 8].Value + detailList[z].IdpenggunaNavigation.Nama + ", ";
                    }
                    worksheet.Cells[i + 2, 1, i + 2, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells[i + 2, 6].Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                    no++;
                }

                // Auto fit columns
                worksheet.Cells.AutoFitColumns();

                // Convert to byte array
                var excelBytes = package.GetAsByteArray();

                // Set content type and file name
                Response.Headers.Add("Content-Disposition", "attachment; filename=Publikasi Buku.xlsx");
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
    }
}
