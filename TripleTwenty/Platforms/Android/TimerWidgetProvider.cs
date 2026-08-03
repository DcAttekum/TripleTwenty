using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace TripleTwenty.Platforms.Android
{
    [BroadcastReceiver(Label = "Timer Widget", Exported = true)]
    [IntentFilter(new[] {
        "android.appwidget.action.APPWIDGET_UPDATE",
        "com.TripleTwenty.TimerWidget.OPEN",
        "com.TripleTwenty.TimerWidget.PAUSE",
        "com.TripleTwenty.TimerWidget.PLAY",
        "com.TripleTwenty.TimerWidget.STOP"
    })]
    [MetaData(AppWidgetManager.MetaDataAppwidgetProvider, Resource = "@xml/timerwidget_provider_info")]
    public class TimerWidgetProvider : AppWidgetProvider
    {
        #region Actions

        const string OpenAction = "com.TripleTwenty.TimerWidget.OPEN";
        const string PauseAction = "com.TripleTwenty.TimerWidget.PAUSE";
        const string PlayAction = "com.TripleTwenty.TimerWidget.PLAY";
        const string StopAction = "com.TripleTwenty.TimerWidget.STOP";

        #endregion

        #region Private Methods

        private static PendingIntent CreatePendingIntent(Context context, string action, int requestCode)
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

            views.SetOnClickPendingIntent(Resource.Id.widgetOpen, CreatePendingIntent(context, OpenAction, 101));
            views.SetOnClickPendingIntent(Resource.Id.widgetOpen, CreatePendingIntent(context, PauseAction, 102));
            views.SetOnClickPendingIntent(Resource.Id.widgetOpen, CreatePendingIntent(context, PlayAction, 103));
            views.SetOnClickPendingIntent(Resource.Id.widgetOpen, CreatePendingIntent(context, StopAction, 104));

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
                var views = BuildRemoteViews(context);
                appWidgetManager.UpdateAppWidget(id, views);
            }
        }

        public override void OnReceive(Context? context, Intent? intent)
        {
            if (context == null || intent == null)
            {
                return;
            }

            switch (intent.Action)
            {
                case OpenAction:
                case PauseAction:
                case PlayAction:
                case StopAction:
                    break;
                default:
                    base.OnReceive(context, intent);
                    break;
            }
        }

        #endregion
    }
}
