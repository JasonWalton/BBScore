using System;
using BBScore;
using System.Threading;
using UIKit;
using BBScore.Messages;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace BBScore.iOS
{
	public class iOSLongRunningTaskExample  
	{
		nint _taskId;
		CancellationTokenSource _cts;

		public async Task Start ()
		{
			_cts = new CancellationTokenSource ();

			_taskId = UIApplication.SharedApplication.BeginBackgroundTask ("LongRunningTask", OnExpiration);

			try {
				//INVOKE THE SHARED CODE
				var counter = new GameTimer();
				await counter.Start(_cts.Token);

			} catch (OperationCanceledException) {
			} finally {
				if (_cts.IsCancellationRequested) {
					var message = new CancelledMessage();
//					Device.BeginInvokeOnMainThread (
//						() => MessagingCenter.Send(message, "CancelledMessage")
//					);
				}
			}

			UIApplication.SharedApplication.EndBackgroundTask (_taskId);
		}

		public void Stop ()
		{
			UIApplication.SharedApplication.EndBackgroundTask (_taskId);
			_cts.Cancel ();
		}

		void OnExpiration ()
		{
			_cts.Cancel ();
		}
	}
}

