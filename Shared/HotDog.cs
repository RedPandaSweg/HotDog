using Newtonsoft.Json;

namespace HotDogApp
{
    public class HotDog
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public Ingredient Bun { get; set; }

        public Ingredient Sausage { get; set; }

        public Ingredient Topping0 { get; set; }

        public Ingredient Topping1 { get; set; }

        public Ingredient Topping2 { get; set; }

        public Ingredient Sauce0 { get; set; }

        public Ingredient Sauce1 { get; set; }

        public Ingredient Sauce2 { get; set; }

        public decimal Price { get; set; }

        public string Order { get; set; }
    }
}
