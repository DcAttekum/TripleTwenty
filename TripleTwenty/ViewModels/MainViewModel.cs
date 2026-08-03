using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace TripleTwenty.ViewModels
{
    /// <summary>
    /// The main view model.
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        #region Constants

        const string IsRunning = "Timer.IsRunning";
        const string DurationSeconds = "Timer.DurationSeconds";
        const string RemainingAtPause = "Timer.RemainingAtPause";
        const string StartedAt = "Timer.StartedAt";
        const string Completed = "Timer.Completed";

        const int DefaultTimerDuration = 20; // In minutes

        #endregion

        #region Properties

        public string Time => GetCurrentTimeRemaining().ToString(@"mm\:ss");
        private TimeSpan GetCurrentTimeRemaining()
        {
            var remainingAtPause = TimeSpan.FromSeconds(Preferences.Get(RemainingAtPause, 0d));
            var isRunning = Preferences.Get(IsRunning, false);
            var remaining = remainingAtPause;

            if (isRunning)
            {
                var startedAtTicks = Preferences.Get(StartedAt, 0L);
                var startedAt = new DateTime(startedAtTicks, DateTimeKind.Utc);
                var elapsedSinceStart = DateTime.UtcNow - startedAt;

                remaining = remainingAtPause - elapsedSinceStart;
                if (remaining < TimeSpan.Zero)
                {
                    remaining = TimeSpan.Zero;
                }
            }

            return remaining;
        }

        private Timer? uiRefreshTimer { get; set; }

        #endregion

        #region Constructors

        public MainViewModel()
        {
            SetDuration(TimeSpan.FromMinutes(DefaultTimerDuration));
        }

        #endregion

        #region Private Methods

        private void SetDuration(TimeSpan duration)
        {
            Preferences.Set(DurationSeconds, duration.TotalSeconds);
            Preferences.Set(RemainingAtPause, duration.TotalSeconds);
            Preferences.Set(IsRunning, false);
            Preferences.Set(Completed, false);

            RaiseTimeChanged();
        }

        private void OnCompleted()
        {
            throw new NotImplementedException();
        }

        private void StartUIRefreshTimer()
        {
            uiRefreshTimer?.Dispose();
            uiRefreshTimer = new Timer(_ =>
            {
                var remaining = GetCurrentTimeRemaining();
                RaiseTimeChanged();

                if (remaining <= TimeSpan.Zero && !Preferences.Get(Completed, false))
                {
                    Preferences.Set(Completed, true);
                    Preferences.Set(IsRunning, false);

                    uiRefreshTimer?.Dispose();
                    uiRefreshTimer = null;

                    MainThread.BeginInvokeOnMainThread(OnCompleted);
                }
            }, null, TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500));
        }

        #endregion

        #region Commands

        [RelayCommand]
        private void ClickStartTimer()
        {
            if (!Preferences.Get(IsRunning, false) && !Preferences.Get(Completed, false))
            {
                Preferences.Set(StartedAt, DateTime.UtcNow.Ticks);
                Preferences.Set(IsRunning, true);

                StartUIRefreshTimer();
                RaiseTimeChanged();
            }
        }

        [RelayCommand]
        private void ClickPauseTimer()
        {
            if (Preferences.Get(IsRunning, false))
            {
                var remaining = GetCurrentTimeRemaining();
                Preferences.Set(RemainingAtPause, remaining.TotalSeconds);
                Preferences.Set(IsRunning, false);

                uiRefreshTimer?.Dispose();
                uiRefreshTimer = null;
                RaiseTimeChanged();
            }
        }

        [RelayCommand]
        private void ClickStopTimer()
        {
            uiRefreshTimer?.Dispose();
            uiRefreshTimer = null;
            SetDuration(TimeSpan.FromMinutes(DefaultTimerDuration));
        }

        #endregion

        #region PropertyChanged

        public void RaiseTimeChanged() =>
            MainThread.BeginInvokeOnMainThread(() => OnPropertyChanged(nameof(Time)));

        #endregion
    }
}
