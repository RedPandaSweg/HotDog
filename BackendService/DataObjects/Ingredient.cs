using Microsoft.Azure.Mobile.Server;

namespace BackendService.DataObjects
{
    public class Ingredient : EntityData
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public int Stock { get; set; }

        public int Threshold { get; set; }

        public decimal Price { get; set; }

        public decimal Cost { get; set; }

        public string PriceTag { get; set; }
    }
}
