namespace E_Commerce.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Optional self-referencing relationship for nested categories
        public int? ParentCategoryId { get; set; }
        public Category ParentCategory { get; set; }

        public ICollection<Category> ChildCategories { get; set; }

        // Navigation property for related products
        public ICollection<Product> Products { get; set; }
    }
}
