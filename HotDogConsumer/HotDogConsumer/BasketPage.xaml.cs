using HotDogApp;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HotDogConsumer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BasketPage : ContentPage
    {
        MobileServiceClient _client;

        private bool _orderSent;

        decimal _price = 0;

        public BasketPage()
        {
            InitializeComponent();

            _client = new MobileServiceClient(Constants.ApplicationURL);

            orderListView.ItemsSource = ConsumerConstants.HotDogCollection;

            UpdatePrice();
        }

        // calculate and show price
        private void UpdatePrice()
        {
            _price = 0;
            foreach (var hotdog in ConsumerConstants.HotDogCollection)
            {
                _price += hotdog.Price;
            }
            priceLabel.Text = " " + _price.ToString("C") + " ";
        }

        private async void OnHotDogSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var response = await DisplayActionSheet("What do you want to do?", "Cancel", "", "Change ingredients", "Remove hot dog");

            switch (response)
            {
                case "Change ingredients":
                    await Navigation.PushAsync(new HotDogConfigurationPage(e.SelectedItem as HotDog, false));
                    break;

                case "Remove hot dog":
                    ConsumerConstants.HotDogCollection.Remove(e.SelectedItem as HotDog);
                    orderListView.ItemsSource = ConsumerConstants.HotDogCollection;
                    UpdatePrice();
                    break;
            };
        }

        private async void OnSendOrder(object sender, EventArgs e)
        {
            if (ConsumerConstants.HotDogCollection.Count == 0)
            {
                await DisplayAlert("Error", "Please add a hot dog to your order", "Ok");
                return;
            }

            var orderTable = _client.GetTable<Order>();
            // get latest ordernumber and increment it
            var orderNumberList = await orderTable.Where(x => x.CartId == ConsumerConstants.CartConstant.Id).ToListAsync();
            orderNumberList = orderNumberList.OrderByDescending(y => y.OrderNumber).ToList();
            int orderNumber;
            if (orderNumberList.Count > 0) orderNumber = orderNumberList.Max(x => x.OrderNumber) + 1;
            else orderNumber = 1;
            // create and insert new order
            Order order = new Order
            {
                Id = Guid.NewGuid().ToString(),
                CartId = ConsumerConstants.CartConstant.Id,
                OrderNumber = orderNumber,
                OrderDate = DateTime.UtcNow.Date,
                Price = _price,
                Fulfilled = false
            };
            await orderTable.InsertAsync(order);

            // insert order ID and upload hot dogs
            var hotDogTable = _client.GetTable<HotDogSQL>();
            foreach (var hotDog in ConsumerConstants.HotDogCollection)
            {
                hotDog.Order = order.Id;
                var hotdogsql = ConversionToSQL(hotDog);
                await hotDogTable.InsertAsync(hotdogsql);
            }

            // Change Page after sending
            _orderSent = true;

            orderButton.Text = "Order sent!";
            orderButton.IsEnabled = false;
            addButton.IsEnabled = false;
            orderListView.IsEnabled = false;
            orderNumberLayout.IsVisible = true;
            orderNumberLabel.Text = orderNumber.ToString();
            cartProofLabel.Text = ConsumerConstants.CartConstant.Identifier + " @" + ConsumerConstants.CartConstant.Position;
        }

        public HotDogSQL ConversionToSQL(HotDog hotDog)
        {
            string sauce0 = null;
            if (hotDog.Sauce0 != null) sauce0 = hotDog.Sauce0.Id;
            string sauce1 = null;
            if (hotDog.Sauce1 != null) sauce1 = hotDog.Sauce1.Id;
            string sauce2 = null;
            if (hotDog.Sauce2 != null) sauce2 = hotDog.Sauce2.Id;
            string topping0 = null;
            if (hotDog.Topping0 != null) topping0 = hotDog.Topping0.Id;
            string topping1 = null;
            if (hotDog.Topping1 != null) topping1 = hotDog.Topping1.Id;
            string topping2 = null;
            if (hotDog.Topping2 != null) topping2 = hotDog.Topping2.Id;

            HotDogSQL hotDogSQL = new HotDogSQL
            {
                Id = hotDog.Id,
                Bun = hotDog.Bun.Id,
                Sausage = hotDog.Sausage.Id,
                Sauce0 = sauce0,
                Sauce1 = sauce1,
                Sauce2 = sauce2,
                Topping0 = topping0,
                Topping1 = topping1,
                Topping2 = topping2,
                Price = hotDog.Price,
                Order = hotDog.Order
            };
            return hotDogSQL;
        }

        protected override bool OnBackButtonPressed()
        {
            if (_orderSent) return true;

            Navigation.PushAsync(new HotDogConfigurationPage(new HotDog(), false));
            return true;
        }

        private void OnAddHotDog(object sender, EventArgs e)
        {
            Navigation.PushAsync(new HotDogConfigurationPage(new HotDog(), false));
        }
    }
}