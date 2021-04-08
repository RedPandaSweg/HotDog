using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HotDogApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IngredientConfigPage : ContentPage
    {
        private bool _isNewIngredient = false;

        MobileServiceClient _client;
        IMobileServiceTable<Ingredient> _ingredientTable;

        public IngredientConfigPage(Ingredient ingredient)
        {
            InitializeComponent();

            _client = new MobileServiceClient(Constants.ApplicationURL);
            _ingredientTable = _client.GetTable<Ingredient>();

            typePicker.ItemsSource = new List<string> { "Bun", "Sausage", "Topping", "Sauce" };

            //Check if new ingredient
            if (ingredient.Id == null)
            {
                ingredient.Id = Guid.NewGuid().ToString();
                _isNewIngredient = true;
            };

            BindingContext = new Ingredient
            {
                Id = ingredient.Id,
                Type = ingredient.Type,
                Name = ingredient.Name,
                Cost = ingredient.Cost,
                Price = ingredient.Price,
                Stock = ingredient.Stock,
                Threshold = ingredient.Threshold,
                PriceTag = ingredient.PriceTag
            };
        }

        private async void OnSaveIngredient(object sender, EventArgs e)
        {
            Ingredient ingredient = BindingContext as Ingredient;

            // Catch bad content
            if (ingredient.Name == null)
            {
                await DisplayAlert("Error", "Please enter a name.", "OK");
                return;
            }

            ingredient.PriceTag = ingredient.Name + " (" + ingredient.Price.ToString("C") + ")";

            try
            {
                if (_isNewIngredient)
                {
                    await _ingredientTable.InsertAsync(ingredient);
                    CartConstants.OfflineIngredients.Add(ingredient);

                    foreach (Cart cart in CartConstants.OfflineCarts)
                    {
                        Inventory inventory = new Inventory
                        {
                            Id = Guid.NewGuid().ToString(),
                            CartId = cart.Id,
                            IngredientId = ingredient.Id,
                            IngredientName = ingredient.Name,
                            MaxStock = ingredient.Stock,
                            Stock = ingredient.Stock,
                            Threshold = ingredient.Threshold,
                            Resupplying = false
                        };
                        await _client.GetTable<Inventory>().InsertAsync(inventory);
                    }
                }
                else
                {
                    await _ingredientTable.UpdateAsync(ingredient);

                    var index = CartConstants.OfflineIngredients.IndexOf(CartConstants.OfflineIngredients.FirstOrDefault(x => x.Id == ingredient.Id));
                    CartConstants.OfflineIngredients[index].Name = ingredient.Name;
                    CartConstants.OfflineIngredients[index].Price = ingredient.Price;
                    CartConstants.OfflineIngredients[index].Cost = ingredient.Cost;
                    CartConstants.OfflineIngredients[index].Stock = ingredient.Stock;
                    CartConstants.OfflineIngredients[index].Threshold = ingredient.Threshold;
                    CartConstants.OfflineIngredients[index].PriceTag = ingredient.PriceTag;

                    var inventoryList = await _client.GetTable<Inventory>().Where(x => x.IngredientId == ingredient.Id).ToListAsync();
                    foreach (Inventory inventory in inventoryList)
                    {
                        inventory.IngredientName = ingredient.Name;
                        inventory.MaxStock = ingredient.Stock;
                        inventory.Threshold = ingredient.Threshold;
                        await _client.GetTable<Inventory>().UpdateAsync(inventory);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Save error: {0}", new[] { ex.Message });
            }

            await Navigation.PopAsync();
        }

    }
}