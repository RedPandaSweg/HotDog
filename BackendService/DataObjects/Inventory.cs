using Microsoft.Azure.Mobile.Server;

namespace BackendService.DataObjects
{
    public class Inventory : EntityData
    {
        public string CartId { get; set; }

        public string IngredientId { get; set; }

        public string IngredientName { get; set; }

        public int MaxStock { get; set; }

        public int Stock { get; set; }

        public int Threshold { get; set; }

        public bool Resupplying { get; set; }
    }
}
