using Microsoft.Azure.Mobile.Server;

namespace BackendService.DataObjects
{
    public class HotDogSQL : EntityData
    {
        public string Bun { get; set; }

        public string Sausage { get; set; }

        public string Topping0 { get; set; }
        public string Topping1 { get; set; }
        public string Topping2 { get; set; }

        public string Sauce0 { get; set; }
        public string Sauce1 { get; set; }
        public string Sauce2 { get; set; }

        public decimal Price { get; set; }

        public string Order { get; set; }
    }
}
