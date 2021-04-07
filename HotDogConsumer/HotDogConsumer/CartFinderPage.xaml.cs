using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HotDogApp;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace HotDogConsumer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CartFinderPage : ContentPage
    {
        private SQLiteAsyncConnection _connection;

        private ObservableCollection<Ingredient> _ingredients = new ObservableCollection<Ingredient>();

        private ObservableCollection<Cart> _carts = new ObservableCollection<Cart>();

        public CartFinderPage()
        {
            InitializeComponent();

            ListBuilder();

            //bunPicker.ItemsSource = null;
        }

        public async void ListBuilder()
        {
            Location loc = await Geolocation.GetLastKnownLocationAsync();

            loc.Latitude += 10000;
            loc.Longitude += 50000;

            _carts.Add(new Cart { Id = Guid.NewGuid().ToString(), Identifier = "Aurelion", Vendor = "Manfred", Position = new Location(loc), CartStock = new List<Ingredient>() });

            loc.Latitude += 33333;
            loc.Longitude += 99999;

            _carts.Add(new Cart { Id = Guid.NewGuid().ToString(), Identifier = "Ahri", Vendor = "Saskia", Position = new Location(loc), CartStock = new List<Ingredient>() });
        }

        private async void OnFindClicked(object sender, EventArgs e)
        {
            Location location = await Geolocation.GetLastKnownLocationAsync();

            if (location != null)
            {
                _carts = new ObservableCollection<Cart>(_carts.OrderBy(x => x.Position.CalculateDistance(location, 0)).ToList());

                //await DisplayAlert("ok", $"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}", "ok");
            }
        }
    }
}