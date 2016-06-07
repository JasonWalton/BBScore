using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Xamarin.Forms;
using BBScore;
using BBScore.Messages;

namespace BBScore.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		iOSLongRunningTaskExample timerTask = null;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{

			global::Xamarin.Forms.Forms.Init ();
			LoadApplication (new App ());
			WireUpTimerTask ();
//			Chilkat.Ftp2 ftp = new Chilkat.Ftp2 ();
//			bool success = ftp.UnlockComponent("KEITHSFTP_SKKP0Jm59JnA");
//			if (success) {
//				ftp.Hostname = "ftp://speedtest.tele2.net";// "ftp.biblebowl.net";
//				ftp.Username = ""; //"0055844|softuser";
//				ftp.Password = ""; // "softpass";
//				success = ftp.Connect ();
//			}

			return base.FinishedLaunching (app, options);
		}

		private void WireUpTimerTask()
		{
			MessagingCenter.Subscribe<StartLongRunningTaskMessage> (this, "StartLongRunningTaskMessage", async message => {
				timerTask = new iOSLongRunningTaskExample ();
				await timerTask.Start ();
			});

			MessagingCenter.Subscribe<StopLongRunningTaskMessage> (this, "StopLongRunningTaskMessage", message => {
				if (timerTask != null)
					timerTask.Stop ();
			});
//			MessagingCenter.Subscribe<CancelledMessage> (this, "CancelledMessage", message => {
//				if (timerTask != null)
//					timerTask.Stop ();
//			});
		}
	}
}

