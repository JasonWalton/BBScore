using System;

using Xamarin.Forms;

namespace BBScore
{
	[ContentProperty("Child")]
	public class OnPlatformView : StackLayout
	{
		private OnPlatform<View> _child;
		public OnPlatform<View> Child
		{
			get { return _child; }
			set {
				if (_child != value) {
					if (_child != null)
						Children.Remove (_child);
					_child = value;
					if (_child != null)
						Children.Add (_child);
				}
			}
		}
	}
}


