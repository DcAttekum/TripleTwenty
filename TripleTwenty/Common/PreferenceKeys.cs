namespace TripleTwenty.Common
{
    /// <summary>
    /// Holds the preference keys used throughout the app.
    /// </summary>
    public static class PreferenceKeys
    {
        /// <summary>
        /// Indicates if the timer is running.
        /// </summary>
        public static string IsRunning = "Timer.IsRunning";

        /// <summary>
        /// Indicates the time the timer is set to.
        /// </summary>
        public static string DurationSeconds = "Timer.DurationSeconds";

        /// <summary>
        /// Indicates the time remaining when pausing.
        /// </summary>
        public static string RemainingAtPause = "Timer.RemainingAtPause";

        /// <summary>
        /// Indicates the time the timer was started at.
        /// </summary>
        public static string StartedAt = "Timer.StartedAt";

        /// <summary>
        /// Indicates the timer having completed.
        /// </summary>
        public static string Completed = "Timer.Completed";
    }
}
