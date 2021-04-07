using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HotDogApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FinancialsPage : ContentPage
    {
        private IMobileServiceClient _client;
        private IMobileServiceTable<Order> _orders;
        private IMobileServiceTable<Resupply> _resupplies;

        private bool showProfits;

        public FinancialsPage()
        {
            InitializeComponent();

            _client = new MobileServiceClient(Constants.ApplicationURL);
            _orders = _client.GetTable<Order>();
            _resupplies = _client.GetTable<Resupply>();
        }

        private void OnSwitchButtonClicked(object sender, EventArgs e)
        {
            if (showProfits)
            {
                headerLabel.Text = "Sales";
                showProfits = false;
            }
            else
            {
                headerLabel.Text = "Profits";
                showProfits = true;
            }
        }

        private async void OnTodayCartClicked(object sender, EventArgs e)
        {
            var compareDate = DateTime.UtcNow.Date;
            var orderList = await _orders.Where(x => x.CartId == CartConstants.CartId && x.OrderDate == compareDate && x.Fulfilled == true).ToListAsync();
            List<Resupply> resupplyList = new List<Resupply>();
            if (showProfits) resupplyList = await _resupplies.Where(x => x.CartId == CartConstants.CartId && x.Date == compareDate).ToListAsync();
            CalculateFinancials(orderList, resupplyList);
        }

        private async void OnWeekCartClicked(object sender, EventArgs e)
        {
            var compareDate = DateTime.UtcNow.AddDays(-7).Date;
            var orderList = await _orders.Where(x => x.CartId == CartConstants.CartId && x.OrderDate >= compareDate && x.Fulfilled == true).ToListAsync();
            List<Resupply> resupplyList = new List<Resupply>();
            if (showProfits) resupplyList = await _resupplies.Where(x => x.CartId == CartConstants.CartId && x.Date >= compareDate).ToListAsync();
            CalculateFinancials(orderList, resupplyList);
        }

        private async void OnMonthCartClicked(object sender, EventArgs e)
        {
            var compareDate = DateTime.UtcNow.AddMonths(-1).Date;
            var orderList = await _orders.Where(x => x.CartId == CartConstants.CartId && x.OrderDate >= compareDate && x.Fulfilled == true).ToListAsync();
            List<Resupply> resupplyList = new List<Resupply>();
            if (showProfits) resupplyList = await _resupplies.Where(x => x.CartId == CartConstants.CartId && x.Date >= compareDate).ToListAsync();
            CalculateFinancials(orderList, resupplyList);
        }

        private async void OnTodayAllClicked(object sender, EventArgs e)
        {
            var compareDate = DateTime.UtcNow.Date;
            var orderList = await _orders.Where(x => x.OrderDate == compareDate && x.Fulfilled == true).ToListAsync();
            List<Resupply> resupplyList = new List<Resupply>();
            if (showProfits) resupplyList = await _resupplies.Where(x => x.Date == compareDate).ToListAsync();
            CalculateFinancials(orderList, resupplyList);
        }

        private async void OnWeekAllClicked(object sender, EventArgs e)
        {
            var compareDate = DateTime.UtcNow.AddDays(-7).Date;
            var orderList = await _orders.Where(x => x.OrderDate >= compareDate && x.Fulfilled == true).ToListAsync();
            List<Resupply> resupplyList = new List<Resupply>();
            if (showProfits) resupplyList = await _resupplies.Where(x => x.Date >= compareDate).ToListAsync();
            CalculateFinancials(orderList, resupplyList);
        }

        private async void OnMonthAllClicked(object sender, EventArgs e)
        {
            var compareDate = DateTime.UtcNow.AddMonths(-1).Date;
            var orderList = await _orders.Where(x => x.OrderDate >= compareDate && x.Fulfilled == true).ToListAsync();
            List<Resupply> resupplyList = new List<Resupply>();
            if (showProfits) resupplyList = await _resupplies.Where(x => x.Date >= compareDate).ToListAsync();
            CalculateFinancials(orderList, resupplyList);
        }

        private void CalculateFinancials(List<Order> orderList, List<Resupply> resupplyList)
        {
            // calculate prices
            var prices = new decimal(0);
            foreach (var order in orderList)
            {
                prices += order.Price;
            }

            // calcate costs
            var costs = new decimal(0);
            foreach (var resupply in resupplyList)
            {
                costs += resupply.Cost;
            }

            decimal profit = prices - costs;

            profitLabel.Text = profit.ToString("C");

            if (profit > 0) profitLabel.TextColor = Color.LimeGreen;
            if (profit < 0) profitLabel.TextColor = Color.Red;
            if (profit == 0) profitLabel.TextColor = Color.Black;
        }

    }
}