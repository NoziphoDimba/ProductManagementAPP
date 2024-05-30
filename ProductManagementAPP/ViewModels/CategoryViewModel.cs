using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

namespace ProductManagementAPP.ViewModels
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; }

        public string CategoryCode { get; set; }

        public bool IsActive { get; set; }
    }

}
