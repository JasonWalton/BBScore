using System;
using System.ComponentModel;
using System.Threading;

using Xamarin.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BBScore.Messages;

namespace BBScore
{
	#region enumerations
	public enum WhichPlayer
	{
		None,
		Player1,
		Player2,
		Player3,
		Player4,
		Them
	}
	public enum BonusValue
	{
		None,
		BV_0,
		BV_5,
		BV_10,
		BV_15,
		BV_20,
		BV_25,
		BV_30,
		BV_35,
		BV_40
	}
	public enum GameType
	{
		RoundRobin,
		DoubleElim
	}
	public enum TimerState
	{
		NotStarted,
		Active,
		Paused
	}
	#endregion
	#region BoolColorConverter
	public sealed class BoolColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if ((bool)value)
				return Color.Lime;
			else
				return Color.White;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return new NotImplementedException ();
		}
	}
	#endregion
	#region DelegateCommand
	public class DelegateCommand : ICommand
	{
		readonly Func<bool> _canExecute;
		readonly Action<object> _execute;

		public DelegateCommand(Action<object> execute) : this(execute, null) { }
		public DelegateCommand(Action<object> execute, Func<bool> canExecute)
		{
			_execute = execute;
			_canExecute = canExecute;
		}
		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter) {
			return (_canExecute == null) || _canExecute ();
		}
		public void Execute(object parameter) { _execute ((object)parameter); }
		public void RaiseCanExecuteChanged() { CanExecuteChanged?.Invoke (this, EventArgs.Empty); }
	}
	#endregion
	public class Question
	{
		public Question(Game g, int i)
		{
			game = g;
			QuestionIndex = i;
		}
		private Game game;
		public int QuestionIndex;
		public WhichPlayer WhoGotIt { get; set; }
		public WhichPlayer WhoMissed { get; set; }
		public WhichPlayer WhoMissedRebound { get; set; }
		public BonusValue BonusPossible { get; set; }
		public BonusValue BonusEarned { get; set; }
		public bool WeGotIt {
			get {
				return  (WhoGotIt == WhichPlayer.Player1 || WhoGotIt == WhichPlayer.Player2 ||
				WhoGotIt == WhichPlayer.Player3 || WhoGotIt == WhichPlayer.Player4);
			}
		}
		public string ScoresheetLine { get { return ToString(); } }
		public string ScoresheetUs { get { return UsToString (); } }
		public string ScoresheetThem { get { return ThemToString (); } }
		public string QuestionIndexToString { get { return QuestionIndex.ToString(); } }
		public Question NullSelection { get {return null; } }

		public override string ToString ()
		{
			//generate line entry for the scoresheet view
			string scoreLine = "";
			scoreLine += ScoreString (true);
			scoreLine += " " + string.Format(QuestionIndex.ToString (), "00");
			scoreLine += "   " + ScoreString (false);
			return scoreLine;
//			return string.Format ("[Question: WhoGotIt={0}, WhoMissed={1}, WhoMissedRebound={2}, BonusPossible={3}, BonusEarned={4}, WeGotIt={5}, TheyGotIt={6}, TheyMissedIt={7}]", WhoGotIt, WhoMissed, WhoMissedRebound, BonusPossible, BonusEarned, WeGotIt, TheyGotIt, TheyMissedIt);
		}
		public string UsToString()
		{
			return ScoreString (true);
		}
		public string ThemToString()
		{
			return ScoreString (false);
		}
		private string ScoreString(bool us)
		{
			if (us && WeGotIt || !us && TheyGotIt)
			{
				string scoreString = "10 + ";
				scoreString += BonusString (BonusEarned) + "/" + BonusString (BonusPossible) + " = ";
				scoreString += CurrentScoreString (us) + (us ? "  " : " ");
				return scoreString;
			}
			else
				return new string (' ', (us ? 17 : 16));
		}
		private string BonusString(BonusValue bv)
		{
			switch (bv) {
			case BonusValue.BV_0:
				return " 0";
			case BonusValue.BV_5:
				return " 5";
			case BonusValue.BV_10:
				return "10";
			case BonusValue.BV_15:
				return "15";
			case BonusValue.BV_20:
				return "20";
			case BonusValue.BV_25:
				return "25";
			case BonusValue.BV_30:
				return "30";
			case BonusValue.BV_35:
				return "35";
			case BonusValue.BV_40:
				return "40";
			default:
				return "  ";
			}
		}
		private string CurrentScoreString(bool us)
		{
			return game.ScoreAtQuestion(QuestionIndex, us).ToString ();
		}
		public bool WeMissedIt {
			get {
				return  (WhoMissed == WhichPlayer.Player1 || WhoMissed == WhichPlayer.Player2 ||
					WhoMissed == WhichPlayer.Player3 || WhoMissed == WhichPlayer.Player4) ||
					(WhoMissedRebound == WhichPlayer.Player1 || WhoMissedRebound == WhichPlayer.Player2 ||
					WhoMissedRebound == WhichPlayer.Player3 || WhoMissedRebound == WhichPlayer.Player4);
			}
		}
		public bool TheyGotIt { get { return (WhoGotIt == WhichPlayer.Them); } }
		public bool TheyMissedIt { get { return (WhoMissed == WhichPlayer.Them || WhoMissedRebound == WhichPlayer.Them); } }
		public int BonusScore(BonusValue bonusValue)
		{
			switch (bonusValue) {
			case BonusValue.BV_0:
				return 0;
			case BonusValue.BV_5:
				return 5;
			case BonusValue.BV_10:
				return 10;
			case BonusValue.BV_15:
				return 15;
			case BonusValue.BV_20:
				return 20;
			case BonusValue.BV_25:
				return 25;
			case BonusValue.BV_30:
				return 30;
			case BonusValue.BV_35:
				return 35;
			case BonusValue.BV_40:
				return 40;
			default:
				return 0;
			}
		}
		public int Score(bool us)
		{
			if (us && WeGotIt || !us && TheyGotIt)
				return 10 + BonusScore (BonusEarned);
			else
				return 0;
		}
	}
		
	public class Game : INotifyPropertyChanged
	{
		public DelegateCommand PrevQuestionCommand { get; private set; }
		public DelegateCommand NextQuestionCommand { get; private set; }
		public DelegateCommand CorrectCommand { get; private set; }
		public DelegateCommand MissCommand { get; private set; }
		public DelegateCommand CorrectThemCommand { get; private set; }
		public DelegateCommand MissThemCommand { get; private set; }
		public DelegateCommand BonusPossibleCommand { get; private set; }
		public DelegateCommand BonusEarnedCommand { get; private set; }
		public DelegateCommand TimerTapCommand { get; private set; }
		public DelegateCommand ScoresheetCommand { get; private set; }
		public DelegateCommand ResetCommand { get; private set; }
		public Dictionary<int, Question> Questions { get; set; }
		public int QuestionIndex = 1;
		public GameType GType { get { return DataResource.GType; } set { DataResource.GType = value; } }
		public string P1Name { get { return DataResource.Player1; } set { DataResource.Player1 = value; } }
		public string P2Name { get { return DataResource.Player2; } set { DataResource.Player2 = value; } }
		public string P3Name { get { return DataResource.Player3; } set { DataResource.Player3 = value; } }
		public string P4Name { get { return DataResource.Player4; } set { DataResource.Player4 = value; } }
		public string TeamName { get { return DataResource.TeamName; } set { DataResource.TeamName = value; } }
		public string OppName { get { return DataResource.Opponent; } set { DataResource.Opponent = value; } }
		private TimerState _timerState;
		private int _secondsRemainingInGame;
		private string _timerString = "14:00";
		public string TimerString { get { return _timerString; } }
		public Color BackgroundColor { get { return this.GetBackgroundColor (); } }

		bool DidThisPlayerGetIt(WhichPlayer player) {
			return Questions [QuestionIndex].WhoGotIt == player;
		}

		bool DidThisPlayerMissIt(WhichPlayer player) {
			return Questions [QuestionIndex].WhoMissed == player || Questions [QuestionIndex].WhoMissedRebound == player; 
		}

		bool IsThisTheBonusPossible(BonusValue bonusValue) {
			return Questions [QuestionIndex].BonusPossible == bonusValue;
		}

		bool IsThisTheBonusEarned(BonusValue bonusValue) {
			return Questions [QuestionIndex].BonusEarned == bonusValue;
		}

		bool IsThisBonusEarnedPossible(BonusValue bonusValue) {
			if (Questions [QuestionIndex].BonusPossible == BonusValue.None)
				return true;
			return bonusValue <= Questions [QuestionIndex].BonusPossible;
		}

		public string QuestionIndexText { get { return QuestionIndex.ToString () + " / " + (Questions.Count).ToString ();} }
		public int UsScore { get { return CalculateUsScore (); } }
		public int ThemScore { get { return CalculateThemScore (); } }
		public bool CorrectStatusP1 { get { return DidThisPlayerGetIt (WhichPlayer.Player1); } }
		public bool CorrectStatusP2 { get { return DidThisPlayerGetIt (WhichPlayer.Player2); } }
		public bool CorrectStatusP3 { get { return DidThisPlayerGetIt (WhichPlayer.Player3); } }
		public bool CorrectStatusP4 { get { return DidThisPlayerGetIt (WhichPlayer.Player4); } }
		public bool CorrectStatusThem { get { return DidThisPlayerGetIt (WhichPlayer.Them); } }
		public bool MissStatusP1 { get { return DidThisPlayerMissIt (WhichPlayer.Player1); } }
		public bool MissStatusP2 { get { return DidThisPlayerMissIt (WhichPlayer.Player2); } }
		public bool MissStatusP3 { get { return DidThisPlayerMissIt (WhichPlayer.Player3); } }
		public bool MissStatusP4 { get { return DidThisPlayerMissIt (WhichPlayer.Player4); } }
		public bool MissStatusThem { get { return DidThisPlayerMissIt (WhichPlayer.Them); } }
		public bool BonusPossibleStatus20 { get { return IsThisTheBonusPossible (BonusValue.BV_20); } }
		public bool BonusPossibleStatus25 { get { return IsThisTheBonusPossible (BonusValue.BV_25); } }
		public bool BonusPossibleStatus30 { get { return IsThisTheBonusPossible (BonusValue.BV_30); } }
		public bool BonusPossibleStatus35 { get { return IsThisTheBonusPossible (BonusValue.BV_35); } }
		public bool BonusPossibleStatus40 { get { return IsThisTheBonusPossible (BonusValue.BV_40); } }
		public bool BonusEarnedStatus0 { get { return IsThisTheBonusEarned (BonusValue.BV_0); } }
		public bool BonusEarnedStatus5 { get { return IsThisTheBonusEarned (BonusValue.BV_5); } }
		public bool BonusEarnedStatus10 { get { return IsThisTheBonusEarned (BonusValue.BV_10); } }
		public bool BonusEarnedStatus15 { get { return IsThisTheBonusEarned (BonusValue.BV_15); } }
		public bool BonusEarnedStatus20 { get { return IsThisTheBonusEarned (BonusValue.BV_20); } }
		public bool BonusEarnedStatus25 { get { return IsThisTheBonusEarned (BonusValue.BV_25); } }
		public bool BonusEarnedStatus30 { get { return IsThisTheBonusEarned (BonusValue.BV_30); } }
		public bool BonusEarnedStatus35 { get { return IsThisTheBonusEarned (BonusValue.BV_35); } }
		public bool BonusEarnedStatus40 { get { return IsThisTheBonusEarned (BonusValue.BV_40); } }
		public bool BonusEarnedEnabled25 { get { return IsThisBonusEarnedPossible (BonusValue.BV_25); } }
		public bool BonusEarnedEnabled30 { get { return IsThisBonusEarnedPossible (BonusValue.BV_30); } }
		public bool BonusEarnedEnabled35 { get { return IsThisBonusEarnedPossible (BonusValue.BV_35); } }
		public bool BonusEarnedEnabled40 { get { return IsThisBonusEarnedPossible (BonusValue.BV_40); } }

		public Game ()
		{
			int totalQuestions = DataResource.GType == GameType.RoundRobin ? 15 : 20;
			_timerString = DataResource.GType == GameType.RoundRobin ? "14:00" : "10:00";
			Questions = new Dictionary<int, Question>(totalQuestions);
			for (int i = 1; i <= totalQuestions; i++)
				Questions [i] = new Question (this, i);
			InitTimer ();

			PrevQuestionCommand = new DelegateCommand (ExecutePrev);
			NextQuestionCommand = new DelegateCommand (ExecuteNext);
			CorrectCommand = new DelegateCommand (ExecuteCorrect);
			MissCommand = new DelegateCommand (ExecuteMiss);
			CorrectThemCommand = new DelegateCommand (ExecuteCorrectThem);
			MissThemCommand = new DelegateCommand (ExecuteMissThem);
			BonusPossibleCommand = new DelegateCommand (ExecuteBonusPossible);
			BonusEarnedCommand = new DelegateCommand (ExecuteBonusEarned);
			TimerTapCommand = new DelegateCommand (ExecuteTimerTap);
			ScoresheetCommand = new DelegateCommand (ExecuteScoresheet);
			ResetCommand = new DelegateCommand (ExecuteReset);
			HandleReceivedMessages ();
		}
		public void SetGameInProgress()
		{
			NavigationMessage.PutData<bool> ("GameInProgress", true);
		}
		public void UpdateGameData(Game gameWithNewSettings)
		{
			for (int i = 1; i <= 20; i++)
				this.Questions.Remove (i);
			for (int i = (GType == GameType.RoundRobin ? 15 : 20); i >= 1; i--)
			{
				if (gameWithNewSettings.Questions.ContainsKey (i))
					this.Questions.Add (i, gameWithNewSettings.Questions [i]);
				else
					this.Questions.Add (i, new Question (this, i));
			}
			UpdateQuestionIndex ();
			UpdateCorrectWrongProperties ();
		}
		public int ScoreAtQuestion(int questionIndex, bool myTeam)
		{
			int total = 0;
			for (int i = 1; i <= questionIndex; i++) {
				total += Questions [i].Score (myTeam);
			}
			return total;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void ExecutePrev(object nothing)
		{
			if (QuestionIndex != 1)
				QuestionIndex--;
			UpdateQuestionIndex ();
			UpdateCorrectWrongProperties ();
		}
		public void ExecuteNext(object nothing)
		{
			if (QuestionIndex != (Questions.Count))
				QuestionIndex++;
			UpdateQuestionIndex ();
			UpdateCorrectWrongProperties ();
		}
		public void ExecuteCorrect(object player_object)
		{
			SetGameInProgress ();
			WhichPlayer player = (WhichPlayer)player_object;

			//Credit WhoGotIt
			Questions[QuestionIndex].WhoGotIt = player;

			//Fix Miss or MissedRebound if it had been this player
			if (Questions [QuestionIndex].WeMissedIt) //.WhoMissed == player)
				Questions [QuestionIndex].WhoMissed = WhichPlayer.None;
//			else if (Questions [QuestionIndex].WhoMissedRebound == player)
//				Questions [QuestionIndex].WhoMissedRebound = WhichPlayer.None;
			
			UpdateCorrectWrongProperties ();
		}

		public void ExecuteMiss(object player_object)
		{
			SetGameInProgress ();
			WhichPlayer player = (WhichPlayer)player_object;

			//Fix WhoGotIt if it had been this player
			if (Questions [QuestionIndex].WeGotIt)
				Questions [QuestionIndex].WhoGotIt = WhichPlayer.None;

			//Clear miss if pressing the X again for the same player
			if (Questions [QuestionIndex].WhoMissed == player || Questions [QuestionIndex].WhoMissedRebound == player)
				Questions [QuestionIndex].WhoMissed = WhichPlayer.None;
			else if (Questions [QuestionIndex].WhoMissedRebound == player)
				Questions [QuestionIndex].WhoMissedRebound = WhichPlayer.None;
			//Assign Miss or MissedRebound
			else if (Questions [QuestionIndex].WeMissedIt) //.WhoMissed == WhichPlayer.None)
				Questions [QuestionIndex].WhoMissed = player;
			else
				Questions [QuestionIndex].WhoMissedRebound = player;
			

			UpdateCorrectWrongProperties ();
		}

		public void ExecuteCorrectThem(object player_object)
		{
			SetGameInProgress ();
			WhichPlayer player = (WhichPlayer)player_object;

			Questions [QuestionIndex].WhoGotIt = player;
			if (Questions [QuestionIndex].WhoMissed == player)
				Questions [QuestionIndex].WhoMissed = WhichPlayer.None;
			UpdateCorrectWrongProperties ();
		}

		public void ExecuteMissThem(object player_object)
		{
			SetGameInProgress ();
			WhichPlayer player = (WhichPlayer)player_object;

			if (Questions [QuestionIndex].WhoMissed == WhichPlayer.None)
				Questions [QuestionIndex].WhoMissed = player;
			else
				Questions [QuestionIndex].WhoMissedRebound = player;
			
			if (Questions [QuestionIndex].WhoGotIt == player)
				Questions [QuestionIndex].WhoGotIt = WhichPlayer.None;
			UpdateCorrectWrongProperties ();
		}
		public void ExecuteBonusPossible(object bonusValue_object)
		{
			SetGameInProgress ();
			BonusValue bonusValue = (BonusValue)bonusValue_object;

			Questions [QuestionIndex].BonusPossible = bonusValue;
			UpdateCorrectWrongProperties ();
		}

		public void ExecuteBonusEarned(object bonusValue_object)
		{
			SetGameInProgress ();
			BonusValue bonusValue = (BonusValue)bonusValue_object;

			Questions [QuestionIndex].BonusEarned = bonusValue;
			UpdateCorrectWrongProperties ();
		}

		private Color GetBackgroundColor()
		{
			if (_secondsRemainingInGame <= 60)
				return Color.Red;
			else
				return Color.Gray;
		}

		public void ExecuteTimerTap(object nothing)
		{
			SetGameInProgress ();
			switch (_timerState) {
			case TimerState.NotStarted:
				_timerState = TimerState.Active;
				var startMessage = new StartLongRunningTaskMessage ();
				MessagingCenter.Send (startMessage, "StartLongRunningTaskMessage");
				break;
			case TimerState.Active:
				_timerState = TimerState.Paused;
				break;
			case TimerState.Paused:
				_timerState = TimerState.Active;
				break;
			default:
				break;
			}
		}
		private void InitTimer()
		{
			_timerState = TimerState.NotStarted;
			_secondsRemainingInGame = (DataResource.GType == GameType.RoundRobin ? (14 * 60) : (10 * 60));
		}
		private void HandleReceivedMessages()
		{
			MessagingCenter.Subscribe<TickedMessage> (this, "TickedMessage", message => {
				Device.BeginInvokeOnMainThread(() => {
					switch(_timerState)
					{
					case TimerState.NotStarted:
					case TimerState.Paused:
						break;
					case TimerState.Active:
						_secondsRemainingInGame--;
						if (_secondsRemainingInGame == 0) {
							InitTimer();
							PropertyChanged(this, new PropertyChangedEventArgs("BackgroundColor"));
							MessagingCenter.Send (new StopLongRunningTaskMessage (), "StopLongRunningTaskMessage");
						}
						else if (_secondsRemainingInGame == 60)
							PropertyChanged(this, new PropertyChangedEventArgs("BackgroundColor"));
						_timerString = ConvertSecondsToTimerString();
						PropertyChanged(this, new PropertyChangedEventArgs("TimerString"));
						break;
					default:
						break;
					}
				});
			});

//			MessagingCenter.Subscribe<CancelledMessage> (this, "CancelledMessage", message => {
//				Device.BeginInvokeOnMainThread(() => {
//				});
//			});
		}
		private string ConvertSecondsToTimerString()
		{
			int minutes = (int)(_secondsRemainingInGame / 60);
			int seconds = _secondsRemainingInGame % 60;
			return minutes.ToString () + ":" + seconds.ToString ("00");
		}

		public void ExecuteScoresheet(object nothing)
		{
			NavigationMessage.PutData ("ActiveGame", this);
			App.Current.MainPage.Navigation.PushAsync (new Scoresheet ());
		}

		public async void ExecuteReset(object nothing)
		{
			NavigationMessage.PutData ("ActiveGame", this);
			Page page = await App.Current.MainPage.Navigation.PopAsync ();
		}

		public void RefreshView()
		{
			UpdateQuestionIndex ();
			UpdateCorrectWrongProperties ();
		}

		private void UpdateQuestionIndex()
		{
			PropertyChanged(this, new PropertyChangedEventArgs("QuestionIndexText"));
		}

		private void UpdateCorrectWrongProperties()
		{
			PropertyChanged(this, new PropertyChangedEventArgs("CorrectStatusP1"));
			PropertyChanged(this, new PropertyChangedEventArgs("CorrectStatusP2"));
			PropertyChanged(this, new PropertyChangedEventArgs("CorrectStatusP3"));
			PropertyChanged(this, new PropertyChangedEventArgs("CorrectStatusP4"));
			PropertyChanged(this, new PropertyChangedEventArgs("CorrectStatusThem"));
			PropertyChanged(this, new PropertyChangedEventArgs("MissStatusP1"));
			PropertyChanged(this, new PropertyChangedEventArgs("MissStatusP2"));
			PropertyChanged(this, new PropertyChangedEventArgs("MissStatusP3"));
			PropertyChanged(this, new PropertyChangedEventArgs("MissStatusP4"));
			PropertyChanged(this, new PropertyChangedEventArgs("MissStatusThem"));
			PropertyChanged(this, new PropertyChangedEventArgs("UsScore"));
			PropertyChanged(this, new PropertyChangedEventArgs("ThemScore"));
			PropertyChanged (this, new PropertyChangedEventArgs ("BonusPossibleStatus20"));
			PropertyChanged (this, new PropertyChangedEventArgs ("BonusPossibleStatus25"));
			PropertyChanged (this, new PropertyChangedEventArgs ("BonusPossibleStatus30"));
			PropertyChanged (this, new PropertyChangedEventArgs ("BonusPossibleStatus35"));
			PropertyChanged (this, new PropertyChangedEventArgs ("BonusPossibleStatus40"));
			PropertyChanged (this, new PropertyChangedEventArgs ("BonusEarnedStatus0"));
			PropertyChanged (this, new PropertyChangedEventArgs ("BonusEarnedStatus5"));
			PropertyChanged (this, new PropertyChangedEventArgs ("BonusEarnedStatus10"));
			PropertyChanged (this, new PropertyChangedEventArgs ("BonusEarnedStatus15"));
			PropertyChanged (this, new PropertyChangedEventArgs ("BonusEarnedStatus20"));
			PropertyChanged (this, new PropertyChangedEventArgs ("BonusEarnedStatus25"));
			PropertyChanged (this, new PropertyChangedEventArgs ("BonusEarnedStatus30"));
			PropertyChanged (this, new PropertyChangedEventArgs ("BonusEarnedStatus35"));
			PropertyChanged (this, new PropertyChangedEventArgs ("BonusEarnedStatus40"));
			PropertyChanged (this, new PropertyChangedEventArgs ("BonusEarnedEnabled25"));
			PropertyChanged (this, new PropertyChangedEventArgs ("BonusEarnedEnabled30"));
			PropertyChanged (this, new PropertyChangedEventArgs ("BonusEarnedEnabled35"));
			PropertyChanged (this, new PropertyChangedEventArgs ("BonusEarnedEnabled40"));
		}
		private int CalculateUsScore()
		{
			int score = 0;
			for (int i = 1; i <= Questions.Count; i++) {
				if (Questions[i].WeGotIt) {
					score += 10;
					score += Questions[i].BonusScore (Questions[i].BonusEarned);
				}
			}
			return score;
		}
		private int CalculateThemScore()
		{
			int score = 0;
			for (int i = 1; i < Questions.Count; i++) {
				if (Questions [i].TheyGotIt) {
					score += 10;
					score += Questions [i].BonusScore (Questions [i].BonusEarned);
				}
			}
			return score;
		}
	}
}

