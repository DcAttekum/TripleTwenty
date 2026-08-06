using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;
using Plugin.LocalNotification.EventArgs;
using TripleTwenty.Common;

namespace TripleTwenty.Services
{
    /// <summary>
    /// THe timer actions.
    /// </summary>
    public enum TimerAction
    {
        Started,
        Paused,
        Stopped,
        Completed
    }

    /// <summary>
    /// The timer service keeps track of the time.
    /// </summary>
    public static class TimerService
    {
        #region Properties

        /// <summary>
        /// The default long timer duration in minutes.
        /// </summary>
#if DEBUG
        public static double LongTimerDuration { get; } = 0.5;
#else
        public static double LongTimerDuration { get; } = 20;
#endif

        /// <summary>
        /// The default short timer duration in minutes.
        /// </summary>
#if DEBUG
        public static double ShortTimerDuration { get; } = 0.1;
#else
        public static double ShortTimerDuration { get; } = 0.33333;
#endif

        /// <summary>
        /// The current timer duration.
        /// </summary>
        public static double TimerDuration { get; set; } = LongTimerDuration;

        private static int _notificationId = 212;

        private static bool _initialized = false;

        #endregion

        #region Private Methods

        private static void SetNotification()
        {
            var request = new NotificationRequest
            {
                NotificationId = _notificationId,
                Title = TimerDuration == LongTimerDuration ? "Time to look away!" : "Back at it!",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.Add(GetCurrentTimeRemaining())
                }
            };

            LocalNotificationCenter.Current.Show(request);
        }

        private static void CancelNotification()
        {
            LocalNotificationCenter.Current.Cancel(_notificationId);
        }

        #endregion

        #region Public Methods

        public static void Initialize()
        {
            if (!_initialized)
            {
                _initialized = true;

                LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationActionTapped;
                LocalNotificationCenter.Current.NotificationReceived += OnNotificationReceived;
            }
        }

        public static void OnNotificationReceived(NotificationEventArgs e)
        {
            TimerDuration = TimerDuration == LongTimerDuration ? ShortTimerDuration : LongTimerDuration;
            Reset();
        }

        public static void OnNotificationActionTapped(NotificationActionEventArgs e)
        {
            if (e.IsDismissed || e.IsTapped)
            {
                Start();
            }
        }

        #region Timer

        /// <summary>
        /// Get the current remaining time on the timer.
        /// </summary>
        /// <returns>The remaining time on the timer, returns 0 if negative.</returns>
        public static TimeSpan GetCurrentTimeRemaining()
        {
            var remainingAtPause = TimeSpan.FromSeconds(Preferences.Get(PreferenceKeys.RemainingAtPause, 0d));
            var isRunning = Preferences.Get(PreferenceKeys.IsRunning, false);
            var remaining = remainingAtPause;

            if (isRunning)
            {
                var startedAtTicks = Preferences.Get(PreferenceKeys.StartedAt, 0L);
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

        /// <summary>
        /// Resets timer.
        /// </summary>
        public static void Reset()
        {
            Preferences.Set(PreferenceKeys.DurationSeconds, TimeSpan.FromMinutes(TimerDuration).TotalSeconds);
            Preferences.Set(PreferenceKeys.RemainingAtPause, TimeSpan.FromMinutes(TimerDuration).TotalSeconds);
            Preferences.Set(PreferenceKeys.IsRunning, false);
            Preferences.Set(PreferenceKeys.Completed, false);
        }

        /// <summary>
        /// Starts timer if not running or completed.
        /// </summary>
        /// <returns>Success state.</returns>
        public static bool Start()
        {
            var success = false;

            if (!Preferences.Get(PreferenceKeys.IsRunning, false) && !Preferences.Get(PreferenceKeys.Completed, false))
            {
                Preferences.Set(PreferenceKeys.StartedAt, DateTime.UtcNow.Ticks);
                Preferences.Set(PreferenceKeys.IsRunning, true);
                SetNotification();

                success = true;
            }

            return success;
        }

        /// <summary>
        /// Pauses timer if running.
        /// </summary>
        /// <returns>Success state.</returns>
        public static bool Pause()
        {
            var success = false;

            if (Preferences.Get(PreferenceKeys.IsRunning, false))
            {
                var remaining = GetCurrentTimeRemaining();

                LocalNotificationCenter.Current.Cancel(_notificationId);
                Preferences.Set(PreferenceKeys.RemainingAtPause, remaining.TotalSeconds);
                Preferences.Set(PreferenceKeys.IsRunning, false);

                success = true;
            }

            return success;
        }

        /// <summary>
        /// Stops the timer and resets it back to the default time.
        /// </summary>
        public static void Stop()
        {
            TimerDuration = LongTimerDuration;
            Reset();
            CancelNotification();
        }

        #endregion

        #endregion
    }
}
