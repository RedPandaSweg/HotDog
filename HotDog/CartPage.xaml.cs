using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HotDogApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CartPage : ContentPage
    {
        MobileServiceClient _client;
        public CartPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            _client = new MobileServiceClient(Constants.ApplicationURL);
            cartListView.ItemsSource = await _client.GetTable<Cart>().ToCollectionAsync();

            base.OnAppearing();
        }

        private async void OnCreateClicked(object sender, EventArgs e)
        {
            var identifier = await DisplayPromptAsync("New cart", "Name the cart", "Confirm", "Cancel");

            if (identifier == null) await DisplayAlert("Error", "Please enter a name for the new cart", "Ok");
            else
            {
                // check if identifier exists
                var nameCheck = await _client.GetTable<Cart>().Where(x => x.Identifier == identifier).ToListAsync();
                if (nameCheck.Count > 0)
                {
                    await DisplayAlert("Error", "A cart with this name already exists", "Ok");
                    return;
                }

                await CreateCart(identifier);
            }
        }

        private async Task CreateCart(string identifier)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, true, layoutToEnable))
            {
                // create new cart while using current location as default
                Location location = await Geolocation.GetLocationAsync();
                var placemark = await Geocoding.GetPlacemarksAsync(location);
                var position = placemark.FirstOrDefault().Thoroughfare;
                Cart cart = new Cart
                {
                    Id = Guid.NewGuid().ToString(),
                    Identifier = identifier,
                    Longitude = location.Longitude,
                    Latitude = location.Latitude,
                    Position = position
                };
                await _client.GetTable<Cart>().InsertAsync(cart);
                cartListView.ItemsSource = await _client.GetTable<Cart>().OrderBy(x => x.Identifier).ToCollectionAsync();

                // creating fully stocked inventory and insert to db
                var ingredientList = await _client.GetTable<Ingredient>().ToListAsync();
                foreach (Ingredient ingredient in ingredientList)
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
                CartConstants.OfflineCarts = await _client.GetTable<Cart>().ToCollectionAsync();
                await DisplayAlert("Success", "New cart " + cart.Identifier + " successfully created", "Ok");
            }
        }
    }
}