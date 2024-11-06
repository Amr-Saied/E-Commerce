namespace E_Commerce.Models
{

    public class Variance
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<ProductVariance> ProductVariances { get; set; }
        public ICollection<VarianceValue> VarianceValues { get; set; }

    }

}
