using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Forms.Platform.Android;

using BBScore;
using BBScore.Messages;
using Xamarin.Forms;

namespace BBScore.Droid
{
	[Activity (Label = "BBScore.Droid", Icon = "@drawable/icon", MainLauncher = true,
		ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		Intent intent;
		PowerManager.WakeLock wakeLock;
		protected override void OnCreate (Bundle bundle)
		{
			PowerManager pm = (PowerManager)GetSystemService (Context.PowerService);
			wakeLock = pm.NewWakeLock (WakeLockFlags.Partial, "bbs wakelock");
			wakeLock.Acquire ();
			intent = new Intent(this, typeof(LongRunningTaskService));
			MessagingCenter.Subscribe<StartLongRunningTaskMessage> (this, "StartLongRunningTaskMessage", message => {
				StartService(intent);
			});
			MessagingCenter.Subscribe<StopLongRunningTaskMessage> (this, "StopLongRunningTaskMessage", message => {
//				var intent = new Intent(this, typeof(LongRunningTaskService));
				StopService(intent);
			});
			base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);

			LoadApplication (new App ());
		}
		protected override void OnDestroy ()
		{
			wakeLock.Release ();
			base.OnDestroy ();
		}
	}
}

