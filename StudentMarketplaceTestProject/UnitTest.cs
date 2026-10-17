using Assignment3_Group.Controllers;
using Assignment3_Group.Data;
using Assignment3_Group.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq; 
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ViewFeatures;



namespace StudentMarketplaceTestProject
{
    [TestClass]
    public sealed class UnitTest
    {
        //Creating in-memory database from student application 
        private StudentMarketplaceDB A_GetDbContext()
        {
            //creating temporary DB for testing purposes

            var options = new DbContextOptionsBuilder<StudentMarketplaceDB>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            var context = new StudentMarketplaceDB(options);

            //Seed test user data 
            context.Users.AddRange(
                new User
                {
                    UserId = 123,
                    UserName = "Jacob Gold",
                    Password = "UpDaWahz",
                    email = "Jacob.Gold@gmail.com",
                    PhoneNumber = "0216554328"
                },

                new User
                {
                    UserId = 234,
                    UserName = "Jane Price",
                    Password = "WindyW3lly",
                    email = "Jane.Price@gmail.com",
                    PhoneNumber = "02198875234"
                },

                new User
                {
                    UserId = 345,
                    UserName = "David Jones",
                    Password = "UpDaWahz",
                    email = "David.Jones@gmail.com",
                    PhoneNumber = "02176654987"
                }
            );

            //Seed test catagory data 
            context.Categories.AddRange(
                new Category
                {
                    CategoryId = 123,
                    CategoryName = "Toys",
                    CategoryDescription = "Kids toys"
                },

                new Category
                {
                    CategoryId = 234,
                    CategoryName = "Electronic",
                    CategoryDescription = "Anything that is electronic and connects to wifi"
                },

                new Category
                {
                    CategoryId = 345,
                    CategoryName = "Furnature",
                    CategoryDescription = "Any item you would put into your home"
                }
            );

            //Seed test listing data 
            context.Listings.AddRange(
                new Listings
                {
                    ListingId = 123,
                    SellerId = 123,
                    ListingCategory = 123,
                    ListingPrice = 20,
                    ListingStatus = true,
                    ListingCondition = "Used",
                    ListingDate = DateTime.Now,
                    ListingDescription = "This is an old woody toy from toy story",
                    ListingTitle = "Woody toy story",
                    ContactEmail = "Jacob.Gold@gmail.com",
                    ContactPhoneNumber = "0216554328"
                },

                new Listings
                {
                    ListingId = 234,
                    SellerId = 234,
                    ListingCategory = 234,
                    ListingPrice = 80,
                    ListingStatus = true,
                    ListingCondition = "Used",
                    ListingDate = DateTime.Now,
                    ListingDescription = "An old iphone 14 that i used",
                    ListingTitle = "Iphone 14",
                    ContactEmail = "Jane.Price@gmail.com",
                    ContactPhoneNumber = "02198875234"
                },

                new Listings
                {
                    ListingId = 345,
                    SellerId = 345,
                    ListingCategory = 345,
                    ListingPrice = 60,
                    ListingStatus = true,
                    ListingCondition = "Used",
                    ListingDate = DateTime.Now,
                    ListingDescription = "An old wooden stool",
                    ListingTitle = "Wooden stool",
                    ContactEmail = "David.Jones@gmail.com",
                    ContactPhoneNumber = "02176654987"
                }
            );

            context.SaveChanges();
            return context;
        }

        [TestMethod]
        public void B_Index_Load()
        {
            // Arrange
            var context = A_GetDbContext();
            var controller = new HomeController(context);

            // Act
            IActionResult result = controller.Index();

            // Assert
            // Making sure result is retrun view since that is all index passes
            if (result is ViewResult viewResult)
            {
                Assert.IsNull(viewResult.Model);
            }
            else
            {
                // Force the test to fail because it wasn't ViewResult
                Assert.Fail($"Expected a ViewResult, but got a {result?.GetType().Name ?? "null"}");
            }
        }

        [TestMethod]
        public void C_Index_CreateAccount()
        {
            // Arrange
            var context = A_GetDbContext();
            var controller = new HomeController(context);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new Mock<ISession>().Object;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //create a user so signUp works 
            var user = new User
            {
                UserName = "TestStudent",
                Password = "TestPassword123",
                email = "teststudent@example.com",
                PhoneNumber = "0211234567"
            };

            // Act
            IActionResult result = controller.SignUp(user);

            // Assert
            Assert.AreEqual(4, context.Users.Count());
            Assert.IsTrue(context.Users.Any(u => u.UserName == "TestStudent"), "The new user was not saved.");
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
        }

        [TestMethod]
        public void D_Index_Login()
        {
            // Arrange
            var context = A_GetDbContext();
            var controller = new HomeController(context);

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new Mock<ISession>().Object;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            //create a user so signUp works 
            var user = new User
            {
                UserName = "TestStudent",
                Password = "TestPassword123",
                email = "teststudent@example.com",
                PhoneNumber = "0211234567"
            };

            // Save the user
            context.Users.Add(user);
            context.SaveChanges();

            // 2. Act
            var result = controller.Login(user);

            // 3. Assert
            var redirectResult = result as RedirectToActionResult;

            //Checks that redurectResult isn't null if it is, show the text
            Assert.IsNotNull(redirectResult, "The action did not return a RedirectToActionResult.");

            // Making sure the controller login worked by checking if we are in marketplace
            Assert.AreEqual("MarketPlace", redirectResult.ActionName);
        }

        [TestMethod]
        public void E_Marketplace()
        {
            // Arrange
            var context = A_GetDbContext();
            var controller = new HomeController(context);

            //Set up the faked session data
            var httpContext = new DefaultHttpContext();
            var sessionMock = new Mock<ISession>();

            // Create the JSON string the controller expects to find in the session
            string userJson = "{\"UserId\":1,\"UserName\":\"TestStudent\",\"Password\":\"TestPassword123\",\"email\":\"teststudent@example.com\",\"PhoneNumber\":\"0211234567\"}";
            byte[] sessionValue = System.Text.Encoding.UTF8.GetBytes(userJson);

            // Mock TryGetValue so controller gets "CurrentUser" and succeeds
            sessionMock
                .Setup(s => s.TryGetValue("CurrentUser", out sessionValue))
                .Returns(true);

            httpContext.Session = sessionMock.Object;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // create a user so database matches
            var user = new User
            {
                UserName = "TestStudent",
                Password = "TestPassword123",
                email = "teststudent@example.com",
                PhoneNumber = "0211234567"
            };

            context.Users.Add(user);
            context.SaveChanges();

            // For marketplace parameters
            string search = null;
            int categoryId = 0;
            string sort = null;
            string status = null;

            // 2. Act
            var result = controller.MarketPlace(search, categoryId, sort, status);

            // 3. Assert
            if (result is ViewResult viewResult)
            {
                Assert.IsNotNull(viewResult);
            }
            else
            {
                Assert.Fail($"Expected a ViewResult, but got a {result?.GetType().Name ?? "null"}");
            }

        }

        // Show the details of one item.
        [TestMethod]
        public void F_MarketPlaceDetails()
        {
            // Arrange
            var context = A_GetDbContext();
            var controller = new HomeController(context);

            //Set up the faked session data
            var httpContext = new DefaultHttpContext();
            var sessionMock = new Mock<ISession>();

            // Create the JSON string the controller expects to find in the session
            string userJson = "{\"UserId\":1,\"UserName\":\"TestStudent\",\"Password\":\"TestPassword123\",\"email\":\"teststudent@example.com\",\"PhoneNumber\":\"0211234567\"}";
            byte[] sessionValue = System.Text.Encoding.UTF8.GetBytes(userJson);

            sessionMock
                .Setup(s => s.TryGetValue("CurrentUser", out sessionValue))
                .Returns(true);

            httpContext.Session = sessionMock.Object;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // create a user so database matches
            var user = new User
            {
                UserName = "TestStudent",
                Password = "TestPassword123",
                email = "teststudent@example.com",
                PhoneNumber = "0211234567"
            };

            var lising = new Listings
            {
                ListingId = 123,
                SellerId = 123,
                ListingCategory = 123,
                ListingPrice = 20,
                ListingStatus = true,
                ListingCondition = "Used",
                ListingDate = DateTime.Now,
                ListingDescription = "This is an old woody toy from toy story",
                ListingTitle = "Woody toy story",
                ContactEmail = "Jacob.Gold@gmail.com",
                ContactPhoneNumber = "0216554328"
            };

            context.Users.Add(user);
            context.SaveChanges();

            // For marketplace parameters
            string search = null;
            int categoryId = 0;
            string sort = null;
            string status = null;

            // 2. Act
            var result = controller.MarketPlaceDetails(lising.ListingId);

            // 3. Assert
            if (result is ViewResult viewResult)
            {
                Assert.IsNotNull(viewResult);
            }
            else
            {
                Assert.Fail($"Expected a ViewResult, but got a {result?.GetType().Name ?? "null"}");
            }
        }

        [TestMethod]
        public void G_MarketplaceCreate()
        {
            // Arrange
            var context = A_GetDbContext();
            var controller = new HomeController(context);

            //Set up the faked session data
            var httpContext = new DefaultHttpContext();
            var sessionMock = new Mock<ISession>();

            string userJson =
                "{\"UserId\":123,\"UserName\":\"Jacob Gold\",\"Password\":\"UpDaWahz\",\"email\":\"Jacob.Gold@gmail.com\",\"PhoneNumber\":\"0216554328\"}";

            byte[] sessionValue =
                System.Text.Encoding.UTF8.GetBytes(userJson);

            sessionMock
                .Setup(s => s.TryGetValue("CurrentUser", out sessionValue))
                .Returns(true);

            httpContext.Session = sessionMock.Object;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Create a new listing
            var listing = new Listings
            {
                ListingCategory = 123,
                ListingPrice = 20,
                ListingStatus = true,
                ListingCondition = "Used",
                ListingDescription = "An old Woody toy",
                ListingTitle = "Test Woody Toy",
                ContactEmail = "Jacob.Gold@gmail.com",
                ContactPhoneNumber = "0216554328"
            };

            // Act
            IActionResult result = controller.MarketPlaceCreate(listing, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));

            var redirectResult = result as RedirectToActionResult;

            Assert.AreEqual("MarketPlace", redirectResult!.ActionName);

            Assert.IsTrue(context.Listings.Any(
                l => l.ListingTitle == "Test Woody Toy"));

            var savedListing = context.Listings.First(
                l => l.ListingTitle == "Test Woody Toy");

            Assert.AreEqual(123, savedListing.SellerId);
        }

        [TestMethod]
        public void H_MarketPlaceCreate()
        {
            // Arrange
            var context = A_GetDbContext();
            var controller = new HomeController(context);

            //Set up the faked session data
            var httpContext = new DefaultHttpContext();
            var sessionMock = new Mock<ISession>();

            string userJson =
                "{\"UserId\":123,\"UserName\":\"Jacob Gold\",\"Password\":\"UpDaWahz\",\"email\":\"Jacob.Gold@gmail.com\",\"PhoneNumber\":\"0216554328\"}";

            byte[] sessionValue =
                System.Text.Encoding.UTF8.GetBytes(userJson);

            sessionMock
                .Setup(s => s.TryGetValue("CurrentUser", out sessionValue))
                .Returns(true);

            httpContext.Session = sessionMock.Object;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Create the listing
            var listing = new Listings
            {
                ListingCategory = 123,
                ListingPrice = 20,
                ListingStatus = true,
                ListingCondition = "Used",
                ListingDescription = "An old Woody toy",
                ListingTitle = "Test Woody Toy",
                ContactEmail = "Jacob.Gold@gmail.com",
                ContactPhoneNumber = "0216554328"
            };

            // Act
            IActionResult result =
                controller.MarketPlaceCreate(listing, null);

            // Assert
            Assert.AreEqual(4, context.Listings.Count());

            Assert.IsTrue(context.Listings.Any(l => l.ListingTitle == "Test Woody Toy"));

            var savedListing = context.Listings.First(
                l => l.ListingTitle == "Test Woody Toy");

            Assert.AreEqual(123, savedListing.SellerId);

            var redirectResult = result as RedirectToActionResult;

            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("MarketPlace", redirectResult.ActionName);
        }

        [TestMethod]
        public void I_MyListings()
        {
            // Arrange
            var context = A_GetDbContext();
            var controller = new HomeController(context);

            //Set up the faked session data
            var httpContext = new DefaultHttpContext();
            var sessionMock = new Mock<ISession>();

            // Create the JSON string the controller expects to find in the session
            string userJson = "{\"UserId\":1,\"UserName\":\"TestStudent\",\"Password\":\"TestPassword123\",\"email\":\"teststudent@example.com\",\"PhoneNumber\":\"0211234567\"}";
            byte[] sessionValue = System.Text.Encoding.UTF8.GetBytes(userJson);

            sessionMock
                .Setup(s => s.TryGetValue("CurrentUser", out sessionValue))
                .Returns(true);

            httpContext.Session = sessionMock.Object;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // create a user so database matches
            var user = new User
            {
                UserName = "TestStudent",
                Password = "TestPassword123",
                email = "teststudent@example.com",
                PhoneNumber = "0211234567"
            };

            var lising = new Listings
            {
                ListingId = 123,
                SellerId = 123,
                ListingCategory = 123,
                ListingPrice = 20,
                ListingStatus = true,
                ListingCondition = "Used",
                ListingDate = DateTime.Now,
                ListingDescription = "This is an old woody toy from toy story",
                ListingTitle = "Woody toy story",
                ContactEmail = "Jacob.Gold@gmail.com",
                ContactPhoneNumber = "0216554328"
            };

            context.Users.Add(user);
            context.SaveChanges();

            // For marketplace parameters
            string search = null;
            int categoryId = 0;
            string sort = null;
            string status = null;

            // 2. Act
            var result = controller.MarketPlaceDetails(lising.ListingId);

            // 3. Assert
            if (result is ViewResult viewResult)
            {
                Assert.IsNotNull(viewResult); 
            }
            else
            {
                Assert.Fail($"Expected a ViewResult, but got a {result?.GetType().Name ?? "null"}");
            }
        }

        // Save the edit form.
        [TestMethod]
        public void J_MarketPlaceEdit()
        {
            // Arrange
            var context = A_GetDbContext();
            var controller = new HomeController(context);

            //Set up the faked session data
            var httpContext = new DefaultHttpContext();
            var sessionMock = new Mock<ISession>();

            string userJson =
                "{\"UserId\":123,\"UserName\":\"Jacob Gold\",\"Password\":\"UpDaWahz\",\"email\":\"Jacob.Gold@gmail.com\",\"PhoneNumber\":\"0216554328\"}";

            byte[] sessionValue =
                System.Text.Encoding.UTF8.GetBytes(userJson);

            sessionMock
                .Setup(s => s.TryGetValue("CurrentUser", out sessionValue))
                .Returns(true);

            httpContext.Session = sessionMock.Object;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // Edit the existing listing with ID 123
            var listing = context.Listings.First(l => l.ListingId == 123);

            listing.ListingTitle = "Updated Woody Toy";
            listing.ListingDescription = "An updated Woody toy description";
            listing.ListingCategory = 123;
            listing.ListingPrice = 25;
            listing.ListingCondition = "Used";
            listing.ListingStatus = true;
            listing.ContactEmail = "Jacob.Gold@gmail.com";
            listing.ContactPhoneNumber = "0216554328";

            // Act
            IActionResult result = controller.MarketPlaceEdit(listing);

            // Assert
            var redirectResult = result as RedirectToActionResult;

            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("MyListings", redirectResult.ActionName);

            var savedListing = context.Listings.First(
                l => l.ListingId == 123);

            Assert.AreEqual("Updated Woody Toy", savedListing.ListingTitle);
            Assert.AreEqual(25, savedListing.ListingPrice);
            Assert.AreEqual(123, savedListing.SellerId);
        }

        [TestMethod]
        public void K_DeletePost()
        {
            var context = A_GetDbContext();
            var controller = new HomeController(context);

            //Set up the faked session data
            var httpContext = new DefaultHttpContext();
            var sessionMock = new Mock<ISession>();

            string userJson =
                "{\"UserId\":123,\"UserName\":\"Jacob Gold\",\"Password\":\"UpDaWahz\",\"email\":\"Jacob.Gold@gmail.com\",\"PhoneNumber\":\"0216554328\"}";
            byte[] sessionValue = System.Text.Encoding.UTF8.GetBytes(userJson);

            sessionMock
                .Setup(s => s.TryGetValue("CurrentUser", out sessionValue))
                .Returns(true);

            httpContext.Session = sessionMock.Object;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Set up TempData.
            var tempDataProvider = new Mock<ITempDataProvider>();
            tempDataProvider
                .Setup(p => p.LoadTempData(It.IsAny<HttpContext>()))
                .Returns(new Dictionary<string, object>());
            tempDataProvider
                .Setup(p => p.SaveTempData(
                    It.IsAny<HttpContext>(),
                    It.IsAny<IDictionary<string, object>>()));

            controller.TempData = new TempDataDictionary(
                httpContext, tempDataProvider.Object);

            // Confirm the listing exists before deleting it.
            Assert.IsTrue(context.Listings.Any(l => l.ListingId == 123));

            IActionResult result = controller.DeletePost(123);

            // Check the redirect and that the listing was deleted.
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("MyListings", redirectResult.ActionName);
            Assert.IsFalse(context.Listings.Any(l => l.ListingId == 123));
        }

        [TestMethod]
        public void L_ChangeListingStatus()
        {
            var context = A_GetDbContext();
            var controller = new HomeController(context);

            //Set up the faked session data
            var httpContext = new DefaultHttpContext();
            var sessionMock = new Mock<ISession>();

            string userJson =
                "{\"UserId\":123,\"UserName\":\"Jacob Gold\",\"Password\":\"UpDaWahz\",\"email\":\"Jacob.Gold@gmail.com\",\"PhoneNumber\":\"0216554328\"}";
            byte[] sessionValue = System.Text.Encoding.UTF8.GetBytes(userJson);

            sessionMock
                .Setup(s => s.TryGetValue("CurrentUser", out sessionValue))
                .Returns(true);

            httpContext.Session = sessionMock.Object;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Set up TempData.
            var tempDataProvider = new Mock<ITempDataProvider>();
            tempDataProvider.Setup(p => p.LoadTempData(It.IsAny<HttpContext>())).Returns(new Dictionary<string, object>());
            tempDataProvider.Setup(p => p.SaveTempData(It.IsAny<HttpContext>(),It.IsAny<IDictionary<string, object>>()));

            controller.TempData = new TempDataDictionary(
                httpContext, tempDataProvider.Object);

            // Make sure the listing starts as available.
            var listing = context.Listings.First(l => l.ListingId == 123);
            listing.ListingStatus = true;
            context.SaveChanges();

            // Change the listing to sold.
            IActionResult result = controller.ChangeListingStatus(123, false);

            // Check the redirect and the updated status.
            var redirectResult = result as RedirectToActionResult;
            Assert.IsNotNull(redirectResult);
            Assert.AreEqual("MyListings", redirectResult.ActionName);

            var updatedListing =
                context.Listings.First(l => l.ListingId == 123);

            Assert.IsFalse(updatedListing.ListingStatus);
        }

        [TestMethod]
        public void M_UserDetails()
        {
            var context = A_GetDbContext();
            var controller = new HomeController(context);

            //Set up the faked session data
            var httpContext = new DefaultHttpContext();
            var sessionMock = new Mock<ISession>();

            string userJson =
                "{\"UserId\":123,\"UserName\":\"Jacob Gold\",\"Password\":\"UpDaWahz\",\"email\":\"Jacob.Gold@gmail.com\",\"PhoneNumber\":\"0216554328\"}";
            byte[] sessionValue = System.Text.Encoding.UTF8.GetBytes(userJson);

            sessionMock
                .Setup(s => s.TryGetValue("CurrentUser", out sessionValue))
                .Returns(true);

            httpContext.Session = sessionMock.Object;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Open the student's profile.
            IActionResult result = controller.UserDetails();

            // Check that the profile view displays the correct user.
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);

            var user = viewResult.Model as User;
            Assert.IsNotNull(user);

            Assert.AreEqual(123, user.UserId);
            Assert.AreEqual("Jacob Gold", user.UserName);
            Assert.AreEqual("Jacob.Gold@gmail.com", user.email);
            Assert.AreEqual("0216554328", user.PhoneNumber);
        }

    }
}
