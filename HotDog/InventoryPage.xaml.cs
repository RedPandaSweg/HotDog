using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HotDogApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventoryPage : ContentPage
    {
        MobileServiceClient _client;
        ObservableCollection<Ingredient> _missingIngredients;
        ObservableCollection<Inventory> _inventories;
        public InventoryPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            _client = new MobileServiceClient(Constants.ApplicationURL);

            // create list with missing ingredients and set it to picker source
            _missingIngredients = new ObservableCollection<Ingredient>(CartConstants.OfflineIngredients);
            _inventories = await _client.GetTable<Inventory>().Where(x => x.CartId == CartConstants.CartId).ToCollectionAsync();
            inventoryListView.ItemsSource = _inventories;
            foreach (var inventory in _inventories)
            {
                var test = _missingIngredients.FirstOrDefault(x => x.Id == inventory.IngredientId);
                if (test != null)
                {
                    _missingIngredients.Remove(test);
                }
            }
            newInventoryPicker.ItemsSource = _missingIngredients;
            if (_missingIngredients.Count > 0)
            {
                newInventoryPicker.IsVisible = true;
                createInventoryButton.IsVisible = true;
            }

            base.OnAppearing();
        }

        private async void OnInventorySelected(object sender, SelectedItemChangedEventArgs e)
        {
            // resupply an inventory with delivered ingredients
            var inventory = e.SelectedItem as Inventory;
            var response = await DisplayAlert(inventory.IngredientName, "Resupply " + inventory.IngredientName + "?", "Yes", "Cancel");

            if (response)
            {
                inventory.Stock = inventory.MaxStock;
                inventory.Resupplying = false;
                using (var scope = new ActivityIndicatorScope(syncIndicator, true, layoutToEnable))
                {
                    await _client.GetTable<Inventory>().UpdateAsync(inventory);
                    _inventories = await _client.GetTable<Inventory>().Where(x => x.CartId == CartConstants.CartId).ToCollectionAsync();
                    inventoryListView.ItemsSource = _inventories;
                }
            }
        }

        private async void OnCreateInventoryClicked(object sender, EventArgs e)
        {
            if (newInventoryPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Error", "Please select an ingredient to stock", "Ok");
                return;
            }

            await CreateInventory();
        }

        private async Task CreateInventory()
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, true, layoutToEnable))
            {
                // create new inventory and insert in db
                var ingredient = (newInventoryPicker.SelectedItem as Ingredient);
                Inventory inventory = new Inventory
                {
                    Id = Guid.NewGuid().ToString(),
                    CartId = CartConstants.CartId,
                    IngredientId = ingredient.Id,
                    IngredientName = ingredient.Name,
                    MaxStock = ingredient.Stock,
                    Stock = ingredient.Stock,
                    Threshold = ingredient.Threshold,
                    Resupplying = false
                };
                await _client.GetTable<Inventory>().InsertAsync(inventory);

                _inventories.Add(inventory);
                _inventories = new ObservableCollection<Inventory>(_inventories.OrderBy(x => x.IngredientName));
                inventoryListView.ItemsSource = _inventories;

                // check if there are any missing ingredients left
                _missingIngredients.Remove(ingredient);
                if (_missingIngredients.Count == 0)
                {
                    newInventoryPicker.IsVisible = false;
                    createInventoryButton.IsVisible = false;
                    return;
                }
                newInventoryPicker.ItemsSource = _missingIngredients;
                newInventoryPicker.SelectedIndex = 0;
            }
        }
    }
}