using Newtonsoft.Json;

namespace HotDogApp
{
    public class Inventory
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string CartId { get; set; }

        public string IngredientId { get; set; }

        public string IngredientName { get; set; }

        public int MaxStock { get; set; }

        public int Stock { get; set; }

        public int Threshold { get; set; }

        public bool Resupplying { get; set; }
    }
}
