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
    public partial class MainPage : ContentPage
    {
        private IMobileServiceClient _client;
        private bool _isDataLoaded;
        
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            if (!_isDataLoaded)
            {
                _client = new MobileServiceClient(Constants.ApplicationURL);
                await RefreshItems(true);
                _isDataLoaded = true;
            }
            cartPicker.ItemsSource = CartConstants.OfflineCarts;

            base.OnAppearing();
        }

        #region Cart
        private void OnCartClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CartPage());
        }

        private void OnCartChanged(object sender, EventArgs e)
        {
            ordersButton.IsVisible = true;
            ingredientsButton.IsVisible = true;
            inventoryButton.IsVisible = true;
            accountingButton.IsVisible = true;
            cartButton.IsVisible = true;
            locationButton.IsVisible = true;
            CartConstants.CartId = ((sender as Picker).SelectedItem as Cart).Id;
            CartConstants.CartName = ((sender as Picker).SelectedItem as Cart).Identifier;
        }

        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, true, layoutToEnable))
            {
                CartConstants.OfflineCarts = await _client.GetTable<Cart>().OrderBy(x => x.Identifier).ToCollectionAsync();
            }
            cartPicker.ItemsSource = CartConstants.OfflineCarts;
        }
        #endregion

        private void OnIngredientsClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ShowIngredientsPage());
        }

        private void OnOrdersClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new IncomingOrderPage());
        }

        private void OnAccountingClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FinancialsPage());
        }

        private void OnInventoryClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new InventoryPage());
        }

        private async void OnUpdateLocationClicked(object sender, EventArgs e)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, true, layoutToEnable))
            {
                var cartList = await _client.GetTable<Cart>().Where(x => x.Id == CartConstants.CartId).ToListAsync();
                Cart cart = cartList[0];
                Location location = await Geolocation.GetLocationAsync();
                var placemark = await Geocoding.GetPlacemarksAsync(location);
                var position = placemark.FirstOrDefault().Thoroughfare;
                cart.Longitude = location.Longitude;
                cart.Latitude = location.Latitude;
                cart.Position = position;

                await _client.GetTable<Cart>().UpdateAsync(cart);

                var index = CartConstants.OfflineCarts.IndexOf(CartConstants.OfflineCarts.FirstOrDefault(x => x.Id == cart.Id));
                CartConstants.OfflineCarts[index].Latitude = cart.Latitude;
                CartConstants.OfflineCarts[index].Longitude = cart.Longitude;
                CartConstants.OfflineCarts[index].Position = cart.Position;
            }
        }

        private async Task RefreshItems(bool showActivityIndicator)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator, layoutToEnable))
            {
                CartConstants.OfflineIngredients = await _client.GetTable<Ingredient>().ToCollectionAsync();
                CartConstants.OfflineCarts = await _client.GetTable<Cart>().OrderBy(x => x.Identifier).ToCollectionAsync();
            }
        }
    }
}