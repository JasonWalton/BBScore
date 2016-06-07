using System;
using Android.App;
using System.Threading;
using Android.OS;
using Android.Content;
using System.Threading.Tasks;
using Xamarin.Forms;
using BBScore.Messages;

namespace BBScore.Droid
{
	[Service]
	public class LongRunningTaskService : Service  
	{
		CancellationTokenSource _cts;

		public override IBinder OnBind (Intent intent)
		{
			return null;
		}

		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			_cts = new CancellationTokenSource ();

			Task.Run (() => {
				try {
					//INVOKE THE SHARED CODE
					var counter = new GameTimer();
					counter.Start(_cts.Token).Wait();
				}
				catch (System.OperationCanceledException) {
				}
				finally {
					if (_cts.IsCancellationRequested) {
//						var message = new CancelledMessage();
//						Device.BeginInvokeOnMainThread (
//							() => MessagingCenter.Send(message, "CancelledMessage")
//						);
					}
				}

			}, _cts.Token);

			return StartCommandResult.Sticky;
		}

		public override void OnDestroy ()
		{
			if (_cts != null) {
				_cts.Token.ThrowIfCancellationRequested ();

				_cts.Cancel ();
			}
			base.OnDestroy ();
		}
	}
}

