using Xamarin.Forms;
using System.Threading.Tasks;
using System.Threading;
using BBScore.Messages;
using System;

namespace BBScore.Messages
{
	public class StartLongRunningTaskMessage {}
	public class StopLongRunningTaskMessage {}
	public class TickedMessage
	{
		public string Message { get; set; }
	}
//	public class CancelledMessage {}
}
namespace BBScore
{
	public class GameTimer
	{
		public TimerState _timerState { get; set; }
		bool run = false;
		public GameTimer()
		{
		}
		public async Task Start(CancellationToken token)
		{
			run = true;
			await Task.Run (async () => {
				while (run) {
					token.ThrowIfCancellationRequested ();

					await Task.Delay(1000);
					var message = new TickedMessage();

					Device.BeginInvokeOnMainThread(() => {
						MessagingCenter.Send<TickedMessage>(message, "TickedMessage");
					});
				}
			}
			, token);
		}
		public void Stop()
		{
			run = false;
		}
	}
}