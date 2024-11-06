namespace E_Commerce.Models
{
    public class Value
    {
        public int Id { get; set; }

        public string Option { get; set; }

        public ICollection<VarianceValue> VarianceValues { get; set; }
    }
}
