namespace E_Commerce.DTO
{
    public class ProductDTO
    {
        public string Name { get; set; }
        public decimal Price { get; set; } // base price if necessary (but this is likely handled per ProductItem)
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public List<VariationDTO> Variations { get; set; } // Variations with price and option details
    }

    public class VariationDTO
    {
        public int VariationId { get; set; }
        public List<int> VariationOptionIds { get; set; }
        public decimal Price { get; set; } // Price specific to the product item variation
        public int QtyInStock { get; set; }
        public string ProductImage { get; set; } // Optional product image for this variation
    }
}
