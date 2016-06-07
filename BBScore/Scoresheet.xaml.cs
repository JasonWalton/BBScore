using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace BBScore
{
	public partial class Scoresheet : ContentPage
	{
		public Scoresheet ()
		{
			this.BindingContext = new Game ();
			InitializeComponent ();
			Tossups.ItemSelected += (sender, e) => {
				((ListView)sender).SelectedItem = null;
			};
		}

		protected override void OnAppearing()
		{
			base.OnAppearing ();
			Game test = NavigationMessage.GetData<Game>("ActiveGame");
			this.BindingContext = test;
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing ();
			NavigationMessage.PutData ("ViewScoresheet", 0);
		}
	}
}

