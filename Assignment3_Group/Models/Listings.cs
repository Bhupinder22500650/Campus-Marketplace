using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment3_Group.Models
{
    public class Listings
    {
        [Key]
        public int ListingId { get; set; }

        // This is the student who added the item.
        [Range(1, int.MaxValue)]
        public int SellerId { get; set; }

        [Display(Name = "Category")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
        public int ListingCategory { get; set; }

        [Required(ErrorMessage = "Please enter a title.")]
        [StringLength(100)]
        [Display(Name = "Title")]
        public string ListingTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a description.")]
        [StringLength(1000)]
        [Display(Name = "Description")]
        public string ListingDescription { get; set; } = string.Empty;

        // This lets the price include cents.
        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0.01", "99999999.99",
            ErrorMessage = "Enter a price between $0.01 and $99,999,999.99.")]
        [Display(Name = "Price (NZD)")]
        public decimal ListingPrice { get; set; }

        [Required]
        [RegularExpression("New|Used", ErrorMessage = "Select New or Used.")]
        [Display(Name = "Condition")]
        public string ListingCondition { get; set; } = "Used";

        // True means the item is for sale. False means it is sold.
        [Display(Name = "Available for sale")]
        public bool ListingStatus { get; set; } = true;

        public DateTime ListingDate { get; set; } = DateTime.Now;

        // The student can leave this email box empty.
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(254, ErrorMessage = "The email address is too long.")]
        [Display(Name = "Student email (optional)")]
        public string? ContactEmail { get; set; }

        // The student can leave this phone box empty.
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(30, ErrorMessage = "The phone number is too long.")]
        [Display(Name = "Phone number (optional)")]
        public string? ContactPhoneNumber { get; set; }

        // The picture is saved in the images folder.
        // We only keep its file name here.
        [StringLength(100)]
        [RegularExpression(@"^[A-Za-z0-9][A-Za-z0-9_-]*\.(jpg|jpeg|png|webp)$",
            ErrorMessage = "Use a simple file name, such as textbook.jpg.")]
        [Display(Name = "Image (optional)")]
        public string? ImageFileName { get; set; }

        // Keep this old field so we do not remove old data.
        public byte? ProductImage { get; set; }

        // These let us find the seller and the category for this item.
        [ForeignKey("SellerId")]
        public User? Seller { get; set; }

        [ForeignKey("ListingCategory")]
        public Category? Category { get; set; }
    }
}
