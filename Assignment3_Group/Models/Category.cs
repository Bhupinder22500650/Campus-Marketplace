using System.ComponentModel.DataAnnotations;

namespace Assignment3_Group.Models
{
    public class Category
    {
        //Making the primary key for users be the user id
        [Key]
        public int CategoryId { get; set; }

        //Making required data the category name and description
        [Required]
        public string CategoryName { get; set; }

        [Required]
        public string CategoryDescription { get; set; }
    }
}
