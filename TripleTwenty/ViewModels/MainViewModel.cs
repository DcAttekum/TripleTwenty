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
    public partial class MainViewModel : ObservableObject, IDisposable
    {
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
                TimerService.TimerDuration = TimerService.LongTimerDuration;
                TimerService.Reset();
                RaiseTimeChanged();
            }

            // Starts ui refresh if timer was started while app was not open.
            if (Preferences.ContainsKey(PreferenceKeys.IsRunning) && Preferences.Get(PreferenceKeys.IsRunning, false))
            {
                StartUIRefreshTimer();
            }
        }

        #endregion

        #region Private Methods

        private void StartUIRefreshTimer()
        {
            uiRefreshTimer?.Dispose();
            uiRefreshTimer = new Timer(_ =>
            {
                RaiseTimeChanged();
            }, null, TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500));
        }

        #endregion

        #region Commands

        [RelayCommand]
        private void StartTimer()
        {
            if (TimerService.Start())
            {
                StartUIRefreshTimer();
                RaiseTimeChanged();
            }
        }

        [RelayCommand]
        private void PauseTimer()
        {
            if (TimerService.Pause())
            {
                RaiseTimeChanged();
            }
        }

        [RelayCommand]
        private void StopTimer()
        {
            TimerService.Stop();
            RaiseTimeChanged();
        }

        #endregion

        #region PropertyChanged

        public void RaiseTimeChanged() =>
            MainThread.BeginInvokeOnMainThread(() => OnPropertyChanged(nameof(Time)));

        #endregion

        #region IDisposable

        public void Dispose()
        {
            uiRefreshTimer?.Dispose();
            uiRefreshTimer = null;
        }

        #endregion
    }
}
