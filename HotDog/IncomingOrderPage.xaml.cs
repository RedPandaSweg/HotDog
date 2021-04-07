using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HotDogApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IncomingOrderPage : ContentPage
    {
        MobileServiceClient _client;

        public ObservableCollection<HotDog> _hotDogCollection;
        public ObservableCollection<Order> _orderList;
        
        public IncomingOrderPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            _client = new MobileServiceClient(Constants.ApplicationURL);

            GetOrders();

            base.OnAppearing();
        }

        public async Task GetOrders()
        {
            // retrieve orders with own cart ID, that are not fulfilled and sort by order number
            _orderList = await _client.GetTable<Order>().Where(x => x.CartId == CartConstants.CartId && x.Fulfilled == false).OrderBy(y => y.OrderNumber).ToCollectionAsync();

            // retrieve sql hot dogs by order, convert to normal hot dogs and set itemssource
            _hotDogCollection = new ObservableCollection<HotDog>();
            foreach (var order in _orderList)
            {
                var hotDogTable = await _client.GetTable<HotDogSQL>().Where(x => x.Order == order.Id).ToListAsync();

                var hotDogCollection = new ObservableCollection<HotDog>();

                foreach (var hotDog in hotDogTable)
                {
                    hotDogCollection.Add(ConversionToHotDog(hotDog));
                }

                order.HotDogCollection = hotDogCollection;
            }

            orderListView.ItemsSource = _orderList;
        }

        public static HotDog ConversionToHotDog(HotDogSQL hotDogSQL)
        {
            Ingredient bun = CartConstants.OfflineIngredients.FirstOrDefault(x => x.Id == hotDogSQL.Bun);
            Ingredient sausage = CartConstants.OfflineIngredients.FirstOrDefault(x => x.Id == hotDogSQL.Sausage);
            Ingredient sauce0 = null;
            if (hotDogSQL.Sauce0 != null) sauce0 = CartConstants.OfflineIngredients.FirstOrDefault(x => x.Id == hotDogSQL.Sauce0);
            Ingredient sauce1 = null;
            if (hotDogSQL.Sauce1 != null) sauce1 = CartConstants.OfflineIngredients.FirstOrDefault(x => x.Id == hotDogSQL.Sauce1);
            Ingredient sauce2 = null;
            if (hotDogSQL.Sauce2 != null) sauce2 = CartConstants.OfflineIngredients.FirstOrDefault(x => x.Id == hotDogSQL.Sauce2);
            Ingredient topping0 = null;
            if (hotDogSQL.Topping0 != null) topping0 = CartConstants.OfflineIngredients.FirstOrDefault(x => x.Id == hotDogSQL.Topping0);
            Ingredient topping1 = null;
            if (hotDogSQL.Topping1 != null) topping1 = CartConstants.OfflineIngredients.FirstOrDefault(x => x.Id == hotDogSQL.Topping1);
            Ingredient topping2 = null;
            if (hotDogSQL.Topping2 != null) topping2 = CartConstants.OfflineIngredients.FirstOrDefault(x => x.Id == hotDogSQL.Topping2);

            HotDog hotDog = new HotDog
            {
                Id = hotDogSQL.Id,
                Bun = bun,
                Sausage = sausage,
                Sauce0 = sauce0,
                Sauce1 = sauce1,
                Sauce2 = sauce2,
                Topping0 = topping0,
                Topping1 = topping1,
                Topping2 = topping2,
                Price = hotDogSQL.Price,
                Order = hotDogSQL.Order
            };

            return hotDog;
        }

        #region Hot Dog finished

        private async void OnOrderSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var response = await DisplayAlert("Finish Order", "Order fulfilled?", "Yes", "No");
            if (response)
            {
                using (var scope = new ActivityIndicatorScope(syncIndicator, true, layoutToEnable))
                {
                    await OrderProcess(e.SelectedItem as Order);

                    await GetOrders();
                }
            }
        }

        private async Task OrderProcess(Order order)
        {
            await InventoryUpdater(order);

            var hotDogsToRemove = await _client.GetTable<HotDogSQL>().Where(x => x.Order == order.Id).ToListAsync();
            foreach (HotDogSQL hotdog in hotDogsToRemove)
            {
                try
                {
                    await _client.GetTable<HotDogSQL>().DeleteAsync(hotdog);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message.ToString());
                }
            }
            order.Fulfilled = true;
            try
            {
                await _client.GetTable<Order>().UpdateAsync(order);
            } catch ( Exception ex )
            {
                throw new Exception(ex.Message.ToString());
            }
            
        }

        private async Task InventoryUpdater(Order order)
        {
            var inventoryList = await _client.GetTable<Inventory>().Where(x => x.CartId == CartConstants.CartId).ToListAsync();

            foreach (HotDog hotDog in order.HotDogCollection)
            {
                await CheckIngredient(inventoryList, hotDog.Bun);
                await CheckIngredient(inventoryList, hotDog.Sausage);
                if (hotDog.Sauce0 != null) await CheckIngredient(inventoryList, hotDog.Sauce0);
                if (hotDog.Sauce1 != null) await CheckIngredient(inventoryList, hotDog.Sauce1);
                if (hotDog.Sauce2 != null) await CheckIngredient(inventoryList, hotDog.Sauce2);
                if (hotDog.Topping0 != null) await CheckIngredient(inventoryList, hotDog.Topping0);
                if (hotDog.Topping1 != null) await CheckIngredient(inventoryList, hotDog.Topping1);
                if (hotDog.Topping2 != null) await CheckIngredient(inventoryList, hotDog.Topping2);
            }
        }

        private async Task CheckIngredient(List<Inventory> inventoryList, Ingredient ingredient)
        {
            var inventoryItem = inventoryList.FirstOrDefault(x => x.IngredientId == ingredient.Id);
            await UpdateInventory(inventoryItem, ingredient);
        }

        // lower stock by one, update inventory db and check stock threshold - send mail to resupply if needed and create resupply entry
        private async Task UpdateInventory(Inventory item, Ingredient ingredient)
        {
            item.Stock -= 1;
            
            if (item.Stock <= item.Threshold && item.Resupplying == false)
            {
                item.Resupplying = SendMail(ingredient);
                Resupply resupply = new Resupply
                {
                    Id = Guid.NewGuid().ToString(),
                    CartId = CartConstants.CartId,
                    Date = DateTime.UtcNow.Date,
                    Cost = (ingredient.Stock - ingredient.Threshold) * ingredient.Cost,
                    IngredientId = ingredient.Id
                };
                await _client.GetTable<Resupply>().InsertAsync(resupply);
            }

            await _client.GetTable<Inventory>().UpdateAsync(item);
        }

        private bool SendMail(Ingredient ingredient)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("hotdogiubh@gmail.com");
                mail.To.Add("hotdogiubh@gmail.com");
                mail.Subject = "Order for " + ingredient.Name + " to " + CartConstants.CartName;
                mail.Body = "Please send " + (ingredient.Stock - ingredient.Threshold) + "servings of " + ingredient.Name + " to " + CartConstants.CartName;

                SmtpServer.Port = 587;
                SmtpServer.Host = "smtp.gmail.com";
                SmtpServer.EnableSsl = true;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("hotdogiubh@gmail.com", "hotdog#1");

                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                DisplayAlert("Failed", ex.Message, "OK");
                return false;
            }
        }
        #endregion

        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, true, layoutToEnable))
            {
                await GetOrders();
            }
        }
    }
}