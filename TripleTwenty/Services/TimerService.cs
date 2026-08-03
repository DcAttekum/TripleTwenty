using TripleTwenty.Common;

namespace TripleTwenty.Services
{
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
            Preferences.Set(PreferenceKeys.DurationSeconds, TimeSpan.FromMinutes(Defaults.TimerDuration).TotalSeconds);
            Preferences.Set(PreferenceKeys.RemainingAtPause, TimeSpan.FromMinutes(Defaults.TimerDuration).TotalSeconds);
            Preferences.Set(PreferenceKeys.IsRunning, false);
            Preferences.Set(PreferenceKeys.Completed, false);
        }
    }
}
