using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Assignment3_Group.Controllers;
using Assignment3_Group.Data;
using Assignment3_Group.Models;

namespace StudentMarketplaceTestProject
{
    [TestClass]
    public sealed class Test1
    {
        //Creating in-memory database from student application 
        private StudentMarketplaceDB GetDbContext()
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
        //For this test case, i want to test creating an account, checking account details, then creating a listing in market place
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
