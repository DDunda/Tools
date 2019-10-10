using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace Tools.Async.AsyncEasyIO
{
	public static class AsyncEasyIO
	{
		public static async Task Write(string location, params string[] lines)
		{
			if (lines.Length == 0)
			{
				using (File.Create(location)) { } // Creates or empties a file
				return;
			}
			using (StreamWriter stream = new StreamWriter(location))
			{
				for (int i = 0; i < lines.Length - 1; i++)
					await stream.WriteLineAsync(lines[i]);

				await stream.WriteAsync(lines[lines.Length - 1]); // Writes last line without a closing newline character
			}
		}
		public static async Task Write(string location, params byte[] bytes)
		{
			await File.WriteAllBytesAsync(location, bytes);
		}
		public static async Task Append(string location, params string[] lines)
		{
			using (StreamWriter stream = new StreamWriter(location, true))
			{
				await stream.WriteLineAsync(); // Starts on new line
				foreach (string line in lines)
					await stream.WriteAsync(line);
			}
		}
		public static async Task<string> ReadAll(string location)
		{
			string lines = "";
			using (StreamReader stream = new StreamReader(location))
			{
				lines = await stream.ReadToEndAsync();
			}
			return lines;
		}
		public static async Task<Dictionary<string, string>> ReadCsvDictionary(string file)
		{
			Dictionary<string, string> data = new Dictionary<string, string>();
			using (StreamReader stream = new StreamReader(file))
			{
				string keyPair;
				while ((keyPair = await stream.ReadLineAsync()) != null)
					if (keyPair.Contains(',') && keyPair.IndexOf(',') == keyPair.LastIndexOf(','))
					{
						string[] splitInfo = keyPair.Split(',');
						data[splitInfo[0].Trim()] = splitInfo[1].Trim();
					}
			}
			return data;
		}
	}
}