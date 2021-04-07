using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace HotDogApp
{
    public static class CartConstants
    {
        public static string CartId { get; set; }

        public static string CartName { get; set; }

        public static ObservableCollection<Ingredient> OfflineIngredients { get; set; }

        public static ObservableCollection<Cart> OfflineCarts { get; set; }
    }
}
