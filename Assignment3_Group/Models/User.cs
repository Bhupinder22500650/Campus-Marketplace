using System.ComponentModel.DataAnnotations;
namespace Assignment3_Group.Models
{
    public class User
    {
        //Making the primary key for users be the user id
        [Key]
        public int UserId {  get; set; }

        //Making required data be the users name, password and email (needed for communication)
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        //Making it so you don't have to have the phone number in the database
        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
