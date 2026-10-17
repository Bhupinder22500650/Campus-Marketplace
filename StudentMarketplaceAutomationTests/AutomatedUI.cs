using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;

namespace StudentMarketplaceAutomationTests
{
    [TestClass]
    public class AutomatedUI
    {
        // This controls Chrome
        private ChromeDriver driver;

        //Website address
        private string baseUrl = "https://localhost:7228";

        // Open Chrome before starting the test
        public AutomatedUI()
        {
            driver = new ChromeDriver();

            driver.Manage().Window.Maximize();

            // Wait up to 10 seconds to find an element
            driver.Manage().Timeouts().ImplicitWait =
                TimeSpan.FromSeconds(10);
        }

        [TestMethod]
        public void FullStudentJourney()
        {
            // Add the date and time to make a new username
            string testName = "Auto" + DateTime.Now.ToString("yyyyMMddHHmmssfff");

            //Test account details
            string password = "TestPass123!";
            string email = testName + "@example.com";
            string phone = "0215550101";

            //Two item names
            string bookTitle = testName + " Book";
            string mouseTitle = testName + " Mouse";

            Console.WriteLine("Test username: " + testName);

            // Creating account

            // Open Sign up
            driver.Navigate().GoToUrl(baseUrl + "/Home/SignUp");

            // Fill in the form
            driver.FindElement(By.Id("UserName")).SendKeys(testName);
            driver.FindElement(By.Id("Password")).SendKeys(password);
            driver.FindElement(By.Id("email")).SendKeys(email);
            driver.FindElement(By.Id("PhoneNumber")).SendKeys(phone);

            // Click Create
            driver.FindElement(
                By.CssSelector("main input[type='submit']")).Click();

            // Pause for one second so we can watch
            Thread.Sleep(1000);

            // Check the page heading and username
            Assert.AreEqual(
                "Marketplace",
                driver.FindElement(By.TagName("h1")).Text);

            Assert.AreEqual(
                testName,
                driver.FindElement(By.Id("userDetails")).Text);

            Console.WriteLine("PASS - account created.");

            // User signs out

            // Sign-up logs us in, so sign out first
            driver.FindElement(By.Id("userDetails")).Click();

            driver.FindElement(
                By.CssSelector("main button[type='submit']")).Click();

            // Open Login
            driver.FindElement(By.LinkText("Login")).Click();

            // User logs in

            // Enter the new account details
            driver.FindElement(By.Id("UserName")).SendKeys(testName);
            driver.FindElement(By.Id("Password")).SendKeys(password);

            // Click Login.
            driver.FindElement(
                By.CssSelector("main input[type='submit']")).Click();

            Thread.Sleep(1000);

            // Check that we are logged in
            Assert.AreEqual(
                "Marketplace",
                driver.FindElement(By.TagName("h1")).Text);

            Assert.AreEqual(
                testName,
                driver.FindElement(By.Id("userDetails")).Text);

            Console.WriteLine("PASS - login worked.");

            // Creating book listing

            driver.FindElement(By.LinkText("Sell an item")).Click();

            // Enter the book details
            driver.FindElement(By.Id("ListingTitle")).SendKeys(bookTitle);

            driver.FindElement(By.Id("ListingDescription"))
                .SendKeys("A book for our automation test.");

            // Choose Books.
            driver.FindElement(
                By.XPath("//select[@id='ListingCategory']/option[text()='Books']"))
                .Click();

            // Enter the price
            driver.FindElement(By.Id("ListingPrice")).Clear();
            driver.FindElement(By.Id("ListingPrice")).SendKeys("25.00");

            // Choose Used
            driver.FindElement(
                By.CssSelector(
                    "#ListingCondition option[value='Used']")).Click();

            // Tick Available if it is not ticked
            if (!driver.FindElement(By.Id("ListingStatus")).Selected)
            {
                driver.FindElement(By.Id("ListingStatus")).Click();
            }

            // Press Enter to create the listing
            driver.FindElement(
                By.CssSelector("main button[type='submit']"))
                .SendKeys(Keys.Enter);

            Thread.Sleep(1000);

            // Check that the book appears
            Assert.IsTrue(
                driver.FindElement(
                    By.XPath("//h2[text()='" + bookTitle + "']"))
                .Displayed);

            Console.WriteLine("PASS - book listing created.");

            // Creating mouse listing

            driver.FindElement(By.LinkText("Sell an item")).Click();

            // Enter the mouse details
            driver.FindElement(By.Id("ListingTitle")).SendKeys(mouseTitle);

            driver.FindElement(By.Id("ListingDescription"))
                .SendKeys("A mouse for our automation test.");

            // Choose Electronics
            driver.FindElement(
                By.XPath(
                    "//select[@id='ListingCategory']/option[text()='Electronics']"))
                .Click();

            // Enter the price
            driver.FindElement(By.Id("ListingPrice")).Clear();
            driver.FindElement(By.Id("ListingPrice")).SendKeys("75.00");

            // Choose New
            driver.FindElement(
                By.CssSelector(
                    "#ListingCondition option[value='New']")).Click();

            // Tick Available if it is not ticked
            if (!driver.FindElement(By.Id("ListingStatus")).Selected)
            {
                driver.FindElement(By.Id("ListingStatus")).Click();
            }

            // Press Enter to create the listing
            driver.FindElement(
                By.CssSelector("main button[type='submit']"))
                .SendKeys(Keys.Enter);

            Thread.Sleep(1000);

            // Check that the mouse appears
            Assert.IsTrue(
                driver.FindElement(
                    By.XPath("//h2[text()='" + mouseTitle + "']"))
                .Displayed);

            Console.WriteLine("PASS - mouse listing created.");

            // Search for items

            // Both titles contain our test name
            driver.FindElement(By.Id("search")).SendKeys(testName);

            // Click Apply
            driver.FindElement(
                By.CssSelector("main button[type='submit']")).Click();

            Thread.Sleep(1000);

            // Check that two items appear
            Assert.AreEqual(
                2,
                driver.FindElements(By.CssSelector(".card-title")).Count);

            Console.WriteLine("PASS - search found both test listings.");

            // Sort low and high

            driver.FindElement(
                By.CssSelector("#sort option[value='priceLow']")).Click();

            driver.FindElement(
                By.CssSelector("main button[type='submit']")).Click();

            Thread.Sleep(1000);

            // Get the item titles shown on the page
            var titles = driver.FindElements(By.CssSelector(".card-title"));

            // [0] means first. [1] means second
            // The book should come before the mouse
            Assert.AreEqual(2, titles.Count);
            Assert.AreEqual(bookTitle, titles[0].Text);
            Assert.AreEqual(mouseTitle, titles[1].Text);

            // Get the prices shown on the page
            var prices = driver.FindElements(
                By.CssSelector(".card-body .fw-bold"));

            Assert.AreEqual("NZ$25.00", prices[0].Text);
            Assert.AreEqual("NZ$75.00", prices[1].Text);

            Console.WriteLine("PASS - low to high price order is correct.");

            // Sort from high to low

            driver.FindElement(
                By.CssSelector("#sort option[value='priceHigh']")).Click();

            driver.FindElement(
                By.CssSelector("main button[type='submit']")).Click();

            Thread.Sleep(1000);

            // Read the titles again after sorting
            titles = driver.FindElements(By.CssSelector(".card-title"));

            // The mouse should now come first
            Assert.AreEqual(2, titles.Count);
            Assert.AreEqual(mouseTitle, titles[0].Text);
            Assert.AreEqual(bookTitle, titles[1].Text);

            prices = driver.FindElements(
                By.CssSelector(".card-body .fw-bold"));

            Assert.AreEqual("NZ$75.00", prices[0].Text);
            Assert.AreEqual("NZ$25.00", prices[1].Text);

            Console.WriteLine("PASS - high to low price order is correct.");

            // Filter to category books

            driver.FindElement(
                By.XPath(
                    "//select[@id='categoryId']/option[text()='Books']"))
                .Click();

            driver.FindElement(
                By.CssSelector("main button[type='submit']")).Click();

            Thread.Sleep(1000);

            titles = driver.FindElements(By.CssSelector(".card-title"));

            // Only our book should appear
            Assert.AreEqual(1, titles.Count);
            Assert.AreEqual(bookTitle, titles[0].Text);

            Console.WriteLine("PASS - Books filter shows only the book.");

            // Filter electronics

            driver.FindElement(
                By.XPath(
                    "//select[@id='categoryId']/option[text()='Electronics']"))
                .Click();

            driver.FindElement(
                By.CssSelector("main button[type='submit']")).Click();

            Thread.Sleep(1000);

            titles = driver.FindElements(By.CssSelector(".card-title"));

            // Only our mouse should appear
            Assert.AreEqual(1, titles.Count);
            Assert.AreEqual(mouseTitle, titles[0].Text);

            Console.WriteLine(
                "PASS - Electronics filter shows only the mouse.");

            // Search for book

            // Choose All categories
            driver.FindElement(
                By.CssSelector("#categoryId option[value='']")).Click();

            // Replace the old search with the book title
            driver.FindElement(By.Id("search")).Clear();
            driver.FindElement(By.Id("search")).SendKeys(bookTitle);

            driver.FindElement(
                By.CssSelector("main button[type='submit']")).Click();

            Thread.Sleep(1000);

            titles = driver.FindElements(By.CssSelector(".card-title"));

            // Check that only the book appears
            Assert.AreEqual(1, titles.Count);
            Assert.AreEqual(bookTitle, titles[0].Text);

            Console.WriteLine("PASS - title search shows only the book.");

            // Search for missing item

            driver.FindElement(By.Id("search")).Clear();

            driver.FindElement(By.Id("search"))
                .SendKeys(testName + " Missing");

            driver.FindElement(
                By.CssSelector("main button[type='submit']")).Click();

            Thread.Sleep(1000);

            // Check that no items appear
            Assert.AreEqual(
                0,
                driver.FindElements(By.CssSelector(".card-title")).Count);

            // Check the no-results message
            Assert.IsTrue(
                driver.FindElement(By.CssSelector(".alert-info"))
                .Text.Contains("No items found"));

            Console.WriteLine("PASS - no-match search shows no items.");

            // Open my listings

            driver.FindElement(By.LinkText("My Listings")).Click();

            // Use the item titles to find their table rows
            string bookRow =
                "//tbody/tr[td[1]='" + bookTitle + "']";

            string mouseRow =
                "//tbody/tr[td[1]='" + mouseTitle + "']";

            // Check that we have two listings
            Assert.AreEqual(
                2,
                driver.FindElements(By.CssSelector("tbody tr")).Count);

            // The third column shows Available or Sold
            Assert.AreEqual(
                "Available",
                driver.FindElement(By.XPath(bookRow + "/td[3]")).Text);

            Assert.AreEqual(
                "Available",
                driver.FindElement(By.XPath(mouseRow + "/td[3]")).Text);

            // Mark book as sold

            // Click the button in the book's row
            driver.FindElement(By.XPath(bookRow + "//button")).Click();

            // Check the message
            Assert.AreEqual(
                "Your listing was marked as sold.",
                driver.SwitchTo().Alert().Text);

            // Press OK
            driver.SwitchTo().Alert().Accept();

            Thread.Sleep(1000);

            // Check that the status changed
            Assert.AreEqual(
                "Sold",
                driver.FindElement(By.XPath(bookRow + "/td[3]")).Text);

            Console.WriteLine("PASS - book marked as sold.");

            // Mark mouse as sold

            driver.FindElement(By.XPath(mouseRow + "//button")).Click();

            Assert.AreEqual(
                "Your listing was marked as sold.",
                driver.SwitchTo().Alert().Text);

            driver.SwitchTo().Alert().Accept();

            Thread.Sleep(1000);

            Assert.AreEqual(
                "Sold",
                driver.FindElement(By.XPath(mouseRow + "/td[3]")).Text);

            Console.WriteLine("PASS - mouse marked as sold.");

            // Delete book

            driver.FindElement(By.XPath(bookRow))
                .FindElement(By.LinkText("Delete")).Click();

            // Check the item name before deleting it
            Assert.AreEqual(
                bookTitle,
                driver.FindElement(By.CssSelector("main strong")).Text);

            // Click Yes, delete
            driver.FindElement(
                By.CssSelector("main button[type='submit']")).Click();

            Assert.AreEqual(
                "Your listing was deleted.",
                driver.SwitchTo().Alert().Text);

            driver.SwitchTo().Alert().Accept();

            Thread.Sleep(1000);

            // Check that the book's row is gone
            Assert.AreEqual(
                0,
                driver.FindElements(By.XPath(bookRow)).Count);

            Console.WriteLine("PASS - book deleted.");

            // 17. DELETE THE MOUSE

            driver.FindElement(By.XPath(mouseRow))
                .FindElement(By.LinkText("Delete")).Click();

            Assert.AreEqual(
                mouseTitle,
                driver.FindElement(By.CssSelector("main strong")).Text);

            driver.FindElement(
                By.CssSelector("main button[type='submit']")).Click();

            Assert.AreEqual(
                "Your listing was deleted.",
                driver.SwitchTo().Alert().Text);

            driver.SwitchTo().Alert().Accept();

            Thread.Sleep(1000);

            // Check that My Listings is empty
            Assert.AreEqual(
                0,
                driver.FindElements(By.CssSelector("tbody tr")).Count);

            Assert.IsTrue(
                driver.FindElement(By.TagName("main"))
                .Text.Contains("You have not added any listings yet."));

            Console.WriteLine(
                "PASS - mouse deleted and My Listings is empty.");

            // Checking the marketplace

            driver.FindElement(By.LinkText("Market Place")).Click();

            driver.FindElement(By.Id("search")).SendKeys(testName);

            driver.FindElement(
                By.CssSelector("main button[type='submit']")).Click();

            // Our deleted items should not appear
            Assert.AreEqual( 0, driver.FindElements(By.CssSelector(".card-title")).Count);

            Assert.IsTrue(
                driver.FindElement(By.CssSelector(".alert-info"))
                .Text.Contains("No items found"));

            // Checking profile

            driver.FindElement(By.Id("userDetails")).Click();

            Thread.Sleep(1000);

            Assert.AreEqual("My Profile", driver.FindElement(By.TagName("h1")).Text);

            // Read the details shown on the profile
            var details = driver.FindElements(By.CssSelector("dl dd"));

            // Check the username, email and phone number
            Assert.AreEqual(3, details.Count);
            Assert.AreEqual(testName, details[0].Text);
            Assert.AreEqual(email, details[1].Text);
            Assert.AreEqual(phone, details[2].Text);

            Console.WriteLine("PASS - profile details are correct.");

            // Sign out

            driver.FindElement( By.CssSelector("main button[type='submit']")).Click();

            // Check that the Login link appears
            Assert.IsTrue(
                driver.FindElement(By.LinkText("Login")).Displayed);

            // Try opening the profile after signing out
            driver.Navigate().GoToUrl(baseUrl + "/Home/UserDetails");

            // The website should send us to Login
            Assert.AreEqual(
                "Login",
                driver.FindElement(By.TagName("h1")).Text);

            Console.WriteLine(
                "PASS - sign out worked. Full journey finished.");
        }

        // Close Chrome after the test finishes
        [TestCleanup]
        public void CloseBrowser()
        {
            driver.Quit();
        }
    }
}