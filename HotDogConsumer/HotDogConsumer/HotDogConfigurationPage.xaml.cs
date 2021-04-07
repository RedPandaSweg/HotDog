using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotDogApp;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Collections.ObjectModel;

namespace HotDogConsumer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HotDogConfigurationPage : ContentPage
    {
        private HotDog _hotDog;

        private bool _isDataLoaded;
        private bool _locationNeedsUpdate;

        private decimal _price;

        public HotDogConfigurationPage(HotDog hotDog, bool locationNeedsUpdate)
        {
            InitializeComponent();

            _hotDog = hotDog;
            _locationNeedsUpdate = locationNeedsUpdate;
        }

        protected override void OnAppearing()
        {
            if (_isDataLoaded)
                return;

            _isDataLoaded = true;

            LoadData();

            base.OnAppearing();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private async void LoadData()
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, true, layoutToEnable))
            {
                if (_locationNeedsUpdate) await GetLocationAndSetCarts();
                cartPicker.ItemsSource = new ObservableCollection<Cart>(ConsumerConstants.OfflineCarts);

                //Set ItemsSource for listviews and filter by ingredient type
                bunPicker.ItemsSource = ConsumerConstants.OfflineIngredients.Where(x => x.Type.Equals("Bun")).ToList();
                sausagePicker.ItemsSource = ConsumerConstants.OfflineIngredients.Where(x => x.Type.Equals("Sausage")).ToList();
                var toppingCollection = ConsumerConstants.OfflineIngredients.Where(x => x.Type.Equals("Topping")).ToList();
                toppingPicker0.ItemsSource = toppingCollection;
                toppingPicker1.ItemsSource = toppingCollection;
                toppingPicker2.ItemsSource = toppingCollection;
                var sauceCollection = ConsumerConstants.OfflineIngredients.Where(x => x.Type.Equals("Sauce")).ToList();
                saucePicker0.ItemsSource = sauceCollection;
                saucePicker1.ItemsSource = sauceCollection;
                saucePicker2.ItemsSource = sauceCollection;

                // preset pickers
                if (ConsumerConstants.CartConstant != null) cartPicker.SelectedItem = ConsumerConstants.CartConstant;
                if (_hotDog.Bun == null)
                {
                    bunPicker.SelectedIndex = 0;
                    sausagePicker.SelectedIndex = 0;
                }
                else
                {
                    bunPicker.SelectedItem = _hotDog.Bun;
                    sausagePicker.SelectedItem = _hotDog.Sausage;
                }
                toppingPicker0.SelectedItem = _hotDog.Topping0;
                toppingPicker1.SelectedItem = _hotDog.Topping1;
                toppingPicker2.SelectedItem = _hotDog.Topping2;
                saucePicker0.SelectedItem = _hotDog.Sauce0;
                saucePicker1.SelectedItem = _hotDog.Sauce1;
                saucePicker2.SelectedItem = _hotDog.Sauce2;
            }
        }

        private async void OnAddToBasket(object sender, EventArgs e)
        {
            // catch bad content
            if (cartPicker.SelectedIndex == -1)
            {
                await DisplayAlert("Error", "Please choose a cart", "OK");
                return;
            }

            // build hot dog and add to collection
            BuildHotDog();

            App.Current.MainPage = new NavigationPage(new BasketPage());
        }

        private void OnIngredientsChanged(object sender, EventArgs e)
        {
            var extras = new List<decimal>();
            var price = new decimal();

            //add ingredient prices
            if (toppingPicker0.SelectedItem != null) extras.Add((toppingPicker0.SelectedItem as Ingredient).Price);
            if (toppingPicker1.SelectedItem != null) extras.Add((toppingPicker1.SelectedItem as Ingredient).Price);
            if (toppingPicker2.SelectedItem != null) extras.Add((toppingPicker2.SelectedItem as Ingredient).Price);
            if (saucePicker0.SelectedItem != null) extras.Add((saucePicker0.SelectedItem as Ingredient).Price);
            if (saucePicker1.SelectedItem != null) extras.Add((saucePicker1.SelectedItem as Ingredient).Price);
            if (saucePicker2.SelectedItem != null) extras.Add((saucePicker2.SelectedItem as Ingredient).Price);

            // sum ingredient prices
            if (extras.Count > 0)
            {
                foreach (var ingredient in extras)
                {
                    price += ingredient;
                }
            }

            // calculate discount
            if (extras.Count > 1)
            {
                var discount = price * Convert.ToDecimal((extras.Count - 1) * 0.1);

                price -= discount;

                discountLabel.Text = Decimal.Round(discount, 2).ToString("C") + " saved!";
                discountLabel.IsVisible = true;
            }
            else discountLabel.IsVisible = false;

            // add fixed prices
            if (bunPicker.SelectedItem != null) price += (bunPicker.SelectedItem as Ingredient).Price;
            if (sausagePicker.SelectedItem != null) price += (sausagePicker.SelectedItem as Ingredient).Price;

            _price = Decimal.Round(price, 2);

            priceLabel.Text = _price.ToString("C");
        }

        private void OnCartChanged(object sender, EventArgs e)
        {
            ConsumerConstants.CartConstant = cartPicker.SelectedItem as Cart;
        }

        private void BuildHotDog()
        {
            string id;
            bool isNewHotDog = false;
            if (_hotDog.Id == null)
            {
                id = Guid.NewGuid().ToString();
                isNewHotDog = true;
            }
            else id = _hotDog.Id;

            _hotDog = new HotDog
            {
                Id = id,
                Bun = bunPicker.SelectedItem as Ingredient,
                Sausage = sausagePicker.SelectedItem as Ingredient,
                Topping0 = toppingPicker0.SelectedItem as Ingredient,
                Topping1 = toppingPicker1.SelectedItem as Ingredient,
                Topping2 = toppingPicker2.SelectedItem as Ingredient,
                Sauce0 = saucePicker0.SelectedItem as Ingredient,
                Sauce1 = saucePicker1.SelectedItem as Ingredient,
                Sauce2 = saucePicker2.SelectedItem as Ingredient,
                Price = _price
            };
            
            if (isNewHotDog)
            {
                ConsumerConstants.HotDogCollection.Add(_hotDog);
            }
            else
            {
                ConsumerConstants.HotDogCollection.Where(x => x.Id == id).ToList();
                var index = ConsumerConstants.HotDogCollection.IndexOf(ConsumerConstants.HotDogCollection.FirstOrDefault(x => x.Id == id));
                ConsumerConstants.HotDogCollection[index] = _hotDog;
            }
        }

        private void OnGoToBasketClicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new BasketPage());
        }

        private async Task GetLocationAndSetCarts()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status == PermissionStatus.Granted)
            {
                try
                {
                    // get user location
                    Location consumerLocation = await Geolocation.GetLocationAsync();

                    // set strings for cartpicker
                    foreach (var cart in ConsumerConstants.OfflineCarts)
                    {
                        var distance = new Location(cart.Latitude, cart.Longitude).CalculateDistance(consumerLocation, 0).ToString("F1") + " km)";
                        cart.CartString = cart.Identifier + " (" + cart.Position + " - " + distance;
                    }
                }
                catch (Xamarin.Essentials.FeatureNotEnabledException)
                {
                    await DisplayAlert("Attention", "Please turn on your GPS", "Ok");
                    // set strings for cartpicker
                    foreach (var cart in ConsumerConstants.OfflineCarts)
                    {
                        cart.CartString = cart.Identifier + " - " + cart.Position;
                    }
                }
            }
            else
            {
                // set strings for cartpicker
                foreach (var cart in ConsumerConstants.OfflineCarts)
                {
                    cart.CartString = cart.Identifier + " - " + cart.Position;
                }
            }
        }

        #region Picker Resets
        private void ToppingReset0(object sender, EventArgs e)
        {
            toppingPicker0.SelectedIndex = -1;
        }
        private void ToppingReset1(object sender, EventArgs e)
        {
            toppingPicker1.SelectedIndex = -1;
        }
        private void ToppingReset2(object sender, EventArgs e)
        {
            toppingPicker2.SelectedIndex = -1;
        }
        private void SauceReset0(object sender, EventArgs e)
        {
            saucePicker0.SelectedIndex = -1;
        }
        private void SauceReset1(object sender, EventArgs e)
        {
            saucePicker1.SelectedIndex = -1;
        }
        private void SauceReset2(object sender, EventArgs e)
        {
            saucePicker2.SelectedIndex = -1;
        }
        #endregion

        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, true, layoutToEnable))
            {
                await GetLocationAndSetCarts();
                cartPicker.ItemsSource = new ObservableCollection<Cart>(ConsumerConstants.OfflineCarts);
            }
        }
    }
}