using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.Generic;

namespace BBScore
{
	public class App : Application
	{
		public App ()
		{
			DataResource.InitFetchData ();
			GameSetupPage gameSetupPage = new GameSetupPage();
			DataResourceAccess dra = new DataResourceAccess ();
			gameSetupPage.BindingContext = dra;
			MainPage = new NavigationPage (gameSetupPage);
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

