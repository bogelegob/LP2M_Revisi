using LP2M_Revisi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LP2M_Revisi.Controllers
{
    public class SSOController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SSOController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
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
                return RedirectToAction("Admin");
            }
            else
            {
                return RedirectToAction("Karyawan");
            }
        }
        public IActionResult Admin()
        {
            return View();
        }
        public IActionResult Karyawan()
        {
            return View();
        }
    }
}
