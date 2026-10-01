using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment3_Group.Models
{
    public class Listings
    {
        [Key]
        public int ListingId { get; set; }

        // The controller gets this ID from the signed-in user's session.
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

        // Decimal allows prices such as $12.50.
        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0.01", "99999999.99",
            ErrorMessage = "Enter a price between $0.01 and $99,999,999.99.")]
        [Display(Name = "Price (NZD)")]
        public decimal ListingPrice { get; set; }

        [Required]
        [RegularExpression("New|Used", ErrorMessage = "Select New or Used.")]
        [Display(Name = "Condition")]
        public string ListingCondition { get; set; } = "Used";

        // True means available. False means sold.
        [Display(Name = "Available for sale")]
        public bool ListingStatus { get; set; } = true;

        public DateTime ListingDate { get; set; } = DateTime.Now;

        // For this first version, copy the image into wwwroot/images yourself.
        // Store only its file name, for example textbook.jpg.
        [StringLength(100)]
        [RegularExpression(@"^[A-Za-z0-9][A-Za-z0-9_-]*\.(jpg|jpeg|png|webp)$",
            ErrorMessage = "Use a simple file name, such as textbook.jpg.")]
        [Display(Name = "Image file name (optional)")]
        public string? ImageFileName { get; set; }

        // Keep the old column so this change does not delete existing data.
        // A single byte cannot store a photograph. The feed does not use it.
        public byte? ProductImage { get; set; }

        // These properties connect each listing to its user and category.
        [ForeignKey("SellerId")]
        public User? Seller { get; set; }

        [ForeignKey("ListingCategory")]
        public Category? Category { get; set; }
    }
}
