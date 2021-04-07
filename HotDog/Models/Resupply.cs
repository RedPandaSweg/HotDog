using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotDogApp
{
    class Resupply
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string CartId { get; set; }

        public DateTime Date { get; set; }

        public decimal Cost { get; set; }

        public string IngredientId { get; set; }
    }
}
