using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LP2M_Revisi.Models;
using Newtonsoft.Json;
using System.Data;

namespace LP2M_Revisi.Controllers
{
    public class SurattugasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SurattugasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Surattugas
        public async Task<IActionResult> Index()
        {
            Pengguna penggunaModel;
            string serializedModel = HttpContext.Session.GetString("Identity");
            
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
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
                var surattugas = await _context.Surattugas.Where(b => b.Inputby == penggunaModel.Id).Include(b => b.EditbyNavigation).Include(b => b.InputbyNavigation).ToListAsync();

                return View(surattugas);
            }
            else
            {
                ViewBag.Layout = "_Layout";
                var surattugas = await _context.Surattugas.Where(b => b.Inputby == penggunaModel.Id).Include(b => b.EditbyNavigation).Include(b => b.InputbyNavigation).ToListAsync();
                
                return View(surattugas);
            }
        }
        public async Task<IActionResult> Konfirmasi()
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
            var applicationDbContext = _context.Surattugas.Include(s => s.EditbyNavigation).Include(s => s.InputbyNavigation).Where(s => s.Status == 1 || s.Status == 2 && s.Surattugas == null);
            return View(await applicationDbContext.ToListAsync());
        }
        public string GenerateNextId()
        {
            // Cari ID terakhir dalam database
            var lastId = _context.Surattugas
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
            string nextId = $"STS{lastIdNumeric:D2}";

            return nextId;
        }
        // GET: Surattugas/Details/5
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
            if (id == null || _context.Surattugas == null)
            {
                return NotFound();
            }

            var surattuga = await _context.Surattugas
                .Include(s => s.EditbyNavigation)
                .Include(s => s.InputbyNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (surattuga == null)
            {
                return NotFound();
            }

            return View(surattuga);
        }

        // GET: Surattugas/Create
        public IActionResult Create()
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
            Surattuga suratTugas = new Surattuga();
            suratTugas.Id = GenerateNextId();
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            // Semua pengguna untuk pilihan many-to-many
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            return View(suratTugas);
        }

        // POST: Surattugas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Namakegiatan,Masapelaksanaan,Status,Inputby,Inputdate,Editby,Editdate,Surattugas")] Surattuga surattuga, IFormFile Buktipendukung, string SelectedUserIds)
        {
            if (ModelState.IsValid)
            {
                var serializedModel = HttpContext.Session.GetString("Identity");
                var pengguna = JsonConvert.DeserializeObject<Pengguna>(serializedModel);
                surattuga.Inputby = pengguna.Id;

                DateTime tgl = DateTime.Now;
                Console.WriteLine("Tanggallllllll : " + tgl);
                surattuga.Inputdate = tgl;

                surattuga.Status = 0;
                surattuga.Id = GenerateNextId();

                // Menangani pengguna yang dipilih
                string[] selectedIdsArray = SelectedUserIds.Split(',');

                /*foreach (var userId in selectedIdsArray)
                {
                    if (!string.IsNullOrEmpty(userId))
                    {
                        // Buat objek Detailbuku dan set nilainya
                        var detailBuku = new Detailsurattugas
                        {
                            Idsurattugas = surattuga.Id, // Sesuaikan dengan properti yang sesuai
                            Idpengguna = userId
                        };

                        // Tambahkan objek Detailbuku ke konteks
                        _context.Detailsurattugas.Add(detailBuku);
                    }
                }*/
                if (Buktipendukung != null && Buktipendukung.Length > 0)
                {
                    surattuga.Namafile = Buktipendukung.FileName;
                    using (var memoryStream = new MemoryStream())
                    {
                        await Buktipendukung.CopyToAsync(memoryStream);
                        // Menyimpan konten file sebagai byte array dalam properti Buktipendukung
                        surattuga.Buktipendukung = memoryStream.ToArray();
                    }
                }

                _context.Add(surattuga);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Data berhasil disimpan!";
                Console.WriteLine(TempData["SuccessMessage"]);
                return RedirectToAction("Index");
            }

            // Pilihan pengguna untuk dropdown
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", surattuga.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", surattuga.Inputby);

            // Semua pengguna untuk pilihan many-to-many
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            return View(surattuga);
        }

        // GET: Surattugas/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            Pengguna penggunaModel;
            string serializedModel = HttpContext.Session.GetString("Identity");
            string Role = HttpContext.Session.GetString("selectedRole");
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
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
            }
            if (id == null || _context.Surattugas == null)
            {
                return NotFound();
            }

            var surattuga = await _context.Surattugas.FindAsync(id);
            if (surattuga == null)
            {
                return NotFound();
            }
            if (surattuga.Status != 0)
            {
                TempData["ErrorMessage"] = "Data Sudah diajukan tidak bisa di Edit";
                // Jika status bukan 0, misalnya status != 0 mengarahkan kembali ke halaman Index
                return RedirectToAction(nameof(Index));
            }
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", surattuga.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", surattuga.Inputby);
            return View(surattuga);
        }

        // POST: Surattugas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Namakegiatan,Masapelaksanaan,Buktipendukung,Status,Inputby,Inputdate,Editby,Editdate,Surattugas")] Surattuga surattuga, IFormFile Buktipendukung)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var serializedModel = HttpContext.Session.GetString("Identity");
                    var pengguna = JsonConvert.DeserializeObject<Pengguna>(serializedModel);
                    
                    surattuga.Status = 0;
                    if (Buktipendukung != null && Buktipendukung.Length > 0)
                    {
                        surattuga.Namafile = Buktipendukung.FileName;
                        using (var memoryStream = new MemoryStream())
                        {
                            await Buktipendukung.CopyToAsync(memoryStream);
                            // Menyimpan konten file sebagai byte array dalam properti Buktipendukung
                            surattuga.Buktipendukung = memoryStream.ToArray();
                        }
                    }
                    _context.Update(surattuga);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurattugaExists(surattuga.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "Data berhasil diedit!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", surattuga.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", surattuga.Inputby);
            return View(surattuga);
        }

        // GET: Surattugas/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Surattugas == null)
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
            var surattuga = await _context.Surattugas
                .Include(s => s.EditbyNavigation)
                .Include(s => s.InputbyNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (surattuga == null)
            {
                return NotFound();
            }
            if (surattuga.Status != 0)
            {
                TempData["ErrorMessage"] = "Data Sudah diajukan tidak bisa di hapus";
                // Jika status bukan 0, misalnya status != 0 mengarahkan kembali ke halaman Index
                return RedirectToAction(nameof(Index));
            }
            return View(surattuga);
        }
        [HttpPost]
        public async Task<IActionResult> KirimConfirm(string id)
        {
            var response = new { success = false, message = "Gagal mengirim Surat Tugas." };

            if (id == null)
            {
                response = new { success = false, message = "Gagal mengirim Surat Tugas." };
                return Json(response);
            }

            var surattugas = await _context.Surattugas.FindAsync(id);
            if (surattugas == null)
            {
                response = new { success = false, message = "Gagal mengirim Surat Tugas." };
                return Json(response);
            }
            if (surattugas.Status != 0)
            {
                response = new { success = false, message = "Data Sudah diajukan tidak bisa di Kirim" };
                return Json(response);
            }

            // Mengubah status menjadi 'Menunggu Konfirmasi' (status 1)
            surattugas.Status = 1;

            _context.Surattugas.Update(surattugas);
            await _context.SaveChangesAsync();
            response = new { success = true, message = "Surat Tugas berhasil dikirim." };
            return Json(response);
            //return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> terima(string id)
        {
            var response = new { success = false, message = "Gagal Setujui Surat Tugas." };

            if (id == null)
            {
                response = new { success = false, message = "Gagal Setujui Surat Tugas." };
                return Json(response);
            }

            var surattugas = await _context.Surattugas.FindAsync(id);
            if (surattugas == null)
            {
                response = new { success = false, message = "Gagal Setujui Surat Tugas." };
                return Json(response);
            }

            var serializedModel = HttpContext.Session.GetString("Identity");
            var pengguna = JsonConvert.DeserializeObject<Pengguna>(serializedModel);
            surattugas.Editby = pengguna.Id;
            DateTime tgl = DateTime.Now;
            surattugas.Editdate = tgl;
            surattugas.Status = 2;
            _context.Surattugas.Update(surattugas);
            await _context.SaveChangesAsync();
            response = new { success = true, message = "Surat Tugas berhasil disetujui." };
            return Json(response);
        }
        [HttpPost]
        public async Task<IActionResult> Setuju(string id, IFormFile suratt)
        {
            var response = new { success = false, message = "Gagal Setujui Surat Tugas." };

            if (id == null)
            {
                response = new { success = false, message = "Gagal Setujui Surat Tugas." };
                return Json(response);
            }

            var surattugas = await _context.Surattugas.FindAsync(id);
            if (surattugas == null)
            {
                response = new { success = false, message = "Gagal Setujui Surat Tugas." };
                return Json(response);
            }

            if (suratt != null && suratt.Length > 0)
            {
                surattugas.Namafilesurat = suratt.FileName;
                using (var memoryStream = new MemoryStream())
                {
                    await suratt.CopyToAsync(memoryStream);
                    // Menyimpan konten file sebagai byte array dalam properti Buktipendukung
                    surattugas.Surattugas = memoryStream.ToArray();
                }
            }


            var serializedModel = HttpContext.Session.GetString("Identity");
            var pengguna = JsonConvert.DeserializeObject<Pengguna>(serializedModel);
            surattugas.Editby = pengguna.Id;
            // Mengubah status menjadi 'Diterima' (status 2)

            surattugas.tanggalselesai = DateTime.Now;
            _context.Surattugas.Update(surattugas);
            await _context.SaveChangesAsync();
            response = new { success = true, message = "Surat Tugas berhasil disetujui." };
            return Json(response);
            //return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult>Tolak(string id,string alasan)
        {
            var response = new { success = false, message = "Gagal Tolak Surat Tugas." };

            if (id == null)
            {
                response = new { success = false, message = "Gagal Tolak Surat Tugas." };
                return Json(response);
            }

            var surattugas = await _context.Surattugas.FindAsync(id);
            if (surattugas == null)
            {
                response = new { success = false, message = "Gagal Tolak Surat Tugas." };
                return Json(response);
            }

            var serializedModel = HttpContext.Session.GetString("Identity");
            var pengguna = JsonConvert.DeserializeObject<Pengguna>(serializedModel);
            surattugas.Editby = pengguna.Id;
            DateTime tgl = DateTime.Now;
            surattugas.Editdate = tgl;
            // Mengubah status menjadi 'Diterima' (status 2)
            surattugas.Status = 3;
            surattugas.Keterangan = alasan;

            _context.Surattugas.Update(surattugas);
            await _context.SaveChangesAsync();
            response = new { success = true, message = "Surat Tugas berhasil Tolak." };
            return Json(response);
            //return RedirectToAction(nameof(Index));
        }


        // POST: Surattugas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Surattugas == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Surattugas'  is null.");
            }
            var surattuga = await _context.Surattugas.FindAsync(id);
            if (surattuga != null)
            {
                _context.Surattugas.Remove(surattuga);
            }
            TempData["SuccessMessage"] = "Data berhasil dihapus!";
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool SurattugaExists(string id)
        {
          return (_context.Surattugas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public IActionResult Download(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return RedirectToAction("Index");
            }

            // Cari data Surattuga dari database berdasarkan nama file atau parameter yang sesuai.
            var surattuga = _context.Surattugas.FirstOrDefault(s => s.Namafile == fileName);

            if (surattuga != null && surattuga.Buktipendukung != null)
            {
                // Mengambil data file dari kolom "Buktipendukung" dalam model Surattuga
                byte[] fileBytes = surattuga.Buktipendukung;

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



    }
}
