using Microsoft.Azure.Mobile.Server;
using System;

namespace BackendService.DataObjects
{
    public class Resupply : EntityData
    {
        public string CartId { get; set; }

        public DateTime Date { get; set; }

        public decimal Cost { get; set; }

        public string IngredientId { get; set; }
    }
}
