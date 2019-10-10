using System.IO;
using System.Collections.Generic;
namespace Tools.EasyIO
{
	public static class EasyIO
	{
		public static void Write(string location, params string[] lines)
		{
			if (lines.Length == 0)
			{
				using (File.Create(location)) { } // Creates or empties a file
				return;
			}
			using (StreamWriter stream = new StreamWriter(location))
			{
				for (int i = 0; i < lines.Length - 1; i++)
					stream.WriteLine(lines[i]);

				stream.Write(lines[lines.Length - 1]); // Writes last line without a closing newline character
			}
		}
		public static void Write(string location, params byte[] bytes)
		{
			File.WriteAllBytes(location, bytes);
		}
		public static void Append(string location, params string[] lines)
		{
			using (StreamWriter stream = new StreamWriter(location, true))
			{
				stream.WriteLine(); // Starts on new line
				foreach (string line in lines)
				{
					stream.Write(line);
				}
			}
		}
		public static IEnumerable<string> Read(string location)
		{
			// Adapted from https://stackoverflow.com/a/23408020/11335381
			using (StreamReader stream = new StreamReader(location))
			{
				string line;
				while ((line = stream.ReadLine()) != null)
					yield return line;
			}
		}
		public static string ReadAll(string location)
		{
			string lines = "";
			using (StreamReader stream = new StreamReader(location))
			{
				lines = stream.ReadToEnd();
			}
			return lines;
		}
		public static Dictionary<string, string> ReadCsvDictionary(string file)
		{
			Dictionary<string, string> data = new Dictionary<string, string>();
			foreach (string keyPair in Read(file))
			{
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
