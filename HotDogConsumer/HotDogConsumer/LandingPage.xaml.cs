using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using HotDogApp;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace HotDogConsumer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LandingPage : ContentPage
    {
        MobileServiceClient _client;
        public LandingPage()
        {
            Image image = new Image { Source = ImageSource.FromResource("HotDogConsumer.Images.ddlogo.png") };

            InitializeComponent();

            logoImage.Source = image.Source;
        }

        protected override async void OnAppearing()
        {
            _client = new MobileServiceClient(Constants.ApplicationURL);

            await CheckAndRequestLocationPermission();

            await RefreshItems(true);

            App.Current.MainPage = new HotDogConfigurationPage(new HotDog(),true);

            base.OnAppearing();
        }

        public async Task<PermissionStatus> CheckAndRequestLocationPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status == PermissionStatus.Granted)
                return status;

            if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            {
                await DisplayAlert("Attention", "Please allow the use of the device location in the iOS settings.", "Ok");
                return status;
            }

            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            return status;
        }

        private async Task RefreshItems(bool showActivityIndicator)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator, null))
            {
                ConsumerConstants.OfflineIngredients = await _client.GetTable<Ingredient>().ToListAsync();
                ConsumerConstants.OfflineCarts = await _client.GetTable<Cart>().OrderBy(x => x.Identifier).ToListAsync();
            }
        }
    }
}