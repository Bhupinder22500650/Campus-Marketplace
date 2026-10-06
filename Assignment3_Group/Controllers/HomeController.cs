using Assignment3_Group.Data;
using Assignment3_Group.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Assignment3_Group.Controllers
{
    public class HomeController : Controller
    {
        private readonly StudentMarketplaceDB _db;

        // Let this controller use the database.
        public HomeController(StudentMarketplaceDB db)
        {
            _db = db;
        }

        // Show the home page.
        public IActionResult Index()
        {
            return View();
        }
        // Show the sign-up form.
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        // Save the new student's account.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUp(User obj)
        {
            if (ModelState.IsValid)
            {
                _db.Users.Add(obj);
                _db.SaveChanges();

                User currentUser = new User(
                    obj.UserId,
                    obj.UserName,
                    obj.Password,
                    obj.email,
                    obj.PhoneNumber);

                // Remember the student after signing up.
                HttpContext.Session.SetString(
                    "CurrentUser",
                    JsonSerializer.Serialize(currentUser));

                return RedirectToAction("Index");
            }

            return View(obj);
        }

        // Show the login form.
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Check the student's login details.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User obj)
        {
            var users = _db.Users.ToList();

            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].UserName == obj.UserName &&
                    users[i].Password == obj.Password)
                {
                    User currentUser = new User(
                        users[i].UserId,
                        users[i].UserName,
                        users[i].Password,
                        users[i].email,
                        users[i].PhoneNumber);

                    // Remember the student after logging in.
                    HttpContext.Session.SetString(
                        "CurrentUser",
                        JsonSerializer.Serialize(currentUser));

                    return RedirectToAction("Index");
                }
            }

            ViewBag.LoginError = "Incorrect username or password.";

            return View(obj);
        }

        // Show the marketplace feed.
        [HttpGet]
        public IActionResult MarketPlace(
            string? search,
            int? categoryId,
            string sort = "newest",
            string status = "all")
        {
            string? sessionData =
                HttpContext.Session.GetString("CurrentUser");

            if (sessionData == null)
            {
                return RedirectToAction("Login");
            }

            User? currentUser =
                JsonSerializer.Deserialize<User>(sessionData);

            if (currentUser != null)
            {
                ViewBag.UserName = currentUser.UserName;
            }

            // Get the items and their category names.
            var listings = _db.Listings
                .Include(item => item.Category)
                .AsNoTracking()
                .AsQueryable();

            // Search the title and description.
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                listings = listings.Where(item =>
                    item.ListingTitle.Contains(search) ||
                    item.ListingDescription.Contains(search));
            }

            // Show items from the chosen category.
            if (categoryId.HasValue)
            {
                listings = listings.Where(item =>
                    item.ListingCategory == categoryId.Value);
            }

            // Show available items or sold items.
            if (status == "available")
            {
                listings = listings.Where(item =>
                    item.ListingStatus == true);
            }
            else if (status == "sold")
            {
                listings = listings.Where(item =>
                    item.ListingStatus == false);
            }
            else
            {
                status = "all";
            }

            // Put the items in the chosen order.
            if (sort == "priceLow")
            {
                listings = listings
                    .OrderBy(item => item.ListingPrice)
                    .ThenByDescending(item => item.ListingId);
            }
            else if (sort == "priceHigh")
            {
                listings = listings
                    .OrderByDescending(item => item.ListingPrice)
                    .ThenByDescending(item => item.ListingId);
            }
            else
            {
                sort = "newest";

                listings = listings
                    .OrderByDescending(item => item.ListingDate)
                    .ThenByDescending(item => item.ListingId);
            }

            // Keep the student's search and filter choices.
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.Sort = sort;
            ViewBag.Status = status;

            ViewBag.Categories = new SelectList(
                _db.Categories
                    .OrderBy(category => category.CategoryName)
                    .ToList(),
                "CategoryId",
                "CategoryName",
                categoryId);

            return View(listings.ToList());
        }

        // Show the details of one item.
        [HttpGet]
        public IActionResult MarketPlaceDetails(int id)
        {
            string? sessionData =
                HttpContext.Session.GetString("CurrentUser");

            if (sessionData == null)
            {
                return RedirectToAction("Login");
            }

            User? currentUser =
                JsonSerializer.Deserialize<User>(sessionData);

            if (currentUser != null)
            {
                ViewBag.UserName = currentUser.UserName;
            }

            // Find the item, its category and its seller.
            var listing = _db.Listings
                .Include(item => item.Category)
                .Include(item => item.Seller)
                .AsNoTracking()
                .FirstOrDefault(item => item.ListingId == id);

            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        // Show an empty form for a new listing.
        [HttpGet]
        public IActionResult MarketPlaceCreate()
        {
            string? sessionData =
                HttpContext.Session.GetString("CurrentUser");

            if (sessionData == null)
            {
                return RedirectToAction("Login");
            }

            User? currentUser =
                JsonSerializer.Deserialize<User>(sessionData);

            if (currentUser != null)
            {
                ViewBag.UserName = currentUser.UserName;
            }

            // Get the category names from the database.
            ViewBag.Categories = new SelectList(
                _db.Categories
                    .OrderBy(category => category.CategoryName)
                    .ToList(),
                "CategoryId",
                "CategoryName");

            return View(new Listings());
        }

        // Check the form and save the new item.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarketPlaceCreate(
            [Bind("ListingTitle,ListingDescription,ListingCategory,ListingPrice,ListingCondition,ListingStatus,ContactEmail,ContactPhoneNumber")]
            Listings obj,
            IFormFile? imageFile)
        {
            // Check that the student is signed in.
            string? sessionData =
                HttpContext.Session.GetString("CurrentUser");

            if (sessionData == null)
            {
                return RedirectToAction("Login");
            }

            User? currentUser =
                JsonSerializer.Deserialize<User>(sessionData);

            if (currentUser == null)
            {
                return RedirectToAction("Login");
            }

            // Check that the student's account still exists.
            bool userExists = _db.Users.Any(
                user => user.UserId == currentUser.UserId);

            if (!userExists)
            {
                return RedirectToAction("Login");
            }

            // Use the signed-in student as the seller.
            obj.SellerId = currentUser.UserId;
            obj.ListingDate = DateTime.Now;

            // The student does not enter the seller ID.
            ModelState.Remove("SellerId");

            // Check that the chosen category exists.
            if (obj.ListingCategory > 0)
            {
                bool categoryExists = _db.Categories.Any(
                    category =>
                        category.CategoryId == obj.ListingCategory);

                if (!categoryExists)
                {
                    ModelState.AddModelError(
                        "ListingCategory",
                        "Please select an existing category.");
                }
            }

            string fileExtension = "";

            // Check the picture when the student chooses one.
            if (imageFile != null && imageFile.Length > 0)
            {
                fileExtension = Path.GetExtension(
                    imageFile.FileName).ToLowerInvariant();

                // Only allow these picture file endings.
                if (fileExtension != ".jpg" &&
                    fileExtension != ".jpeg" &&
                    fileExtension != ".png" &&
                    fileExtension != ".webp")
                {
                    ModelState.AddModelError(
                        "imageFile",
                        "Please choose a JPG, JPEG, PNG or WEBP image.");
                }

                // The picture must be 2 MB or smaller.
                if (imageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError(
                        "imageFile",
                        "Please choose an image of 2 MB or smaller.");
                }
            }

            // Save only when the form has no errors.
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Give the picture a new name.
                        string fileName =
                            Guid.NewGuid().ToString("N") +
                            fileExtension;

                        string imageFolder = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            "images");

                        // Create the folder if it is missing.
                        Directory.CreateDirectory(imageFolder);

                        string imagePath = Path.Combine(
                            imageFolder,
                            fileName);

                        // Save the picture without replacing another file.
                        using (var stream = new FileStream(
                            imagePath,
                            FileMode.CreateNew))
                        {
                            imageFile.CopyTo(stream);
                        }

                        obj.ImageFileName = fileName;
                    }

                    // Save the item and its optional contact details.
                    _db.Listings.Add(obj);
                    _db.SaveChanges();

                    return RedirectToAction("MarketPlace");
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "The listing could not be saved. Please try again.");
                }
                catch (IOException)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "The picture could not be saved. Please choose it again.");
                }
                catch (UnauthorizedAccessException)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "The app could not save the picture in the images folder.");
                }
            }

            // Show the categories again and keep the chosen category.
            ViewBag.Categories = new SelectList(
                _db.Categories
                    .OrderBy(category => category.CategoryName)
                    .ToList(),
                "CategoryId",
                "CategoryName",
                obj.ListingCategory);

            ViewBag.UserName = currentUser.UserName;

            // Show the form again with its error messages.
            return View(obj);
        }

        //User details (only for signed in user)
        public IActionResult UserDetails()
        {
            return View();
        }

        // Show the error page.
        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId =
                    Activity.Current?.Id ??
                    HttpContext.TraceIdentifier
            });
        }
    }
}