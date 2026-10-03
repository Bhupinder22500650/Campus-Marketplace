using Assignment3_Group.Data;
using Assignment3_Group.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                User currentUser = new User(obj.UserId, obj.UserName, obj.Password, obj.email, obj.PhoneNumber);

                //Saving currentUser details into the http session
                HttpContext.Session.SetString("CurrentUser", JsonSerializer.Serialize(currentUser));

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
            //Turning the database into a list so i can check the data inside
            var usersDb = _db.Users.ToList();

            for (int i = 0; i < usersDb.Count; i++)
            {
                //Making sure both the username and password are correct
                if (usersDb[i].UserName == obj.UserName && usersDb[i].Password == obj.Password)
                {
                    User currentUser = new User(usersDb[i].UserId, usersDb[i].UserName, usersDb[i].Password, usersDb[i].email, usersDb[i].PhoneNumber);

                    //Saving currentUser details into the http session
                    HttpContext.Session.SetString("CurrentUser", JsonSerializer.Serialize(currentUser));

                    //Need to try save data somewhere so the user is actually logged in and can do everything we want them to be able to do
                    return RedirectToAction("Index");
                }
            }
            //Find a way to tell the user they logged in wrong
            ViewBag.LoginError = "Incorrect username or password.";

            return View(obj);
        }

        // Load listings for the marketplace feed.
        [HttpGet]
        public IActionResult MarketPlace(string? search, int? categoryId,
            string sort = "newest", string status = "all")
        { 
            if (HttpContext.Session.GetString("CurrentUser") == null)
            {
                return RedirectToAction("Login");
            }

                // Include also loads the category name for each listing.
                var listings = _db.Listings.Include(item => item.Category)
                    .AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                listings = listings.Where(item =>
                    item.ListingTitle.Contains(search) ||
                    item.ListingDescription.Contains(search));
            }

            if (categoryId.HasValue)
            {
                listings = listings.Where(item => item.ListingCategory == categoryId.Value);
            }

            if (status == "available")
            {
                listings = listings.Where(item => item.ListingStatus == true);
            }
            else if (status == "sold")
            {
                listings = listings.Where(item => item.ListingStatus == false);
            }
            else
            {
                status = "all";
            }

            if (sort == "priceLow")
            {
                listings = listings.OrderBy(item => item.ListingPrice)
                    .ThenByDescending(item => item.ListingId);
            }
            else if (sort == "priceHigh")
            {
                listings = listings.OrderByDescending(item => item.ListingPrice)
                    .ThenByDescending(item => item.ListingId);
            }
            else
            {
                sort = "newest";
                listings = listings.OrderByDescending(item => item.ListingDate)
                    .ThenByDescending(item => item.ListingId);
            }

            // Keep the selected values visible after clicking Apply.
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.Sort = sort;
            ViewBag.Status = status;
            ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _db.Categories.OrderBy(category => category.CategoryName).ToList(),
                "CategoryId", "CategoryName", categoryId);

            return View(listings.ToList());
        }

        // DETAILS: display one listing, including the seller's contact details.
        [HttpGet]
        public IActionResult MarketPlaceDetails(int id)
        {
            if (HttpContext.Session.GetString("CurrentUser") == null)
            {
                return RedirectToAction("Login");
            }

            var listing = _db.Listings.Include(item => item.Category)
                .Include(item => item.Seller).AsNoTracking()
                .FirstOrDefault(item => item.ListingId == id);

            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        // CREATE GET: display an empty form.
        [HttpGet]
        public IActionResult MarketPlaceCreate()
        {
            if (HttpContext.Session.GetString("CurrentUser") == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _db.Categories.OrderBy(category => category.CategoryName).ToList(),
                "CategoryId", "CategoryName");

            return View(new Listings());
        }

        // CREATE POST: check the form, save the item and return to the feed.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarketPlaceCreate(
            [Bind("ListingTitle,ListingDescription,ListingCategory,ListingPrice,ListingCondition,ListingStatus,ImageFileName")]
            Listings obj, IFormFile? imageFile)
        {
            string? sessionData = HttpContext.Session.GetString("CurrentUser");
            if (sessionData == null)
            {
                return RedirectToAction("Login");
            }

            User? currentUser = JsonSerializer.Deserialize<User>(sessionData);
            if (currentUser == null || !_db.Users.Any(user => user.UserId == currentUser.UserId))
            {
                return RedirectToAction("Login");
            }

            // Never let someone type another seller's ID in the form.
            obj.SellerId = currentUser.UserId;
            obj.ListingDate = DateTime.Now;
            ModelState.Remove("SellerId");

            if (!_db.Categories.Any(category => category.CategoryId == obj.ListingCategory))
            {
                ModelState.AddModelError("ListingCategory", "Please select an existing category.");
            }

            //This is so the user can upload a file and we save the name to the database, but the actual image to wwwroot/images
            if (imageFile != null && imageFile.Length > 0)
            {
                var filename = Path.GetFileName(imageFile.FileName);

                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", filename);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                obj.ImageFileName = filename;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Listings.Add(obj);
                    _db.SaveChanges();
                    return RedirectToAction("MarketPlace");
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty,
                        "The listing could not be saved. Check the database and try again.");
                }
            }

            // Reload the dropdown when the form has an error.
            ViewBag.Categories = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                _db.Categories.OrderBy(category => category.CategoryName).ToList(),
                "CategoryId", "CategoryName", obj.ListingCategory);

            return View(obj);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
