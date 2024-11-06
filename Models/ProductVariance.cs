namespace E_Commerce.Models
{
    public class ProductVariance
    {
        public int ProductId { get; set; }
        public Product product { get; set; }


        public int VarianceId { get; set; }
        public Variance variance { get; set; }
    }
}
