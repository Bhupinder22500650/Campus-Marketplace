using System.Diagnostics;
using Assignment3_Group.Data;
using Assignment3_Group.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment3_Group.Controllers
{
    public class HomeController : Controller
    {
        private readonly StudentMarketplaceDB _db; 
        public HomeController(StudentMarketplaceDB db)
        {
            _db = db;
        }

        /*
         * Don't think we need this so i commented it out
            private readonly ILogger<HomeController> _logger;

            public HomeController(ILogger<HomeController> logger)
            {
                _logger = logger;
            }
        */

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //Login (create method)
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUp(User obj)
        {
            if (ModelState.IsValid)
            {
                _db.Users.Add(obj);
                _db.SaveChanges();
                //Will change so it sends to the actual page where you can look at whats listed
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
