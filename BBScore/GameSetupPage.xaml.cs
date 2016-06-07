using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.ComponentModel;

namespace BBScore
{
	public partial class GameSetupPage : ContentPage
	{
		public GameSetupPage ()
		{
			InitializeComponent ();
		}
		protected override void OnAppearing()
		{
			base.OnAppearing ();
			((DataResourceAccess)this.BindingContext).GameInProgress = (NavigationMessage.GetData<bool> ("GameInProgress") == true);

		}
	}
}

