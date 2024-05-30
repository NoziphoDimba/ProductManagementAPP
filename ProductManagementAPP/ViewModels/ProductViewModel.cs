namespace ProductManagementAPP.ViewModels
{
    public class ProductViewModel
    {
        // public string ProductCode { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public int CategoryId { get; set; }
    }

}
