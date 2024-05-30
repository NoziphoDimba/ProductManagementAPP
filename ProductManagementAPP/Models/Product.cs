using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProductManagementAPP.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public string UserId { get; set; } // Foreign key for ApplicationUser
        public ApplicationUser User { get; set; }
    }
}
