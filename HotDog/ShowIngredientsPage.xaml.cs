using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HotDogApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShowIngredientsPage : ContentPage
    {        
        private bool _isDataLoaded;

        MobileServiceClient _client;
        IMobileServiceTable<Ingredient> _ingredientTable;

        public ShowIngredientsPage()
        {
            InitializeComponent();

            _client = new MobileServiceClient(Constants.ApplicationURL);

            _ingredientTable = _client.GetTable<Ingredient>();
        }

        protected override async void OnAppearing()
        {
            if (_isDataLoaded)

                // refresh ingredient collection
                try
                {
                    ingredientsListView.ItemsSource = await _ingredientTable.OrderBy(x => x.Type).ToCollectionAsync();
                }
                catch (MobileServiceInvalidOperationException msioe)
                {
                    Debug.WriteLine("Invalid sync operation: {0}", new[] { msioe.Message });
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Sync error: {0}", new[] { e.Message });
                }

            _isDataLoaded = true;

            await LoadData();

            base.OnAppearing();
        }

        private async Task LoadData()
        {
            // load ingredient collection
            var ingredients = await _ingredientTable.OrderBy(x => x.Type).ToCollectionAsync();

            CartConstants.OfflineIngredients = new ObservableCollection<Ingredient>(ingredients);

            ingredientsListView.ItemsSource = CartConstants.OfflineIngredients;
        }

        private async void OnAddIngredient(object sender, EventArgs e)
        {
            var page = new IngredientConfigPage(new Ingredient());

            await Navigation.PushAsync(page);
        }

        async void OnIngredientSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (ingredientsListView.SelectedItem == null)
                return;

            var selectedIngredient = e.SelectedItem as Ingredient;

            ingredientsListView.SelectedItem = null;

            await Navigation.PushAsync(new IngredientConfigPage(selectedIngredient));
        }

    }
}