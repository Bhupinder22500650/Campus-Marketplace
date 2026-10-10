using Assignment3_Group.Data;
using Assignment3_Group.Models;
using Microsoft.AspNetCore.Components.Web.Virtualization;
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

                // Show all marketplace items after login.
                return RedirectToAction("MarketPlace");
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

                    // Open the marketplace after login.
                    return RedirectToAction("MarketPlace");
                }
            }

            // Show an error if the login details are wrong.
            ViewBag.LoginError = "Incorrect username or password.";

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignOut()
        {

            // Removes the current user the session remembers and puts them back to index page
            HttpContext.Session.Remove("CurrentUser");

            return RedirectToAction("Index");
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

            if (currentUser == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.UserName = currentUser.UserName;

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
            else if (sort == "createdByYou")
            {
                listings = listings
                    .Where(item => item.SellerId == currentUser.UserId)
                    .OrderByDescending(item => item.ListingDate)
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

        // Read the student saved by our login page.
        private User? GetCurrentUser()
        {
            string? sessionData = HttpContext.Session.GetString("CurrentUser");

            if (string.IsNullOrEmpty(sessionData))
            {
                return null;
            }

            return JsonSerializer.Deserialize<User>(sessionData);
        }

        // Show only the signed-in student's listings.
        [HttpGet]
        public IActionResult MyListings()
        {
            User? currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login");
            }

            var listings = _db.Listings
                .AsNoTracking()
                .Where(item => item.SellerId == currentUser.UserId)
                .OrderByDescending(item => item.ListingDate)
                .ThenByDescending(item => item.ListingId)
                .ToList();

            return View(listings);
        }

        // Open the edit form for one of this student's items.
        [HttpGet]
        public IActionResult MarketPlaceEdit(int id)
        {
            User? currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login");
            }

            // Check the item ID AND its owner.
            var listing = _db.Listings.FirstOrDefault(item =>
                item.ListingId == id && item.SellerId == currentUser.UserId);

            if (listing == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(
                _db.Categories.OrderBy(category => category.CategoryName).ToList(),
                "CategoryId", "CategoryName", listing.ListingCategory);

            return View(listing);
        }

        // Save the edit form.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarketPlaceEdit(
            [Bind("ListingId,ListingTitle,ListingDescription,ListingCategory," +
                  "ListingPrice,ListingCondition,ListingStatus,ContactEmail,ContactPhoneNumber")]
            Listings obj)
        {
            User? currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login");
            }

            // Check ownership again when the form is submitted.
            var listing = _db.Listings.FirstOrDefault(item =>
                item.ListingId == obj.ListingId && item.SellerId == currentUser.UserId);

            if (listing == null)
            {
                return NotFound();
            }

            // The form does not choose the seller. Keep the saved seller.
            obj.SellerId = listing.SellerId;
            ModelState.Remove("SellerId");

            if (obj.ListingCategory > 0 &&
                !_db.Categories.Any(category => category.CategoryId == obj.ListingCategory))
            {
                ModelState.AddModelError("ListingCategory", "Please select a valid category.");
            }

            // Save only when the form has no validation errors.
            if (ModelState.IsValid)
            {
                listing.ListingTitle = obj.ListingTitle;
                listing.ListingDescription = obj.ListingDescription;
                listing.ListingCategory = obj.ListingCategory;
                listing.ListingPrice = obj.ListingPrice;
                listing.ListingCondition = obj.ListingCondition;
                listing.ListingStatus = obj.ListingStatus;
                listing.ContactEmail = obj.ContactEmail;
                listing.ContactPhoneNumber = obj.ContactPhoneNumber;

                try
                {
                    _db.SaveChanges();
                    TempData["ListingMessage"] = "Your listing was updated.";
                    return RedirectToAction("MyListings");
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "The update could not be saved. Please try again.");
                }
            }

            // Reload the dropdown if we need to show the form again.
            ViewBag.Categories = new SelectList(
                _db.Categories.OrderBy(category => category.CategoryName).ToList(),
                "CategoryId", "CategoryName", obj.ListingCategory);

            return View(obj);
        }

        // Show the delete confirmation page.
        [HttpGet]
        public IActionResult MarketPlaceDelete(int id)
        {
            User? currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login");
            }

            var listing = _db.Listings.FirstOrDefault(item =>
                item.ListingId == id && item.SellerId == currentUser.UserId);

            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        // Delete only after the student confirms the form.
        [HttpPost, ActionName("MarketPlaceDelete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int id)
        {
            User? currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login");
            }

            // A hidden form field can be changed, so check the owner here too.
            var listing = _db.Listings.FirstOrDefault(item =>
                item.ListingId == id && item.SellerId == currentUser.UserId);

            if (listing == null)
            {
                return NotFound();
            }

            try
            {
                _db.Listings.Remove(listing);
                _db.SaveChanges();
                TempData["ListingMessage"] = "Your listing was deleted.";
            }
            catch (DbUpdateException)
            {
                TempData["ListingError"] = "The listing could not be deleted. Please try again.";
            }

            return RedirectToAction("MyListings");
        }

        // Change an item to sold or available.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeListingStatus(int id, bool isAvailable)
        {
            User? currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var listing = _db.Listings.FirstOrDefault(item =>
                item.ListingId == id && item.SellerId == currentUser.UserId);

            if (listing == null)
            {
                return NotFound();
            }

            // True = available. False = sold.
            listing.ListingStatus = isAvailable;

            try
            {
                _db.SaveChanges();
                TempData["ListingMessage"] = isAvailable
                    ? "Your listing is available again."
                    : "Your listing was marked as sold.";
            }
            catch (DbUpdateException)
            {
                TempData["ListingError"] = "The status could not be saved. Please try again.";
            }

            return RedirectToAction("MyListings");
        }

        // Show the signed-in student's profile.
        [HttpGet]
        public IActionResult UserDetails()
        {
            User? currentUser = GetCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login");
            }

            return View(currentUser);
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