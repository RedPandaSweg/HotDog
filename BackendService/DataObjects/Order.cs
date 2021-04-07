using Microsoft.Azure.Mobile.Server;
using System;

namespace BackendService.DataObjects
{
    public class Order : EntityData
    {
        public string CartId { get; set; }

        public int OrderNumber { get; set; }

        public decimal Price { get; set; }

        public bool Fulfilled { get; set; }

        public DateTime OrderDate { get; set; }
    }
}
