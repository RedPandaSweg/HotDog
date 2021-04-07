using Newtonsoft.Json;

namespace HotDogApp
{
    public class Cart
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string Identifier { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Position { get; set; }
        
        public string CartString { get; set; }
    }
}
