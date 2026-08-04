using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Widget;
using TripleTwenty.Services;

namespace TripleTwenty.Platforms.Android
{
    [BroadcastReceiver(Label = "Timer Widget", Exported = true)]
    [IntentFilter(new[] {
        "android.appwidget.action.APPWIDGET_UPDATE",
        "com.TripleTwenty.TimerWidget.PAUSE",
        "com.TripleTwenty.TimerWidget.PLAY",
        "com.TripleTwenty.TimerWidget.STOP"
    })]
    [MetaData(AppWidgetManager.MetaDataAppwidgetProvider, Resource = "@xml/timerwidget_provider_info")]
    public class TimerWidgetProvider : AppWidgetProvider
    {
        #region Actions

        public const string TimerStateChangedAction = "com.TripleTwenty.TimerWidget.TIMER_STATE_CHANGED";
        const string PauseAction = "com.TripleTwenty.TimerWidget.PAUSE";
        const string StartAction = "com.TripleTwenty.TimerWidget.START";
        const string StopAction = "com.TripleTwenty.TimerWidget.STOP";

        #endregion

        #region Private Methods

        private static void NotifyAppIfOpen(Context context, TimerAction? action)
        {
            var notify = new Intent(TimerStateChangedAction);
            notify.SetPackage(context.PackageName);
            notify.PutExtra("Action", Convert.ToInt32(action));
            context.SendBroadcast(notify);
        }

        private static PendingIntent? CreateOpenPendingIntent(Context context, int requestCode)
        {
            var packageManager = context.PackageManager;
            var openIntent = packageManager?.GetLaunchIntentForPackage(context.PackageName ?? string.Empty);
            openIntent ??= new Intent(context, typeof(MainActivity));

            openIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop);

            var flags = PendingIntentFlags.UpdateCurrent |
                (Build.VERSION.SdkInt >= BuildVersionCodes.S ? PendingIntentFlags.Immutable : 0);

            return PendingIntent.GetActivity(context, requestCode, openIntent, flags);
        }

        private static PendingIntent? CreatePendingIntent(Context context, string action, int requestCode)
        {
            var intent = new Intent(context, typeof(TimerWidgetProvider));
            intent.SetAction(action);

            var flags = PendingIntentFlags.UpdateCurrent | 
                (Build.VERSION.SdkInt >= BuildVersionCodes.S ? PendingIntentFlags.Mutable : 0);

            return PendingIntent.GetBroadcast(context, requestCode, intent, flags);
        }

        private RemoteViews BuildRemoteViews(Context context)
        {
            var views = new RemoteViews(context.PackageName, Resource.Layout.timerwidget);

            views.SetOnClickPendingIntent(Resource.Id.widgetOpen, CreateOpenPendingIntent(context, 101));
            views.SetOnClickPendingIntent(Resource.Id.widgetPause, CreatePendingIntent(context, PauseAction, 102));
            views.SetOnClickPendingIntent(Resource.Id.widgetStart, CreatePendingIntent(context, StartAction, 103));
            views.SetOnClickPendingIntent(Resource.Id.widgetStop, CreatePendingIntent(context, StopAction, 104));

            return views;
        }

        #endregion

        #region Public Methods

        public override void OnUpdate(Context? context, AppWidgetManager? appWidgetManager, int[]? appWidgetIds)
        {
            if (context == null || appWidgetIds == null || appWidgetManager == null)
            {
                return;
            }

            foreach (var id in appWidgetIds)
            {
                appWidgetManager.UpdateAppWidget(id, BuildRemoteViews(context));
            }
        }

        public override void OnReceive(Context? context, Intent? intent)
        {
            if (context != null && intent != null)
            {
                TimerAction? action = null;

                switch (intent.Action)
                {
                    case PauseAction:
                        if (TimerService.Pause())
                        {
                            action = TimerAction.Paused;
                        }
                        break;
                    case StartAction:
                        if (TimerService.Start())
                        {
                            action = TimerAction.Started;
                        }
                        break;
                    case StopAction:
                        TimerService.Stop();
                        action = TimerAction.Stopped;
                        break;
                    default:
                        base.OnReceive(context, intent);
                        break;
                }

                if (action != null)
                {
                    NotifyAppIfOpen(context, action);
                }
            }
        }

        #endregion
    }
}
