using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LP2M_Revisi.Models;
using Newtonsoft.Json;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Azure;

namespace LP2M_Revisi.Controllers
{
    public class PengaduanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PengaduanController(ApplicationDbContext context)
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
                var pengaduan = await _context.Pengaduan.Where(b => b.pengguna == penggunaModel.Id).Include(b => b.PenggunaNavigation).ToListAsync();
                Console.WriteLine(penggunaModel.Id);
                ViewBag.Pengguna = penggunaModel.Id;
                return View(pengaduan);
            }
            else
            {
                ViewBag.Layout = "_Layout";
                var pengaduan = await _context.Pengaduan.Where(b => b.pengguna == penggunaModel.Id).Include(b => b.PenggunaNavigation).ToListAsync();
                ViewBag.Pengguna = penggunaModel.Id;
                return View(pengaduan);
            }
        }

        public async Task<IActionResult> Konfirmasi()
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
            }
            else
            {
                ViewBag.Layout = "_Layout";
            }
            ViewBag.Pengguna = penggunaModel.Id;
            var applicationDbContext = _context.Pengaduan.Include(s => s.PenggunaNavigation).Where(s => s.Status == 1 || s.Status == 2);
            return View(await applicationDbContext.ToListAsync());
        }

        /*public string GenerateNextId()
        {
            // Cari ID terakhir dalam database
            var lastId = _context.Pengaduan
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
            string nextId = $"PGD{lastIdNumeric:D2}";

            return nextId;
        }*/
        // GET: pengaduan/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null || _context.Pengaduan == null)
            {
                return NotFound();
            }

            var pengaduan = await _context.Pengaduan
                .Include(s => s.PenggunaNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pengaduan == null)
            {
                return NotFound();
            }

            return View(pengaduan);
        }

        // GET: Surattugas/Create
        public IActionResult Create()
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
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
            }
            Pengaduan pengaduan = new Pengaduan();
            
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama");
            return View(pengaduan);
        }

        // POST: Surattugas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(string jenis ,string hakpaten, string hakcipta, string jurnal,string buku, string prosiding, string pengabdian, string seminar, string alasan)
        {
            Console.WriteLine("dafffwrfvesrgghdhrn" + jenis);
            Pengaduan pengaduan = new Pengaduan();
            if (ModelState.IsValid)
            {
                var serializedModel = HttpContext.Session.GetString("Identity");
                var pengguna = JsonConvert.DeserializeObject<Pengguna>(serializedModel);
                pengaduan.pengguna = pengguna.Id;

                DateTime tgl = DateTime.Now;
                pengaduan.createdate = tgl;
                pengaduan.Status = 0;
                pengaduan.Keterangan = alasan;

                if (jenis == "hakpaten")
                {
                    pengaduan.hakpaten = hakpaten;
                }
                if (jenis == "hakcipta")
                {
                    pengaduan.hakcipta = hakcipta;
                }
                if (jenis == "jurnal")
                {
                    pengaduan.jurnal = jurnal;
                }
                if (jenis == "buku")
                {
                    pengaduan.buku = buku;
                }
                if (jenis == "prosiding")
                {
                    pengaduan.prosiding = prosiding;
                }
                if (jenis == "pengabdian")
                {
                    pengaduan.pengabdian = pengabdian;
                }
                if (jenis == "seminar")
                {
                    pengaduan.seminar = seminar;
                }

                _context.Add(pengaduan);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Data berhasil disimpan!";
                return RedirectToAction("Index");
            }

            // Pilihan pengguna untuk dropdown
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", pengaduan.pengguna);
            var response = new { success = true, message = "Surat Tugas berhasil dikirim." };
            return Json(response);
        }

        // GET: Surattugas/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || _context.Pengaduan == null)
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

            var pengaduan = await _context.Pengaduan
                .Include(s => s.pengguna)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pengaduan == null)
            {
                return NotFound();
            }
            if (pengaduan.Status != 0)
            {
                TempData["ErrorMessage"] = "Data Sudah diajukan tidak bisa di hapus";
                // Jika status bukan 0, misalnya status != 0 mengarahkan kembali ke halaman Index
                return RedirectToAction(nameof(Index));
            }
            return View(pengaduan);
        }

        // POST: Surattugas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Pengaduan == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Pengaduan'  is null.");
            }
            var pengaduan = await _context.Pengaduan.FindAsync(id);
            if (pengaduan != null)
            {
                _context.Pengaduan.Remove(pengaduan);
            }
            TempData["SuccessMessage"] = "Data berhasil dihapus!";
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
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

            var pengaduan = await _context.Pengaduan.FindAsync(id);
            if (pengaduan == null)
            {
                response = new { success = false, message = "Gagal Setujui Surat Tugas." };
                return Json(response);
            }

            var serializedModel = HttpContext.Session.GetString("Identity");
            var pengguna = JsonConvert.DeserializeObject<Pengguna>(serializedModel);
            /*pengaduan.Editby = pengguna.Id;*/
            DateTime tgl = DateTime.Now;
            pengaduan.updatedate = tgl;
            pengaduan.Status = 2;
            _context.Pengaduan.Update(pengaduan);
            await _context.SaveChangesAsync();
            response = new { success = true, message = "Surat Tugas berhasil disetujui." };
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> Tolak(string id, string alasan)
        {
            var response = new { success = false, message = "Gagal Tolak Surat Tugas." };

            if (id == null)
            {
                response = new { success = false, message = "Gagal Tolak Surat Tugas." };
                return Json(response);
            }

            var pengaduan = await _context.Pengaduan.FindAsync(id);
            if (pengaduan == null)
            {
                response = new { success = false, message = "Gagal Tolak Surat Tugas." };
                return Json(response);
            }

            var serializedModel = HttpContext.Session.GetString("Identity");
            var pengguna = JsonConvert.DeserializeObject<Pengguna>(serializedModel);
            /*pengaduan.Editby = pengguna.Id;*/
            DateTime tgl = DateTime.Now;
            pengaduan.updatedate = tgl;
            // Mengubah status menjadi 'Diterima' (status 2)
            pengaduan.Status = 3;
            pengaduan.Keterangan = alasan;

            _context.Pengaduan.Update(pengaduan);
            await _context.SaveChangesAsync();
            response = new { success = true, message = "Surat Tugas berhasil Tolak." };
            return Json(response);
            //return RedirectToAction(nameof(Index));
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

            var pengaduan = await _context.Pengaduan.FindAsync(id);
            if (pengaduan == null)
            {
                response = new { success = false, message = "Gagal mengirim Surat Tugas." };
                return Json(response);
            }
            if (pengaduan.Status != 0)
            {
                response = new { success = false, message = "Data Sudah diajukan tidak bisa di Kirim" };
                return Json(response);
            }

            // Mengubah status menjadi 'Menunggu Konfirmasi' (status 1)
            pengaduan.Status = 1;

            _context.Pengaduan.Update(pengaduan);
            await _context.SaveChangesAsync();
            response = new { success = true, message = "Surat Tugas berhasil dikirim." };
            return Json(response);
            //return RedirectToAction(nameof(Index));
        }

        // GET: Surattugas/Edit/5
        public async Task<IActionResult> Edit(string id)
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
            if (id == null || _context.Pengaduan == null)
            {
                return NotFound();
            }

            var pengaduan = await _context.Pengaduan.FindAsync(id);
            if (pengaduan == null)
            {
                return NotFound();
            }
            if (pengaduan.Status != 0)
            {
                TempData["ErrorMessage"] = "Data Sudah diajukan tidak bisa di Edit";
                // Jika status bukan 0, misalnya status != 0 mengarahkan kembali ke halaman Index
                return RedirectToAction(nameof(Index));
            }
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", pengaduan.pengguna);
            return View(pengaduan);
        }

        // POST: Surattugas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Namakegiatan,Masapelaksanaan,Buktipendukung,Status,Inputby,Inputdate,Editby,Editdate,Surattugas")] Pengaduan pengaduan)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var serializedModel = HttpContext.Session.GetString("Identity");
                    var pengguna = JsonConvert.DeserializeObject<Pengguna>(serializedModel);

                    pengaduan.Status = 0;

                    _context.Update(pengaduan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
                TempData["SuccessMessage"] = "Data berhasil diedit!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", pengaduan.pengguna);
            return View(pengaduan);
        }


        [HttpGet]
        public async Task<IActionResult> GetData(string id, string pengaduan)
        {
            List<DataModel> data = new List<DataModel>();

            if (pengaduan == "buku")
            {
                var bukuData = await _context.Bukus
                    .Where(b => b.Inputby == id)
                    .ToListAsync();

                foreach (var buku in bukuData)
                {
                    var pengaduanEntry = new DataModel
                    {
                        Id = buku.Id,
                        Nama = buku.Judulbuku
                    };

                    data.Add(pengaduanEntry);
                }
            }
            else if (pengaduan == "jurnal")
            {
                var jurnalData = await _context.Jurnals
                    .Where(j => j.Inputby == id)
                    .ToListAsync();

                foreach (var buku in jurnalData)
                {
                    var pengaduanEntry = new DataModel
                    {
                        Id = buku.Id,
                        Nama = buku.Namajurnal
                    };

                    data.Add(pengaduanEntry);
                }
            }
            else if (pengaduan == "seminar")
            {
                var jurnalData = await _context.Seminars
                    .Where(j => j.Inputby == id)
                    .ToListAsync();

                foreach (var buku in jurnalData)
                {
                    var pengaduanEntry = new DataModel
                    {
                        Id = buku.Id,
                        Nama = buku.Judulprogram
                    };

                    data.Add(pengaduanEntry);
                }
            }
            else if (pengaduan == "pengabdian")
            {
                var jurnalData = await _context.Pengabdianmasyarakats
                    .Where(j => j.Inputby == id)
                    .ToListAsync();

                foreach (var buku in jurnalData)
                {
                    var pengaduanEntry = new DataModel
                    {
                        Id = buku.Id,
                        Nama = buku.Namakegiatan
                    };

                    data.Add(pengaduanEntry);
                }
            }
            else if (pengaduan == "hakcipta")
            {
                var jurnalData = await _context.Hakcipta
                    .Where(j => j.Inputby == id)
                    .ToListAsync();

                foreach (var buku in jurnalData)
                {
                    var pengaduanEntry = new DataModel
                    {
                        Id = buku.Id,
                        Nama = buku.Judul
                    };

                    data.Add(pengaduanEntry);
                }
            }
            else if (pengaduan == "hakpaten")
            {
                var jurnalData = await _context.Hakpatens
                    .Where(j => j.Inputby == id)
                    .ToListAsync();

                foreach (var buku in jurnalData)
                {
                    var pengaduanEntry = new DataModel
                    {
                        Id = buku.Id,
                        Nama = buku.Judul
                    };

                    data.Add(pengaduanEntry);
                }
            }
            else if (pengaduan == "prosiding")
            {
                var jurnalData = await _context.Prosidings
                    .Where(j => j.Inputby == id)
                    .ToListAsync();

                foreach (var buku in jurnalData)
                {
                    var pengaduanEntry = new DataModel
                    {
                        Id = buku.Id,
                        Nama = buku.Judulprogram
                    };

                    data.Add(pengaduanEntry);
                }
            }

            return Ok(data);
        }


    }
}
