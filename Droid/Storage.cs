﻿using BBScore;
using BBScore.Droid;
using System;
using Xamarin.Forms;
using System.IO;
using System.Threading.Tasks;

[assembly: Dependency (typeof (SaveAndLoad_Android))]
namespace BBScore.Droid
{
	public class SaveAndLoad_Android : ISaveAndLoad
	{
		public async Task SaveTextAsync (string filename, string text)
		{
			var path = CreatePathToFile (filename);
			using (StreamWriter sw = File.CreateText (path))
				await sw.WriteAsync(text);
		}

		public string LoadText (string filename)
		{
			string path = CreatePathToFile (filename);
			using (StreamReader sr = File.OpenText(path))
				return sr.ReadToEnd();			
		}

		public async Task<string> LoadTextAsync (string filename)
		{
			var path = CreatePathToFile (filename);
			using (StreamReader sr = File.OpenText(path))
				return await sr.ReadToEndAsync();
		}

		public bool FileExists (string filename)
		{
			return File.Exists (CreatePathToFile (filename));
		}

		string CreatePathToFile (string filename)
		{
			var docsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			return Path.Combine(docsPath, filename);
		}
	}
}

