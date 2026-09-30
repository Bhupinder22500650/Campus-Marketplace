using System.ComponentModel.DataAnnotations;
namespace Assignment3_Group.Models
{
    public class User
    {
        //Making the primary key for users be the user id
        [Key]
        public int UserId { get; set; }

        //Making required data be the users name, password and email (needed for communication)
        //Making it so username, password, and email are empty to create parameterless User so database can work
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string email { get; set; } = string.Empty;

        //Making it so you don't have to have the phone number in the database
        [Phone]
        public string? PhoneNumber { get; set; }

        public User()
        {

        }

        //creating constructor for user object in current session
        public User(int ID, string userName, string password, string Email, string? phoneNumber)
        {
            UserId = ID;

            UserName = userName;

            Password = password;

            email = Email;

            PhoneNumber = phoneNumber;
        }
    } 
}
