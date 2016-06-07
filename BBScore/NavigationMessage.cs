using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace BBScore
{
	public static class NavigationMessage
	{
		public static void PutData<T>(string key, T value)
		{
			Application.Current.Properties[key] = value;
		}

		public static T GetData<T>(string key)
		{
			T value = default(T);

			IDictionary<string, object> iDictionary = Application.Current.Properties;

			if (iDictionary.ContainsKey(key))
			{
				value = (T)iDictionary[key];
			}

			return value;
		}

		public static bool KeyExists(string key)
		{
			return Application.Current.Properties.ContainsKey (key);
		}
	}
}

