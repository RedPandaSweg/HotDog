using Newtonsoft.Json;
using System.ComponentModel;

namespace HotDogApp
{
    public class Ingredient : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string Type { get; set; }

        private string _name;
        public string Name
        {
			get { return _name; }
            set
			{
				if (_name == value) return;

				_name = value;

				OnPropertyChanged(nameof(Name));
            }
		}

        private int _stock;
        public int Stock
        {
			get { return _stock; }
            set
			{
				if (_stock == value) return;

                _stock = value;

				OnPropertyChanged(nameof(Stock));
            }
		}

        private int _threshold;
        public int Threshold
        {
            get { return _threshold; }
            set
            {
                if (_threshold == value) return;

                _threshold = value;

                OnPropertyChanged(nameof(Threshold));
            }
        }

        private decimal _price;
        public decimal Price
        {
            get { return _price; }
            set
            {
                _price = value;

                OnPropertyChanged(nameof(Price));
            }
        }

        private decimal _cost;
        public decimal Cost
        {
            get { return _cost; }
            set
            {
                _cost = value;

                OnPropertyChanged(nameof(Cost));
            }
        }

        public string PriceTag { get; set; }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
