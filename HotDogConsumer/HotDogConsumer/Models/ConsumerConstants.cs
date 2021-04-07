using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace HotDogApp
{
    public static class ConsumerConstants
    {
        public static Cart CartConstant;
        public static ObservableCollection<HotDog> HotDogCollection = new ObservableCollection<HotDog>();
        public static List<Ingredient> OfflineIngredients;
        public static List<Cart> OfflineCarts;
        public static bool LocationIsLive;
    }
}
