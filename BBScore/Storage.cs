using System;
using System.IO;
using Xamarin.Forms;
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel;
using BBScore.Messages;

namespace BBScore
{
	public class DataResourceAccess : INotifyPropertyChanged
	{
		public string TeamName { get { return DataResource.TeamName; } set { DataResource.TeamName = value; } }
		public string Opponent { get { return DataResource.Opponent; } set { DataResource.Opponent = value; } }
		public string Player1 { get { return DataResource.Player1; }  set { DataResource.Player1 = value; } }
		public string Player2 { get { return DataResource.Player2; }  set { DataResource.Player2 = value; } }
		public string Player3 { get { return DataResource.Player3; }  set { DataResource.Player3 = value; } }
		public string Player4 { get { return DataResource.Player4; }  set { DataResource.Player4 = value; } }
		public bool GType { get { return DataResource.GType == GameType.DoubleElim; } 
			set { DataResource.GType = ( value ? GameType.DoubleElim : GameType.RoundRobin); } }
		private bool _gameInProgress = false;
		public bool GameInProgress { get { return _gameInProgress; }
			set { _gameInProgress = value;
				PropertyChanged (this, new PropertyChangedEventArgs("GameInProgress"));
			} 
		} //{ get { return NavigationMessage.KeyExists ("ActiveGame"); } }
		public DelegateCommand ResumeGameCommand { get; private set; }
		public DelegateCommand NewGameCommand { get; private set; }
		public event PropertyChangedEventHandler PropertyChanged;

		public DataResourceAccess()
		{
			ResumeGameCommand = new DelegateCommand (ExecuteResumeGame);
			NewGameCommand = new DelegateCommand (ExecuteNewGame);
		}

		public void ExecuteResumeGame(object nothing)
		{
			DataResource.SaveEntries ();
			App.Current.MainPage.Navigation.PushAsync (new GamePlay(NavigationMessage.GetData<Game> ("ActiveGame")));
		}

		public void ExecuteNewGame(object nothing)
		{
			DataResource.SaveEntries ();
			MessagingCenter.Send (new StopLongRunningTaskMessage (), "StopLongRunningTaskMessage");
			NavigationMessage.PutData<bool> ("GameInProgress", false);
			App.Current.MainPage.Navigation.PushAsync (new GamePlay ());
		}
	}
	public static class DataResource
	{
		public static string TeamName { get; set; }
		public static string Opponent { get; set; }
		public static string Player1 { get; set; }
		public static string Player2 { get; set; }
		public static string Player3 { get; set; }
		public static string Player4 { get; set; }
		public static BBScore.GameType GType { get; set; }
		public static void InitFetchData()
		{
			//first attempt to load from file
			string data;
			try {
				data = DependencyService.Get<ISaveAndLoad>().LoadText("BBScoreData.txt");
			}
			catch (FileNotFoundException e) {
				data = DataResource.LoadFromEmbeddedResource();
			}
			ParseData (data);
		}
		public static void ParseData(string data)
		{
			string[] values= data.Split ('\n');
			TeamName = values [0];
			Opponent = values [1];
			Player1 = values [2];
			Player2 = values [3];
			Player3 = values [4];
			Player4 = values [5];
			GType = values [6] == "RR" ? GameType.RoundRobin : GameType.DoubleElim;
		}
		private static string LoadFromEmbeddedResource()
		{
			var assembly = typeof(DataResource).GetTypeInfo().Assembly;
			Stream stream = assembly.GetManifestResourceStream("BBScore.GameData.txt");
			using (var reader = new System.IO.StreamReader (stream)) {
				return reader.ReadToEnd ();
			}
		}
		public static void SaveEntries()
		{
			string data = TeamName + "\n" +
				Opponent + "\n" +
				Player1 + "\n" +
				Player2 + "\n" +
				Player3 + "\n" +
				Player4 + "\n" +
				(GType == GameType.RoundRobin ? "RR" : "DE") + "\n";
				//save entries
				Task setDataTask = DependencyService.Get<ISaveAndLoad> ().SaveTextAsync ("BBScoreData.txt", data);
//				//reload names from entries for use in gameplay
//				ParseData (data);
		}
//		public static void SaveEntries(Entry[] entries, bool gameTypeToggled)
//		{
//			string data = "";
//
//			//save text entries
//			foreach (Entry e in entries) {
//				data = data + e.Text + "\n";
//			}
//			//append RR/DE setting
//			DataResource.GType = (gameTypeToggled ? GameType.DoubleElim : GameType.RoundRobin);
//			data += (gameTypeToggled ? "DE" : "RR") + "\n";
//
//			//save entries
//			Task setDataTask = DependencyService.Get<ISaveAndLoad> ().SaveTextAsync ("BBScoreData.txt", data);
//			//reload names from entries for use in gameplay
//			ParseData (data);
//		}
	}
	public interface ISaveAndLoad
	{
		Task SaveTextAsync (string filename, string text);
		Task<string> LoadTextAsync (string filename);
		string LoadText (string filename);
		bool FileExists (string filename);
	}
//	[assembly: Dependency (typeof (SaveAndLoad))]
//	public class SaveAndLoad : ISaveAndLoad {
//		public void SaveText (string filename, string text) {
//			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
//			var filePath = Path.Combine (documentsPath, filename);
//			System.IO.File.WriteAllText (filePath, text);
//		}
//		public string LoadText (string filename) {
//			var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
//			var filePath = Path.Combine (documentsPath, filename);
//			return System.IO.File.ReadAllText (filePath);
//		}
//	}

}
