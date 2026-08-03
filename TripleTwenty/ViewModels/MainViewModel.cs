using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using TripleTwenty.Common;
using TripleTwenty.Models;
using TripleTwenty.Services;

namespace TripleTwenty.ViewModels
{
    /// <summary>
    /// The main view model.
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        #region Constants

        const int DefaultTimerDuration = 20; // In minutes

        #endregion

        #region Properties

        /// <summary>
        /// The time remaining in mm:ss format for the front end.
        /// </summary>
        public string Time => TimerService.GetCurrentTimeRemaining().ToString(@"mm\:ss");

        private Timer? uiRefreshTimer { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MainViewModel()
        {
            // Handles messages from widget.
            WeakReferenceMessenger.Default.Register<TimerStateChangedMessage>(this, (recipient, message) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    switch (message.Action)
                    {
                        case TimerAction.Started:
                            if (uiRefreshTimer == null)
                            {
                                StartUIRefreshTimer();
                            }
                            break;
                        case TimerAction.Paused:
                        case TimerAction.Stopped:
                        case TimerAction.Completed:
                            uiRefreshTimer?.Dispose();
                            uiRefreshTimer = null;
                            break;
                    }

                    RaiseTimeChanged();
                });
            });

            // Sets duration if setting isn't available.
            if (!Preferences.ContainsKey(PreferenceKeys.DurationSeconds))
            {
                SetDuration(TimeSpan.FromMinutes(DefaultTimerDuration));
            }

            // Starts ui refresh if timer was started while app was not open.
            if (Preferences.ContainsKey(PreferenceKeys.IsRunning) && Preferences.Get(PreferenceKeys.IsRunning, false))
            {
                StartUIRefreshTimer();
            }
        }

        #endregion

        #region Private Methods

        private void SetDuration(TimeSpan duration)
        {
            Preferences.Set(PreferenceKeys.DurationSeconds, duration.TotalSeconds);
            Preferences.Set(PreferenceKeys.RemainingAtPause, duration.TotalSeconds);
            Preferences.Set(PreferenceKeys.IsRunning, false);
            Preferences.Set(PreferenceKeys.Completed, false);

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
                var remaining = TimerService.GetCurrentTimeRemaining();
                RaiseTimeChanged();

                if (remaining <= TimeSpan.Zero && !Preferences.Get(PreferenceKeys.Completed, false))
                {
                    Preferences.Set(PreferenceKeys.Completed, true);
                    Preferences.Set(PreferenceKeys.IsRunning, false);

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
            if (TimerService.Start())
            {
                StartUIRefreshTimer();
                RaiseTimeChanged();
            }
        }

        [RelayCommand]
        private void ClickPauseTimer()
        {
            if (TimerService.Pause())
            {
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

            TimerService.Stop();
            RaiseTimeChanged();
        }

        #endregion

        #region PropertyChanged

        public void RaiseTimeChanged() =>
            MainThread.BeginInvokeOnMainThread(() => OnPropertyChanged(nameof(Time)));

        #endregion
    }
}
