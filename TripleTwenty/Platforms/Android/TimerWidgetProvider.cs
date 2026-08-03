using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;

namespace TripleTwenty.Platforms.Android
{
    [BroadcastReceiver(Label = "Timer Widget", Exported = true)]
    [IntentFilter(new[] {"android.appwidget.action.APPWIDGET_UPDATE"})]
    [MetaData(AppWidgetManager.MetaDataAppwidgetProvider, Resource = "@xml/timerwidget_provider_info")]
    public class TimerWidgetProvider : AppWidgetProvider
    {
        public override void OnUpdate(Context? context, AppWidgetManager? appWidgetManager, int[]? appWidgetIds)
        {
            if (context == null || appWidgetIds == null || appWidgetManager == null)
            {
                return;
            }

            foreach (var id in appWidgetIds)
            {
                var views = new RemoteViews(context.PackageName, Resource.Layout.timerwidget);
                views.SetTextViewText(Resource.Id.widgetText, "Some text for testing");
                appWidgetManager.UpdateAppWidget(id, views);
            }
        }
    }
}
