using TripleTwenty.Services;

namespace TripleTwenty.Models
{
    /// <summary>
    /// Used as a signal to tell the rest of the code something changed externally.
    /// </summary>
    public class TimerStateChangedMessage
    {
        /// <summary>
        /// The action that was called.
        /// </summary>
        public TimerAction Action { get; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="action">The action that was called.</param>
        public TimerStateChangedMessage(TimerAction action)
        {
            Action = action;
        }
    }
}
