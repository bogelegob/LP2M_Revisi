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
    public class PengabdianmasyarakatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PengabdianmasyarakatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pengabdianmasyarakats
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
            var applicationDbContext = _context.Pengabdianmasyarakats.Include(p => p.EditbyNavigation).Include(p => p.InputbyNavigation);
            return View(await applicationDbContext.ToListAsync());
        }
        public string GenerateNextId()
        {
            // Cari ID terakhir dalam database
            var lastId = _context.Pengabdianmasyarakats
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
            string nextId = $"PPM{lastIdNumeric:D2}";

            return nextId;
        }

        // GET: Pengabdianmasyarakats/Details/5
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
            if (id == null || _context.Pengabdianmasyarakats == null)
            {
                return NotFound();
            }

            var pengabdianmasyarakat = await _context.Pengabdianmasyarakats
                .Include(p => p.EditbyNavigation)
                .Include(p => p.InputbyNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pengabdianmasyarakat == null)
            {
                return NotFound();
            }
            ViewBag.ProdiList = _context.Prodis.ToDictionary(p => p.Id, p => p.Nama);
            return View(pengabdianmasyarakat);
        }

        // GET: Pengabdianmasyarakats/Create
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
            Pengabdianmasyarakat pengabdi = new Pengabdianmasyarakat();
            pengabdi.Id = GenerateNextId();
            ViewData["Prodi"] = new SelectList(_context.Prodis, "Id", "Nama");
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            return View(pengabdi);
        }

        // POST: Pengabdianmasyarakats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Namakegiatan,Waktupelaksanaan,Jumlahpenerima,Surattugas,Laporan,Buktipendukung,MahasiswaProdiNim,Status,Inputby,Inputdate,Editby,Editdate")] Pengabdianmasyarakat pengabdianmasyarakat, IFormFile Surattugas, IFormFile Laporan, IFormFile Buktipendukung)
        {
            Console.WriteLine(pengabdianmasyarakat.MahasiswaProdiNim);
            if (ModelState.IsValid)
            {
                var serializedModel = HttpContext.Session.GetString("Identity");
                var pengguna = JsonConvert.DeserializeObject<Pengguna>(serializedModel);
                pengabdianmasyarakat.Inputby = pengguna.Id;
                pengabdianmasyarakat.Editby = pengguna.Id;
                pengabdianmasyarakat.Inputdate = DateTime.Now;
                pengabdianmasyarakat.Editdate = DateTime.Now;
                pengabdianmasyarakat.Id = GenerateNextId();
                pengabdianmasyarakat.Status = 0;
                if (Surattugas != null && Surattugas.Length > 0)
                {
                    pengabdianmasyarakat.Namafilesurat = Surattugas.FileName;
                    using (var memoryStream = new MemoryStream())
                    {
                        await Surattugas.CopyToAsync(memoryStream);
                        pengabdianmasyarakat.Surattugas = memoryStream.ToArray();
                    }
                }
                if (Laporan != null && Laporan.Length > 0)
                {
                    pengabdianmasyarakat.Namafilelaporan = Laporan.FileName;
                    using (var memoryStream = new MemoryStream())
                    {
                        await Laporan.CopyToAsync(memoryStream);
                        pengabdianmasyarakat.Laporan = memoryStream.ToArray();
                    }
                }
                if (Buktipendukung != null && Buktipendukung.Length > 0)
                {
                    pengabdianmasyarakat.Namafilebukti = Buktipendukung.FileName;
                    using (var memoryStream = new MemoryStream())
                    {
                        await Buktipendukung.CopyToAsync(memoryStream);
                        pengabdianmasyarakat.Buktipendukung = memoryStream.ToArray();
                    }
                }
                _context.Add(pengabdianmasyarakat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Prodi"] = new SelectList(_context.Prodis, "Id", "Nama");
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", pengabdianmasyarakat.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", pengabdianmasyarakat.Inputby);
            return View(pengabdianmasyarakat);
        }

        // GET: Pengabdianmasyarakats/Edit/5
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
            if (id == null || _context.Pengabdianmasyarakats == null)
            {
                return NotFound();
            }

            var pengabdianmasyarakat = await _context.Pengabdianmasyarakats.FindAsync(id);
            if (pengabdianmasyarakat == null)
            {
                return NotFound();
            }
            string[] splittedData = pengabdianmasyarakat.MahasiswaProdiNim.Split('|');

            // Masukkan masing-masing data ke ViewBag
            ViewBag.Mahasiswa = splittedData.Length > 0 ? splittedData[0] : "";
            ViewBag.Nim = splittedData.Length > 1 ? splittedData[1] : "";
            ViewBag.Prodi = splittedData.Length > 2 ? splittedData[2] : "";
            ViewData["Prodi"] = new SelectList(_context.Prodis, "Id", "Nama", splittedData[2]);
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", pengabdianmasyarakat.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", pengabdianmasyarakat.Inputby);
            return View(pengabdianmasyarakat);
        }

        // POST: Pengabdianmasyarakats/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Namakegiatan,Waktupelaksanaan,Jumlahpenerima,Surattugas,Laporan,Buktipendukung,MahasiswaProdiNim,Status,Inputby,Inputdate,Editby,Editdate")] Pengabdianmasyarakat pengabdianmasyarakat, IFormFile Surattugas, IFormFile Laporan, IFormFile Buktipendukung)
        {
            if (id != pengabdianmasyarakat.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var serializedModel = HttpContext.Session.GetString("Identity");
                    var pengguna = JsonConvert.DeserializeObject<Pengguna>(serializedModel);
                    pengabdianmasyarakat.Editby = pengguna.Id;
                    pengabdianmasyarakat.Editdate = DateTime.Now;
                    pengabdianmasyarakat.Status = 0;
                    if (Surattugas != null && Surattugas.Length > 0)
                    {
                        pengabdianmasyarakat.Namafilesurat = Surattugas.FileName;
                        using (var memoryStream = new MemoryStream())
                        {
                            await Surattugas.CopyToAsync(memoryStream);
                            pengabdianmasyarakat.Surattugas = memoryStream.ToArray();
                        }
                    }
                    if (Laporan != null && Laporan.Length > 0)
                    {
                        pengabdianmasyarakat.Namafilelaporan = Laporan.FileName;
                        using (var memoryStream = new MemoryStream())
                        {
                            await Laporan.CopyToAsync(memoryStream);
                            pengabdianmasyarakat.Laporan = memoryStream.ToArray();
                        }
                    }
                    if (Buktipendukung != null && Buktipendukung.Length > 0)
                    {
                        pengabdianmasyarakat.Namafilebukti = Buktipendukung.FileName;
                        using (var memoryStream = new MemoryStream())
                        {
                            await Buktipendukung.CopyToAsync(memoryStream);
                            pengabdianmasyarakat.Buktipendukung = memoryStream.ToArray();
                        }
                    }
                    _context.Update(pengabdianmasyarakat);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PengabdianmasyarakatExists(pengabdianmasyarakat.Id))
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
            string[] splittedData = pengabdianmasyarakat.MahasiswaProdiNim.Split('|');
            ViewBag.Mahasiswa = splittedData.Length > 0 ? splittedData[0] : "";
            ViewBag.Nim = splittedData.Length > 1 ? splittedData[1] : "";
            ViewBag.Prodi = splittedData.Length > 2 ? splittedData[2] : "";
            ViewData["Prodi"] = new SelectList(_context.Prodis, "Id", "Nama", splittedData[2]);
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", pengabdianmasyarakat.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", pengabdianmasyarakat.Inputby);
            return View(pengabdianmasyarakat);
        }

        // GET: Pengabdianmasyarakats/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Pengabdianmasyarakats == null)
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
            var pengabdianmasyarakat = await _context.Pengabdianmasyarakats
                .Include(p => p.EditbyNavigation)
                .Include(p => p.InputbyNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pengabdianmasyarakat == null)
            {
                return NotFound();
            }

            return View(pengabdianmasyarakat);
        }

        // POST: Pengabdianmasyarakats/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Pengabdianmasyarakats == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Pengabdianmasyarakats'  is null.");
            }
            var pengabdianmasyarakat = await _context.Pengabdianmasyarakats.FindAsync(id);
            if (pengabdianmasyarakat != null)
            {
                _context.Pengabdianmasyarakats.Remove(pengabdianmasyarakat);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return RedirectToAction("Index");
            }

            // Cari data Surattuga dari database berdasarkan nama file atau parameter yang sesuai.
            var surattuga = _context.Pengabdianmasyarakats.FirstOrDefault(s => s.Namafilesurat == fileName);
            var lapor = _context.Pengabdianmasyarakats.FirstOrDefault(s => s.Namafilelaporan == fileName);
            var bukti = _context.Pengabdianmasyarakats.FirstOrDefault(s => s.Namafilebukti == fileName);

            if (surattuga != null && surattuga.Surattugas != null)
            {
                // Mengambil data file dari kolom "Surattugas" dalam model Surattuga
                byte[] fileBytes = surattuga.Surattugas;

                // Mengirimkan data file kepada pengguna
                return File(fileBytes, "application/octet-stream", fileName);
            }
            else if (lapor != null && lapor.Laporan != null)
            {
                // Mengambil data file dari kolom "Surattugas" dalam model Surattuga
                byte[] fileBytes = lapor.Laporan;

                // Mengirimkan data file kepada pengguna
                return File(fileBytes, "application/octet-stream", fileName);
            }
            else if (bukti != null && bukti.Buktipendukung != null)
            {
                // Mengambil data file dari kolom "Surattugas" dalam model Surattuga
                byte[] fileBytes = bukti.Buktipendukung;

                // Mengirimkan data file kepada pengguna
                return File(fileBytes, "application/octet-stream", fileName);
            }
            else
            {
                // Handle jika data file tidak ditemukan
                // Misalnya, tampilkan pesan kesalahan atau alihkan ke halaman lain
                return RedirectToAction("Index");
            }
        }

        private bool PengabdianmasyarakatExists(string id)
        {
          return (_context.Pengabdianmasyarakats?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
