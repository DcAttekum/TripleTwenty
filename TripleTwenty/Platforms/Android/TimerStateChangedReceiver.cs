using Android.App;
using Android.Content;
using CommunityToolkit.Mvvm.Messaging;
using TripleTwenty.Models;
using TripleTwenty.Services;

namespace TripleTwenty.Platforms.Android
{
    [BroadcastReceiver(Exported = false)]
    [IntentFilter(new[] { TimerWidgetProvider.TimerStateChangedAction })]
    public class TimerStateChangedReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            var actionValue = intent?.GetIntExtra("Action", -1) ?? -1;

            if (actionValue != -1)
            {
                var action = (TimerAction)actionValue;
                WeakReferenceMessenger.Default.Send(new TimerStateChangedMessage(action));
            }
        }
    }
}
