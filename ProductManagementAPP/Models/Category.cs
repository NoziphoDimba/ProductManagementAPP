using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProductManagementAPP.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^[A-Z]{3}\d{3}$", ErrorMessage = "Category code must be in format ABC123.")]
        public string CategoryCode { get; set; }
        public bool IsActive { get; set; }
        // Navigation property for related products
      //  public ICollection<Product> Products { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public string UserId { get; set; } // Foreign key for ApplicationUser
        public ApplicationUser User { get; set; }
    }
}
