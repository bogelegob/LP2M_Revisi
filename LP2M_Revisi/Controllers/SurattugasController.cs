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
using OfficeOpenXml;

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
            Console.WriteLine(surattuga.Editby);
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
                surattuga.Inputdate = tgl;

                surattuga.Status = 0;
                surattuga.Id = GenerateNextId();

                // Menangani pengguna yang dipilih

                string[] selectedIdsArray = SelectedUserIds.Split(',');

                foreach (var userId in selectedIdsArray)
                {
                    if (!string.IsNullOrEmpty(userId))
                    {
                        // Buat objek Detailbuku dan set nilainya
                        var detailsurat = new Detailsurat
                        {
                            Idsurat = surattuga.Id, // Sesuaikan dengan properti yang sesuai
                            Idpengguna = userId,
                        };

                        // Tambahkan objek Detailbuku ke konteks
                        _context.Detailsurats.Add(detailsurat);
                    }
                }
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
                TempData["SuccessMessage"] = "Data berhasil ditambahkan!";
                Console.WriteLine(TempData["SuccessMessage"]);
                return RedirectToAction("Index");
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
            
            // Pilihan pengguna untuk dropdown
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", surattuga.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", surattuga.Inputby);

            // Semua pengguna untuk pilihan many-to-many
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama", SelectedUserIds);
            return View(surattuga);
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
            var detailsurats = await _context.Detailsurats
            .Where(d => d.Idsurat == id)
            .Select(d => d.Idpengguna)
            .ToListAsync();
            var penggunaDetails = await _context.Penggunas
            .Where(p => detailsurats.Contains(p.Id))
            .Select(p => new { Id = p.Id, Nama = p.Nama })
            .ToListAsync();
            foreach (var pengguna in penggunaDetails)
            {
                Console.WriteLine($"ID: {pengguna.Id}, Nama: {pengguna.Nama}");
            }
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", surattuga.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", surattuga.Inputby);
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            ViewBag.Detail = penggunaDetails;
            return View(surattuga);
        }

        // POST: Surattugas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Namakegiatan,Masapelaksanaan,Buktipendukung,Status,Inputby,Inputdate,Editby,Editdate,Surattugas")] Surattuga surattuga, IFormFile Buktipendukung, string SelectedUserIds)
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
                    string[] selectedIdsArray = SelectedUserIds.Split(',');

                    var existingDetailsurats = await _context.Detailsurats
                        .Where(d => d.Idsurat == surattuga.Id)
                        .ToListAsync();

                    _context.Detailsurats.RemoveRange(existingDetailsurats);
                    await _context.SaveChangesAsync();

                    // Add new Detailsurat entries based on the selected user IDs
                    foreach (var userId in selectedIdsArray)
                    {
                        if (!string.IsNullOrEmpty(userId))
                        {
                            // Buat objek Detailbuku dan set nilainya
                            var detailsurat = new Detailsurat
                            {
                                Idsurat = surattuga.Id,
                                Idpengguna = userId,
                            };
                            // Tambahkan objek Detailbuku ke konteks
                            _context.Detailsurats.Add(detailsurat);
                        }
                    }
                    _context.Update(surattuga);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Data berhasil diedit.";
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
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Admin")
            {
                ViewBag.Layout = "_LayoutAdmin";
            }
            else
            {
                ViewBag.Layout = "_Layout";
            }
            var detailsurats = await _context.Detailsurats
            .Where(d => d.Idsurat == id)
            .Select(d => d.Idpengguna)
            .ToListAsync();
            var penggunaDetails = await _context.Penggunas
            .Where(p => detailsurats.Contains(p.Id))
            .Select(p => new { Id = p.Id, Nama = p.Nama })
            .ToListAsync();
            ViewBag.Detail = penggunaDetails;
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", surattuga.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", surattuga.Inputby);
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
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
            var surattuga = _context.Surattugas.FirstOrDefault(s => s.Namafile == fileName || s.Namafilesurat == fileName);

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

        public async Task<IActionResult> ExportToExcel(string searchString, string statusFilter)
        {
            IQueryable<Surattuga> suratTugasQuery = _context.Surattugas.Include(b => b.EditbyNavigation).Include(b => b.InputbyNavigation);
            Pengguna penggunaModel;
            string serializedModel = HttpContext.Session.GetString("Identity");
            penggunaModel = JsonConvert.DeserializeObject<Pengguna>(serializedModel);

            // Filter data sesuai pencarian
            if (!string.IsNullOrEmpty(searchString))
            {
                suratTugasQuery = suratTugasQuery.Where(b => b.Namakegiatan.Contains(searchString)
                                                         || b.Masapelaksanaan.Contains(searchString)
                                                         );
            }

            // Filter data sesuai status
            if (!string.IsNullOrEmpty(statusFilter))
            {
                suratTugasQuery = suratTugasQuery.Where(b => b.Status == Convert.ToInt32(statusFilter));
            }
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Karyawan")
            {
                suratTugasQuery = suratTugasQuery.Where(b => b.Inputby == penggunaModel.Id);
            }

            var suratTugasList = await suratTugasQuery.ToListAsync();

            // Create Excel package
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("SuratTugas");

                // Header
                var headerRange = worksheet.Cells["A1:J1"];
                headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells["A1"].Value = "NO";
                worksheet.Cells["B1"].Value = "Nama Kegiatan";
                worksheet.Cells["C1"].Value = "Masa Pelaksanaan";
                worksheet.Cells["D1"].Value = "Bukti Pendukung";
                worksheet.Cells["E1"].Value = "Status";
                worksheet.Cells["F1"].Value = "Tanggal Input";
                worksheet.Cells["G1"].Value = "Surat Tugas";
                worksheet.Cells["H1"].Value = "Input By";


                string status;
                int no = 1;
                // Data
                for (int i = 0; i < suratTugasList.Count; i++)
                {
                    
                    if (suratTugasList[i].Status == 0)
                    {
                        status = "Belum diajukan";
                    }
                    else if (suratTugasList[i].Status == 1)
                    {
                        status = "Menunggu Konfirmasi";
                    }
                    else if (suratTugasList[i].Status == 2)
                    {
                        status = "Diterima";
                    }
                    else if (suratTugasList[i].Status == 3)
                    {
                        status = "Ditolak";
                    }

                    if (suratTugasList[i].Namafilesurat == null)
                    {
                        status = "Belum ada surat tugas";
                    }

                    worksheet.Cells[i + 2, 1].Value = no;
                    worksheet.Cells[i + 2, 2].Value = suratTugasList[i].Namakegiatan;
                    worksheet.Cells[i + 2, 3].Value = suratTugasList[i].Masapelaksanaan;
                    worksheet.Cells[i + 2, 4].Value = suratTugasList[i].Namafile;
                    worksheet.Cells[i + 2, 5].Value = suratTugasList[i].Status;
                    worksheet.Cells[i + 2, 6].Value = suratTugasList[i].Inputdate;
                    worksheet.Cells[i + 2, 7].Value = suratTugasList[i].Namafilesurat;
                    worksheet.Cells[i + 2, 8].Value = suratTugasList[i].InputbyNavigation.Nama;

                    worksheet.Cells[i + 2, 1, i + 2, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    worksheet.Cells[i + 2, 6].Style.Numberformat.Format = "dd-MMM-yyyy HH:mm:ss";
                    no++;
                }

                // Auto fit columns
                worksheet.Cells.AutoFitColumns();

                // Convert to byte array
                var excelBytes = package.GetAsByteArray();

                // Set content type and file name
                Response.Headers.Add("Content-Disposition", "attachment; filename=SuratTugas.xlsx");
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
    }
}
