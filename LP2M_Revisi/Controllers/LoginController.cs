using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using LP2M_Revisi.Models;

namespace LP2M_Revisi.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(string username,string password)
        {
            var pengguna = _context.Penggunas
         .FirstOrDefault(p => p.Username == username && p.Password == password);

            if (pengguna != null)
            {
                // Pengguna ditemukan, simpan data pengguna dalam sesi
                string serializedModel = JsonConvert.SerializeObject(pengguna);
                HttpContext.Session.SetString("Identity", serializedModel);
                if(pengguna.Role == "Admin")
                {
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    return RedirectToAction("DKaryawan", "Dashboard");
                }
            }
            else
            {
                // Pengguna tidak ditemukan, mungkin tambahkan penanganan kesalahan di sini
                return RedirectToAction("Index");
            }
        }
    }
}
