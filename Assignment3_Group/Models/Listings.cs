using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment3_Group.Models
{
    public class Listings
    {
        //Making the primary key for users be the user id
        [Key]
        public int ListingId { get; set; }

        //Creating a foreign key of the UserId in User.cs and naming is SellerId
        [ForeignKey("UserId")]
        public int SellerId { get; set; }

        //Creating as foreign key of the category
        [ForeignKey("CategoryName")]
        public int ListingCategory { get; set; }

        //Making the price and status a requirement of the listing
        [Required]
        public int ListingPrice { get; set; }

        [Required]
        public bool ListingStatus { get; set; }
    }
}
