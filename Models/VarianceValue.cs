using Microsoft.Identity.Client;

namespace E_Commerce.Models
{
    public class VarianceValue
    {
        public int VarianceId { get; set; }
        public Variance variance { get; set; }

        public int ValueId { get; set; }
        public Value Value { get; set; }
    }
}
