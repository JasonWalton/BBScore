using BBScore;
using BBScore.iOS;
using System;
using Xamarin.Forms;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using System.Linq;

[assembly: Dependency (typeof(SaveAndLoad_iOS))]

namespace BBScore.iOS
{
	public class SaveAndLoad_iOS : ISaveAndLoad
	{
		public static string DocumentsPath {
			get {
				var documentsDirUrl = NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User).Last ();
				return documentsDirUrl.Path;
			}
		}

		public async Task SaveTextAsync (string filename, string text)
		{
			string path = CreatePathToFile (filename);
			using (StreamWriter sw = File.CreateText(path))
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
			string path = CreatePathToFile (filename);
			using (StreamReader sr = File.OpenText(path))
				return await sr.ReadToEndAsync();
		}

		public bool FileExists (string filename)
		{
			return File.Exists (CreatePathToFile (filename));
		}

		static string CreatePathToFile(string fileName)
		{
			return Path.Combine (DocumentsPath, fileName);
		}
	}
}