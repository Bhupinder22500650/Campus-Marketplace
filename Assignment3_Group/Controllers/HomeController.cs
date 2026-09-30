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

        //Sign up (create method)
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

        //Login (create method)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User obj)
        {
            if (!ModelState.IsValid)
            {
                //Turning the database into a list so i can check the data inside
                var usersDb = _db.Users.ToList();

                for (int i = 0; i < usersDb.Count; i++)
                {
                    //Making sure both the username and password are correct
                    if (usersDb[i].UserName == obj.UserName && usersDb[i].Password == obj.Password)
                    {
                        //Need to try save data somewhere so the user is actually logged in and can do everything we want them to be able to do
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        //Find a way to tell the user they logged in wrong
                        ViewBag.LoginError = "Incorrect username or password.";
                    }
                }
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
