using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TripleTwenty.ViewModels
{
    /// <summary>
    /// The main view model.
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        #region Properties

        [ObservableProperty]
        private string timeLeft;

        private TimeSpan _time;
        private TimeSpan Time
        {
            get { return _time; }
            set
            {
                _time = value;
                TimeLeft = value.ToString(@"mm\:ss");
            }
        }

        private Timer? Timer { get; set; }

        #endregion

        #region Constructors

        public MainViewModel()
        {
            TimeLeft = string.Empty;
            Time = TimeSpan.FromMinutes(20);
        }

        #endregion

        #region Private Methods

        private void StartTimer()
        {
            if (this.Timer == null)
            {
                this.Timer = new Timer(obj =>
                {
                    if (!Time.Equals(TimeSpan.Zero))
                    {
                        Time = Time.Subtract(TimeSpan.FromSeconds(1));
                    }
                }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            }
        }

        #endregion

        #region Commands

        [RelayCommand]
        private void ClickStartTimer()
        {
            StartTimer();
        }

        [RelayCommand]
        private void ClickPauseTimer()
        {
            if (this.Timer != null)
            {
                this.Timer?.Dispose();
                this.Timer = null;
            }
            else
            {
                StartTimer();
            }
        }

        [RelayCommand]
        private void ClickStopTimer()
        {
            this.Timer?.Dispose();
            this.Timer = null;
            Time = TimeSpan.FromMinutes(20);
        }

        #endregion
    }
}
