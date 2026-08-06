using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Plugin.LocalNotification;

namespace TripleTwenty
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _ = EnsurePermissionAsync();
        }

        private async Task EnsurePermissionAsync()
        {
            var notificationsAllowed = await LocalNotificationCenter.Current.AreNotificationsEnabled();
            if (!notificationsAllowed)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }

            EnsureExactAlarmPermission();
        }

        private void EnsureExactAlarmPermission()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                var alarmManager = (AlarmManager?)GetSystemService(AlarmService);

                if (alarmManager != null && !alarmManager.CanScheduleExactAlarms())
                {
                    var intent = new Intent(Settings.ActionRequestScheduleExactAlarm);
                    intent.SetData(global::Android.Net.Uri.Parse($"package:{PackageName}"));
                    intent.AddFlags(ActivityFlags.NewTask);
                    StartActivity(intent);
                }
            }
        }
    }
}
