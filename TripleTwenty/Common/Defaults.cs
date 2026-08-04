namespace TripleTwenty.Common
{
    /// <summary>
    /// Contains default values used throughout the app.
    /// </summary>
    public static class Defaults
    {
        /// <summary>
        /// The default timer duration in minutes.
        /// </summary>
#if DEBUG
        public static double TimerDuration { get; } = 0.5;
#else
        public static double TimerDuration { get; } = 20;
#endif
    }
}
