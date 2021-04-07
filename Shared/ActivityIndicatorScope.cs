using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HotDogApp
{
    public class ActivityIndicatorScope : IDisposable
    {
        private bool _showIndicator;
        private ActivityIndicator _indicator;
        private Task _indicatorDelay;
        private View _layoutToEnable;

        public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator,View layoutToEnable)
        {
            _indicator = indicator;
            _showIndicator = showIndicator;
            _layoutToEnable = layoutToEnable;

            if (showIndicator)
            {
                _indicatorDelay = Task.Delay(500);
                SetIndicatorActivity(true);
            }
            else
            {
                _indicatorDelay = Task.FromResult(0);
            }
        }

        private void SetIndicatorActivity(bool isActive)
        {
            _indicator.IsVisible = isActive;
            _indicator.IsRunning = isActive;
            if (_layoutToEnable!= null) _layoutToEnable.IsEnabled = !isActive;
        }

        public void Dispose()
        {
            if (_showIndicator)
            {
                _indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
    }
}
