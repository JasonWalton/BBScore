using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace BBScore
{
	public partial class GamePlay : ContentPage
	{
		public GamePlay ()
		{
			InitializeComponent ();
		}

		public GamePlay(Game g)
		{
			InitializeComponent ();
			this.BindingContext = g;
		}

		protected override void OnAppearing ()
		{
			base.OnAppearing ();
//			//for a new game, don't reload the active game data
//			if (NavigationMessage.GetData<int> ("NewGame") == 1) {
//				NavigationMessage.PutData<int> ("NewGame", 0);
//				return;
//			}
//			Game activeGame = NavigationMessage.GetData<Game>("ActiveGame");
//			if (activeGame != null) {
//				if (NavigationMessage.GetData<int> ("ViewScoresheet") == 0) {
//					NavigationMessage.PutData<int> ("ViewScoresheet", 1);
//					return;
//				}
//				Game gameWithNewSettings = (Game)this.BindingContext;
//				gameWithNewSettings.UpdateGameData (activeGame);
//				this.BindingContext = gameWithNewSettings;
//			}
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing ();
			NavigationMessage.PutData ("ActiveGame", this.BindingContext);
		}

//		protected override bool OnBackButtonPressed()
//		{
//			base.OnBackButtonPressed ();
////			NavigationMessage.PutData ("ActiveGame", this.BindingContext);
////			((Game)this.BindingContext).ResetCommand.Execute (WhichPlayer.None);
//			return true;
//		}
	}
}

