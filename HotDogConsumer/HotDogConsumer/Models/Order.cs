using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace HotDogApp
{
    public class Order
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public int OrderNumber { get; set; }

        public string CartId { get; set; }

        public decimal Price { get; set; }

        public bool Fulfilled { get; set; }

        public DateTime OrderDate { get; set; }
    }
}
