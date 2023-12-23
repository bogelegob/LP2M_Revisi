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
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
            return View(pengabdi);
        }

        // POST: Pengabdianmasyarakats/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Namakegiatan,Waktupelaksanaan,Jumlahpenerima,Surattugas,Laporan,Buktipendukung,MahasiswaProdiNim,Status,Inputby,Inputdate,Editby,Editdate")] Pengabdianmasyarakat pengabdianmasyarakat, IFormFile Surattugas, IFormFile Laporan, IFormFile Buktipendukung, string SelectedUserIds)
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
                string[] selectedIdsArray = SelectedUserIds.Split(',');

                foreach (var userId in selectedIdsArray)
                {
                    if (!string.IsNullOrEmpty(userId))
                    {
                        // Buat objek Detailbuku dan set nilainya
                        var detailpengabdian = new Detailpengabdian
                        {
                            Idpengabdian = pengabdianmasyarakat.Id, // Sesuaikan dengan properti yang sesuai
                            Idpengguna = userId,
                        };

                        // Tambahkan objek Detailbuku ke konteks
                        _context.Detailpengabdians.Add(detailpengabdian);
                    }
                }
                _context.Add(pengabdianmasyarakat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Prodi"] = new SelectList(_context.Prodis, "Id", "Nama");
            ViewData["Editby"] = new SelectList(_context.Penggunas, "Id", "Nama", pengabdianmasyarakat.Editby);
            ViewData["Inputby"] = new SelectList(_context.Penggunas, "Id", "Nama", pengabdianmasyarakat.Inputby);
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
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
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
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
            ViewData["ListPengguna"] = new MultiSelectList(_context.Penggunas, "Id", "Nama");
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

        public async Task<IActionResult> ExportToExcel(string searchString, string statusFilter)
        {
            IQueryable<Pengabdianmasyarakat> pengabdianquery = _context.Pengabdianmasyarakats.Include(b => b.EditbyNavigation).Include(b => b.InputbyNavigation);
            Pengguna penggunaModel;
            string serializedModel = HttpContext.Session.GetString("Identity");
            penggunaModel = JsonConvert.DeserializeObject<Pengguna>(serializedModel);

            // Filter data sesuai pencarian
            if (!string.IsNullOrEmpty(searchString))
            {
                //suratTugasQuery = suratTugasQuery.Where();
            }

            // Filter data sesuai status
            if (!string.IsNullOrEmpty(statusFilter))
            {
                pengabdianquery = pengabdianquery.Where(b => b.Status == Convert.ToInt32(statusFilter));
            }
            string Role = HttpContext.Session.GetString("selectedRole");
            if (Role == "Karyawan")
            {
                pengabdianquery = pengabdianquery.Where(b => b.Inputby == penggunaModel.Id);
            }

            var suratTugasList = await pengabdianquery.ToListAsync();

            // Create Excel package
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Pengabdian Masyarakat");

                // Header
                var headerRange = worksheet.Cells["A1:P1"];
                headerRange.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells["A1"].Value = "No";
                worksheet.Cells["B1"].Value = "Kegiatan";
                worksheet.Cells["C1"].Value = "Waktu Pelaksanaan";
                worksheet.Cells["D1"].Value = "Personil";
                worksheet.Cells["E1"].Value = "TPM";
                worksheet.Cells["F1"].Value = "MI";
                worksheet.Cells["G1"].Value = "MK";
                worksheet.Cells["H1"].Value = "MO/TAB";
                worksheet.Cells["I1"].Value = "P4";
                worksheet.Cells["J1"].Value = "TRL";
                worksheet.Cells["K1"].Value = "TKBG";
                worksheet.Cells["L1"].Value = "Jumlah Penerima Manfaat";
                worksheet.Cells["M1"].Value = "Asal Penerima";
                worksheet.Cells["N1"].Value = "Nama Mahasiswa";
                worksheet.Cells["O1"].Value = "Prodi";
                worksheet.Cells["P1"].Value = "NIM";


                string status;
                int no = 1;
                // Data
                for (int i = 0; i < suratTugasList.Count; i++)
                {

                    if (suratTugasList[i].Namafilesurat == null)
                    {
                        status = "Belum ada surat tugas";
                    }

                    /*worksheet.Cells[i + 2, 1].Value = no;
                    worksheet.Cells[i + 2, 2].Value = suratTugasList[i].Namakegiatan;
                    worksheet.Cells[i + 2, 3].Value = suratTugasList[i].Waktupelaksanaan;
                    worksheet.Cells[i + 2, 4].Value = suratTugasList[i].Prsonil;
                    worksheet.Cells[i + 2, 5].Value = suratTugasList[i].TPM;
                    worksheet.Cells[i + 2, 6].Value = suratTugasList[i].MI;
                    worksheet.Cells[i + 2, 7].Value = suratTugasList[i].MK;
                    worksheet.Cells[i + 2, 8].Value = suratTugasList[i].MO;
                    worksheet.Cells[i + 2, 9].Value = suratTugasList[i].P4;
                    worksheet.Cells[i + 2, 10].Value = suratTugasList[i].TRL;
                    worksheet.Cells[i + 2, 11].Value = suratTugasList[i].TKBG;
                    worksheet.Cells[i + 2, 12].Value = suratTugasList[i].Jumlahpenerima;
                    worksheet.Cells[i + 2, 13].Value = suratTugasList[i].AsalPenerima;
                    worksheet.Cells[i + 2, 14].Value = suratTugasList[i].Mahasiswa;
                    worksheet.Cells[i + 2, 15].Value = suratTugasList[i].Prodi;
                    worksheet.Cells[i + 2, 16].Value = suratTugasList[i].Nim;*/

                    worksheet.Cells[i + 2, 1, i + 2, 16].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
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
